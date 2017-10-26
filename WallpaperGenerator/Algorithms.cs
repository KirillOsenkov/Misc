using System.Drawing;
using GuiLabs.Utils;

namespace GuiLabs.Wallpaper
{
    public static class Algorithms
    {
        public static void CreateImage(string fileName, GradientOptions colors)
        {
            var bounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            Bitmap b = new Bitmap(bounds.Width, bounds.Height);
            Graphics g = Graphics.FromImage(b);
            Algorithms.GradientRect(
                g,
                new Rectangle(0, 0, bounds.Width, bounds.Height),
                colors);
            b.Save(fileName, System.Drawing.Imaging.ImageFormat.Bmp);
        }

        public static void GradientRect(Graphics g, Rectangle r, GradientOptions o)
        {
            GradientRect(g, r, 256, o);
        }

        public static void GradientRect(Graphics g, Rectangle r, int steps, GradientOptions o)
        {
            using (SolidBrush b = new SolidBrush(Color.WhiteSmoke))
            {
                for (int i = 0; i < steps; i++)
                {
                    float xratio = (float)i / steps;
                    int x0 = (int)(r.Left + r.Width * xratio);
                    int x1 = (int)(r.Left + r.Width * (float)(i + 1) / steps) - x0;
                    for (int j = 0; j < steps; j++)
                    {
                        float yratio = (float)j / steps;
                        int y0 = (int)(r.Top + r.Height * yratio);
                        int y1 = (int)(r.Top + r.Height * (float)(j + 1) / steps) - y0;

                        Color upper = Colors.Interpolate(o.LeftTop, o.RightTop, xratio);
                        Color lower = Colors.Interpolate(o.LeftBottom, o.RightBottom, xratio);
                        Color final = Colors.Interpolate(upper, lower, yratio);

                        b.Color = final;
                        g.FillRectangle(b, x0, y0, x1, y1);
                    }
                }
            }
        }
    }
}
