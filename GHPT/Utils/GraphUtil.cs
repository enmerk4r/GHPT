using FuzzySharp;
using FuzzySharp.Extractor;
using GHPT.Prompts;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Special;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GHPT.Utils
{
    public static class GraphUtil
    {

        private static readonly Dictionary<string, string> fuzzyPairs = new()
        {
            { "Extrusion", "Extrude" },
            { "Text Panel", "Panel" }
        };

        private static readonly Dictionary<int, IGH_DocumentObject> CreatedComponents = new();


        public static void InstantiateComponent(GH_Document doc, Addition addition, System.Drawing.PointF pivot)
        {
            try
            {
                string name = addition.Name;
                IGH_ObjectProxy myProxy = GetObject(name);
                if (myProxy is null)
                    return;

                Guid myId = myProxy.Guid;

                if (CreatedComponents.ContainsKey(addition.Id))
                {
                    CreatedComponents.Remove(addition.Id);
                }

                var emit = Instances.ComponentServer.EmitObject(myId);
                CreatedComponents.Add(addition.Id, emit);

                doc.AddObject(emit, false);
                emit.Attributes.Pivot = pivot;
                SetValue(addition, emit);
            }
            catch
            {
            }
        }

        public static void ConnectComponent(GH_Document doc, ConnectionPairing pairing)
        {
            CreatedComponents.TryGetValue(pairing.From.Id, out IGH_DocumentObject componentFrom);
            CreatedComponents.TryGetValue(pairing.To.Id, out IGH_DocumentObject componentTo);

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

        private static IGH_Param GetParam(IGH_DocumentObject docObj, Connection connection, bool isInput)
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

            if (_params?.Count()! > 0)
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

        private static IGH_ObjectProxy GetObject(string name)
        {
            IGH_ObjectProxy myProxy = Instances.ComponentServer.FindObjectByName(name, true, true);
            if (myProxy is null)
            {
                if (fuzzyPairs.ContainsKey(name))
                {
                    name = fuzzyPairs[name];
                }

                myProxy = Instances.ComponentServer.FindObjectByName(name, true, true);
            }

            return myProxy;
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
