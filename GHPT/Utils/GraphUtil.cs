using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using System.Collections;

namespace GHPT.Utils
{
    public static class GraphUtil
    {
        public static void InstantiateComponent(GH_Document doc, string name, System.Drawing.PointF pivot)
        {
            try
            {
                IGH_ObjectProxy myProxy = Instances.ComponentServer.FindObjectByName(name, true, true);

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
