using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GHPT.Serialization
{
    public sealed class Serializer
    {

        private Dictionary<IGH_DocumentObject, int> documentObjects;

        public Serializer()
        {
            documentObjects = new();
        }

        public PromptData Serialize(GH_Document doc)
        {
            var actives = doc.ActiveObjects();
            var docObjs = doc.Objects.Where(o => !o.Obsolete && o.Name != "GHPT").ToArray();

            documentObjects = GetSerializableComponents(docObjs);

            PromptData data = new()
            {
                Additions = documentObjects.Select(kvp => DocumentObjectToAddition(kvp.Key, kvp.Value)),
                Connections = DocumentObjectToConnection(),
                Advice = GetAdvice(doc)
            };

            return data;
        }

        public void Deserialize(PromptData data)
        {
            throw new NotImplementedException();
        }

        private string GetAdvice(GH_Document doc)
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
            From = new() { Id = GetId(from, documentObjects), ParameterName = from.Name },
            To = new() { Id = GetId(to, documentObjects), ParameterName = to.Name }
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

    }


}
