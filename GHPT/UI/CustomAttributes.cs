using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using System.Drawing;

namespace GHPT.UI
{

    // https://discourse.mcneel.com/t/change-the-color-of-the-custom-component/56435
    public sealed class CustomAttributes : GH_ComponentAttributes
    {
        public CustomAttributes(IGH_Component component) : base(component)
        {
        }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            if (channel == GH_CanvasChannel.Objects)
            {
                GH_PaletteStyle style = GH_Skin.palette_hidden_standard;

                Color GreenGPT = Color.FromArgb(25, 195, 125);
                Color PurpleGPT = Color.FromArgb(171, 104, 255);
                Color BackgroundGPT = Color.FromArgb(64, 65, 79);
                Color BorderGPT = Color.FromArgb(32, 33, 35);
                GH_Skin.palette_hidden_standard = new GH_PaletteStyle(BackgroundGPT, BorderGPT, Color.White);

                base.Render(canvas, graphics, channel);

                GH_Skin.palette_hidden_standard = style;
            }
            else
            {
                base.Render(canvas, graphics, channel);
            }
        }
    }
}
