using GHPT.Prompts;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using System;
using System.Collections.Generic;

namespace GHPT.Utils
{
    public static class GraphUtil
    {

        private static readonly Dictionary<string, string> fuzzyPairs = new()
        {
            { "Extrusion", "Extrude" },
            { "Text Panel", "Panel" }
        };


        public static void InstantiateComponent(GH_Document doc, Addition addition, System.Drawing.PointF pivot)
        {
            try
            {
                string name = addition.Name;
                IGH_ObjectProxy myProxy = GetObject(name);
                if (myProxy is null)
                    return;

                Guid myId = myProxy.Guid;
                var emit = Instances.ComponentServer.EmitObject(myId);
                ;

                doc.AddObject(emit, false);
                emit.Attributes.Pivot = pivot;
                SetValue(addition, emit);
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
                _ => false
            };

        }

        private static bool SetPanelData(Addition addition, GH_Panel panel)
        {
            panel.SetUserText(addition.Value);
            return true;
        }

        private static bool SetNumberSliderData(Addition addition, GH_NumberSlider slider)
        {
            string value = addition.Value;
            slider.SetInitCode(value);

            return true;
        }

    }
}
