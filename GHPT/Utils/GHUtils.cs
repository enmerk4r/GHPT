using GHPT.Prompts;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GHPT.Utils
{

    public static class GHUtils
    {

        private static readonly Dictionary<int, Guid> idToComponents = new();


        public static bool CreateComponents(GH_Document doc, IEnumerable<Addition> additions)
        {
            List<bool> results = new(additions.Count());
            foreach (Addition addition in additions)
            {
                results.Add(CreateComponent(doc, addition));
            }

            return results.All(r => r);
        }

        private static bool CreateComponent(GH_Document doc, Addition addition)
        {
            var docObj = doc.FindObjects(new List<string> { addition.Name }, 1).FirstOrDefault();
            // idToComponents.Add(addition.Id, componentId);
            return doc.AddObject(docObj, true);
        }

        public static bool ConnectComponents(ConnectionPairing pairing)
        {
            return false;
        }

    }

}
