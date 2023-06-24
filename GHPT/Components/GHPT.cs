using GHPT.IO;
using GHPT.Utils;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Windows.Media.Animation;

namespace GHPT.Components
{
    public class GHPT : GH_Component, IGH_InitCodeAware
    {
        private GH_Document _doc;
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public GHPT()
          : base("GHPT", "GHPT",
            "A component that lets you use ChatGPT to instantiate Grasshopper snippets from a prompt",
            "GHPT", "Prompt")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Prompt", "P", "LLM prompt for instantiating components", GH_ParamAccess.item);
            pManager.AddNumberParameter("Temperature", "T", "Controls how \"creatively\" the network responds to your prompt", GH_ParamAccess.item, 0.7);

            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected async override void SolveInstance(IGH_DataAccess DA)
        {
            _doc = OnPingDocument();

            bool configured = ConfigUtil.CheckConfiguration();
            
            if(!configured)
            {
                ConfigUtil.PromptUserForConfig();
            }
            else
            {
                ConfigUtil.LoadConfig();
            }

            string prompt = string.Empty;
            double temperature = 0.7;

            DA.GetData(0, ref prompt);
            DA.GetData(1, ref temperature);

            ResponsePayload response = await ClientUtil.Ask(prompt);

            string content = response.Choices.First().Message.Content;

            
        }

        public void AddComponents()
        {
            List<string> components = new List<string>()
            {
                "Circle",
                "Circle CNR",
                "Close Curve",
                "Arc"
            };

            float x = this.Attributes.Pivot.X + 200;
            float y = this.Attributes.Pivot.Y;

            foreach (string name in components)
            {
                GraphUtil.InstantiateComponent(_doc, name, new System.Drawing.PointF(x, y));
                x += 200;
            }
        }

        public void SelfDestruct()
        {
            this._doc.RemoveObject(this.Attributes, true);
        }

        protected override void AfterSolveInstance()
        {
            this.AddComponents();
            base.AfterSolveInstance();
            Grasshopper.Instances.RedrawCanvas();
            this.SelfDestruct();
        }

        public void SetInitCode(string code)
        {
            this.Params.Input[0].AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, code);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => null;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("ea3a2f90-b8b9-406f-bb66-f2a4b9fa3812");
    }
}