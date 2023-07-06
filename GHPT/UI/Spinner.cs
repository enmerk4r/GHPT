using Grasshopper.Kernel;

namespace GHPT.UI
{
    public sealed class Spinner
    {
        private bool spinning { get; set; } = false;

        private readonly GH_Component _component;

        public Spinner(GH_Component component)
        {
            this._component = component;
        }

        internal char[] ThoughtSequence = new char[]
        {
            '\\', '|', '/', '—',
        };

        private int currIndex = 0;
        public void Advance()
        {
            if (currIndex >= ThoughtSequence.Length)
                currIndex = 0;

            _component.Message = $"Thinking: {ThoughtSequence[currIndex]}";
            currIndex += 1;
        }

        public void Start()
        {
            spinning = true;
            while (spinning)
            {
                Advance();
                System.Threading.Thread.Sleep(200);
                Grasshopper.Instances.ActiveCanvas.BeginInvoke(new Action(() =>
                {
                    Grasshopper.Instances.RedrawCanvas();
                }));
            }
        }

        public void Stop()
        {
            spinning = false;
            _component.Message = null;
            currIndex = 0;
        }

    }
}
