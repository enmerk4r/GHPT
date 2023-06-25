using GHPT.Prompts;
using GHPT.Utils;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GHPT.Components
{
    public class GHPT : GH_Component, IGH_InitCodeAware
    {
        private GH_Document _doc;
        private PromptData _data;
        private bool _spinning;

        private Queue _queue;
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
            Ready += OnReady;
            _queue = new Queue();
        }

        private void OnReady(object sender, EventArgs e)
        {
            this._spinning = false;
            this.AddComponents();
            this.ConnectComponents();
            Grasshopper.Instances.RedrawCanvas();
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

            if (!configured)
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
            this._spinning = true;
            Task.Run(() =>
            {
                this.RunSpinner();
            });
            _data = await PromptUtils.AskQuestion(prompt);
            Ready?.Invoke(this, new EventArgs());
        }

        public event EventHandler Ready;

        public void AddComponents()
        {
            float x = this.Attributes.Pivot.X + 200;
            float y = this.Attributes.Pivot.Y;

            foreach (Addition addition in _data.Additions)
            {
                GraphUtil.InstantiateComponent(_doc, addition, new System.Drawing.PointF(x, y));
                x += 200;
            }
        }

        private void ConnectComponents()
        {
            foreach (ConnectionPairing connection in _data.Connections)
            {
                GraphUtil.ConnectComponent(_doc, connection);
            }
        }


        protected override void AfterSolveInstance()
        {
            base.AfterSolveInstance();
            if (this._queue.Count > 0)
            {
                this.CreatePromptPanel();
            }
            Grasshopper.Instances.RedrawCanvas();
            
        }

        public void SetInitCode(string code)
        {
            this._queue.Enqueue(code);
            GH_Panel panel = new GH_Panel();
            this.Params.Input[0].AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, code);
        }

        public void CreatePromptPanel()
        {
            string code = (string)this._queue.Dequeue();

            GH_Panel panel = new GH_Panel();
            panel.NickName = "GHPT Prompt";

            panel.UserText = code;
            //panel.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, code);

            _doc.AddObject(panel, false);

            panel.Attributes.Pivot = new System.Drawing.PointF(this.Attributes.Pivot.X - 250, this.Attributes.Pivot.Y - 50);

        }

        public void AdvanceSpinner()
        {
            List<string> sequence = new List<string>()
            {
                "Thinking: \\", "Thinking: |", "Thinking: /", "Thinking: -"
            };

            if (string.IsNullOrEmpty(this.Message))
            {
                this.Message = sequence[0];
            }
            else
            {
                int index = sequence.IndexOf(this.Message);
                int nextIndex = index + 1;
                if (nextIndex >= sequence.Count)
                {
                    nextIndex = 0;
                }

                this.Message = sequence[nextIndex];
            }
            Grasshopper.Instances.ActiveCanvas.BeginInvoke(new Action(() => {
                Grasshopper.Instances.RedrawCanvas();
            }));
            
            
        }

        public void RunSpinner()
        {
            while (_spinning)
            {
                this.AdvanceSpinner();
                Thread.Sleep(200);
            }
            this.Message = null;
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
        public override Guid ComponentGuid => new("ea3a2f90-b8b9-406f-bb66-f2a4b9fa3812");
    }
}