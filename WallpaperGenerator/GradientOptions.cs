using System.Drawing;
using GuiLabs.Utils;

namespace GuiLabs.Wallpaper
{
    public class GradientOptions
    {
        public GradientOptions()
        {
            InitWithRandom();
        }

        public void InitWithRandom()
        {
            LeftTop = Colors.GetRandom();
            LeftBottom = Colors.GetRandom();
            RightTop = Colors.GetRandom();
            RightBottom = Colors.GetRandom();
        }

        public Color LeftTop { get; set; }
        public Color RightTop { get; set; }
        public Color LeftBottom { get; set; }
        public Color RightBottom { get; set; }
    }
}
