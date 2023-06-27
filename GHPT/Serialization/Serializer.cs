using FuzzySharp;
using FuzzySharp.Extractor;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Special;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GHPT.Serialization
{
    public sealed class Serializer
    {

        private readonly Dictionary<int, IGH_DocumentObject> createdComponents;
        private Dictionary<IGH_DocumentObject, int> documentObjects;
        private readonly GH_Document doc;

        public Serializer(GH_Document ghDocument)
        {
            createdComponents = new();
            documentObjects = new();
            doc = ghDocument;
        }

        // Serialization

        public PromptData Serialize()
        {
            var actives = doc.ActiveObjects();
            var docObjs = doc.Objects.Where(o => !o.Obsolete && o.Name != "GHPT").ToArray();

            documentObjects = GetSerializableComponents(docObjs);

            PromptData data = new()
            {
                Additions = documentObjects.Select(kvp => DocumentObjectToAddition(kvp.Key, kvp.Value)),
                Connections = DocumentObjectToConnection(),
                Advice = GetAdvice()
            };

            return data;
        }

        private string GetAdvice()
        {
            GH_Panel panel = doc.Objects.OfType<GH_Panel>().FirstOrDefault(p => p.UserText == "Advice");
            return panel.UserText;
        }

        private IEnumerable<ConnectionPairing> DocumentObjectToConnection()
        {
            List<ConnectionPairing> pairings = new();

            foreach (var docObj in documentObjects.Keys)
            {
                if (docObj is IGH_Param param)
                {
                    pairings.AddRange(GetConnections(param));
                }
                else if (docObj is IGH_Component component)
                {
                    foreach (IGH_Param compParam in component.Params)
                    {
                        pairings.AddRange(GetConnections(compParam));
                    }
                }
            }

            return pairings;
        }

        private IEnumerable<ConnectionPairing> GetConnections(IGH_Param param)
        {
            foreach (var source in param.Sources)
            {
                yield return GetConnection(param, source);
            }
        }

        private ConnectionPairing GetConnection(IGH_Param from, IGH_Param to) => new()
        {
            From = new() { Id = GetId(from), ParameterName = from.Name },
            To = new() { Id = GetId(to), ParameterName = to.Name }
        };

        private int GetId(IGH_DocumentObject docObj)
                => documentObjects.FirstOrDefault(_do => _do.Key == docObj).Value;

        private static bool ToSerialize(IGH_DocumentObject docObj)
        {
            var ass = docObj.GetType().Assembly;

            if (docObj is IGH_Param param)
            {
                bool noSources = param.SourceCount == 0;
                return noSources;
            }
            else if (docObj is IGH_Component component)
            {
                if (component.Name == "GHPT")
                    return false;

                foreach (IGH_Param cParam in component.Params)
                {
                    if (cParam.SourceCount > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static Dictionary<IGH_DocumentObject, int> GetSerializableComponents(IEnumerable<IGH_DocumentObject> docObjs)
        {
            Dictionary<IGH_DocumentObject, int> _objects = new(docObjs.Count());
            int i = 0;
            foreach (var docObj in docObjs)
            {
                if (!ToSerialize(docObj))
                    continue;
                i++;
                _objects.Add(docObj, i);
            }

            return _objects;
        }

        private static Addition DocumentObjectToAddition(IGH_DocumentObject docObj, int i) => new()
        {
            Id = i,
            Name = docObj.Name
        };

        // DeSerialization
        public void Deserialize(PromptData data)
        {
            foreach (Addition addition in data.Additions)
            {
                CreateComponent(addition);
            }

            foreach (ConnectionPairing pairing in data.Connections)
            {
                ConnectComponent(pairing);
            }
        }

        private void CreateComponent(Addition addition)
        {
            try
            {
                string name = addition.Name;
                IGH_ObjectProxy myProxy = GetObject(name);
                if (myProxy is null)
                    return;

                Guid myId = myProxy.Guid;

                if (createdComponents.ContainsKey(addition.Id))
                {
                    createdComponents.Remove(addition.Id);
                }

                var emit = Instances.ComponentServer.EmitObject(myId);
                createdComponents.Add(addition.Id, emit);

                doc.AddObject(emit, false);
                SetValue(addition, emit);
            }
            catch
            {
            }
        }

        private static readonly Dictionary<string, string> fuzzyPairs = new()
        {
            { "Extrusion", "Extrude" },
            { "Text Panel", "Panel" }
        };

        private static IGH_ObjectProxy GetObject(string name)
        {
            IGH_ObjectProxy[] results = Array.Empty<IGH_ObjectProxy>();
            double[] resultWeights = new double[] { 0 };
            Instances.ComponentServer.FindObjects(new string[] { name }, 10, ref results, ref resultWeights);

            var myProxies = results.Where(ghpo => ghpo.Kind == GH_ObjectType.CompiledObject);

            var _components = myProxies.OfType<IGH_Component>();
            var _params = myProxies.OfType<IGH_Param>();

            // Prefer Components to Params
            var myProxy = myProxies.First();
            if (_components is not null)
                myProxy = _components.FirstOrDefault() as IGH_ObjectProxy;
            else if (myProxy is not null)
                myProxy = _params.FirstOrDefault() as IGH_ObjectProxy;

            // Sort weird names
            if (fuzzyPairs.ContainsKey(name))
            {
                name = fuzzyPairs[name];
            }

            myProxy = Instances.ComponentServer.FindObjectByName(name, true, true);

            return myProxy;
        }

        public void ConnectComponent(ConnectionPairing pairing)
        {
            createdComponents.TryGetValue(pairing.From.Id, out IGH_DocumentObject componentFrom);
            createdComponents.TryGetValue(pairing.To.Id, out IGH_DocumentObject componentTo);

            IGH_Param fromParam = GetParam(componentFrom, pairing.From, false);
            IGH_Param toParam = GetParam(componentTo, pairing.To, true);

            if (fromParam is null || toParam is null)
            {
                return;
            }

            toParam.AddSource(fromParam);
            toParam.CollectData();
            toParam.ComputeData();
        }

        private IGH_Param GetParam(IGH_DocumentObject docObj, Connection connection, bool isInput)
        {
            var resultParam = docObj switch
            {
                IGH_Param param => param,
                IGH_Component component => GetComponentParam(component, connection, isInput),
                _ => null
            };

            return resultParam;
        }

        private static IGH_Param GetComponentParam(IGH_Component component, Connection connection, bool isInput)
        {
            IList<IGH_Param> _params = (isInput ? component.Params.Input : component.Params.Output)?.ToArray();

            if (_params is null || _params.Count == 0)
                return null;

            if (_params.Count() <= 1)
                return _params.First();

            // Linq Alternative to below
            // _params.First(p => p.Name.ToLowerInvariant() == connection.ParameterName.ToLowerInvariant());
            foreach (var _param in _params)
            {
                if (_param.Name.ToLowerInvariant() == connection.ParameterName.ToLowerInvariant())
                {
                    return _param;
                }
            }

            ExtractedResult<string> fuzzyResult = Process.ExtractOne(connection.ParameterName, _params.Select(_p => _p.Name));
            if (fuzzyResult.Score >= 50)
            {
                return _params[fuzzyResult.Index];
            }

            return null;
        }

        private static void SetValue(Addition addition, IGH_DocumentObject ghProxy)
        {
            string lowerCaseName = addition.Name.ToLowerInvariant();

            bool result = ghProxy switch
            {
                GH_NumberSlider slider => SetNumberSliderData(addition, slider),
                GH_Panel panel => SetPanelData(addition, panel),
                Param_Point point => SetPointData(addition, point),
                _ => false
            };

        }

        private static bool SetPointData(Addition addition, Param_Point point)
        {
            try
            {
                if (string.IsNullOrEmpty(addition.Value))
                    return false;

                string[] pointValues = addition.Value.Replace("{", "").Replace("}", "").Split(',');
                double[] pointDoubles = pointValues.Select(p => double.Parse(p)).ToArray();

                point.SetPersistentData(new Rhino.Geometry.Point3d(pointDoubles[0], pointDoubles[1], pointDoubles[2]));
            }
            catch
            {
                point.SetPersistentData(new Rhino.Geometry.Point3d(0, 0, 0));
            }
            finally
            {
                point.CollectData();
                point.ComputeData();
            }

            return true;
        }

        private static bool SetPanelData(Addition addition, GH_Panel panel)
        {
            panel.SetUserText(addition.Value);
            return true;
        }

        private static bool SetNumberSliderData(Addition addition, GH_NumberSlider slider)
        {
            string value = addition.Value;
            if (string.IsNullOrEmpty(value)) return false;
            slider.SetInitCode(value);

            return true;
        }

    }


}
