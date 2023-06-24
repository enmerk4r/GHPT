using GHPT.Prompts;
using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GHPT.Utils
{

    public static class GHUtils
    {

        private static readonly Dictionary<int, Guid> idToComponents = new();


        //public static bool CreateComponents(GH_Document doc, IEnumerable<Addition> additions, IEnumerable<System.Drawing.PointF> pivots)
        //{
        //    List<bool> results = new(additions.Count());
        //    foreach (Addition addition in additions)
        //    {
        //        results.Add(InstantiateComponent(doc, addition));
        //    }

        //    return results.All(r => r);
        //}

        public static bool ConnectComponents(ConnectionPairing pairing)
        {
            return false;
        }

        public static void InstantiateComponent(GH_Document doc, Addition addition, System.Drawing.PointF pivot)
        {
            try
            {
                IGH_ObjectProxy myProxy = Instances.ComponentServer.FindObjectByName(addition.Name, true, true);

                Guid myId = myProxy.Guid;
                GH_Component myComponent = (GH_Component)Instances.ComponentServer.EmitObject(myId);
                myComponent.Attributes.Pivot = pivot;


                doc.AddObject(myComponent, false);
            }
            catch
            {
            }
        }

    }

}
