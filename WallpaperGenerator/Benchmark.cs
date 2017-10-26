using System;
using System.Drawing;
using System.Windows.Forms;
using GuiLabs.Canvas;
using GuiLabs.Utils;

namespace GuiLabs.Wallpaper
{
    public partial class Benchmark : Form
    {
        public Benchmark()
        {
            InitializeComponent();
        }

        GradientOptions colors = new GradientOptions();

        private void drawWindow1_Repaint(GuiLabs.Canvas.Renderer.IRenderer Renderer)
        {
            var rect = new Rect(0, 0, drawWindow1.Width, drawWindow1.Height);
            var duration = Common.MeasureExecutionTime(() =>
                Renderer.DrawOperations.GradientFill4(
                    rect,
                    colors.LeftTop,
                    colors.RightTop,
                    colors.LeftBottom,
                    colors.RightBottom));

            Renderer.DrawOperations.DrawString(
                duration.ToString(),
                rect,
                Renderer.DrawOperations.Factory.ProduceNewFontStyleInfo(
                    "Arial", 10, FontStyle.Regular));
        }

        private void drawWindow2_Paint(object sender, PaintEventArgs e)
        {
            var graphics = e.Graphics;
            var duration = Common.MeasureExecutionTime(() =>
                Algorithms.GradientRect(graphics, drawWindow2.ClientRectangle, colors));

            graphics.DrawString(
                duration.ToString(),
                SystemFonts.DefaultFont,
                SystemBrushes.ControlText,
                0,
                0);
        }

        private void drawWindow1_Click(object sender, EventArgs e)
        {
            drawWindow1.Refresh();
        }

        private void drawWindow2_Click(object sender, EventArgs e)
        {
            drawWindow2.Refresh();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            drawWindow1.Refresh();
            drawWindow2.Refresh();
        }
    }
}