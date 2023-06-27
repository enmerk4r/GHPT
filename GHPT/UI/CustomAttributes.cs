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

                GH_Skin.palette_hidden_standard = new GH_PaletteStyle(Colours.Background, Colours.Border, Colours.Text);

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
