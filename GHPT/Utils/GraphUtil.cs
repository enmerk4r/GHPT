using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;

namespace GHPT.Utils
{
    public static class GraphUtil
    {

        private static readonly Dictionary<string, string> fuzzyPairs = new()
        {
            { "Extrusion", "Extrude" },
        };


        public static void InstantiateComponent(GH_Document doc, string name, System.Drawing.PointF pivot)
        {
            try
            {
                IGH_ObjectProxy myProxy = GetObject(name);

                Guid myId = myProxy.Guid;
                GH_Component myComponent = (GH_Component)Instances.ComponentServer.EmitObject(myId);
                myComponent.Attributes.Pivot = pivot;


                doc.AddObject(myComponent, false);
            }
            catch
            {
            }
        }

        private static IGH_ObjectProxy GetObject(string name)
        {
            IGH_ObjectProxy myProxy = Instances.ComponentServer.FindObjectByName(name, true, true);
            if (myProxy is null)
            {
                if (fuzzyPairs.ContainsKey(name))
                {
                    myProxy = Instances.ComponentServer.FindObjectByName(fuzzyPairs[name], true, true);
                }
            }

            return myProxy;
        }

    }
}
