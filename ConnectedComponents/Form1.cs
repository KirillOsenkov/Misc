using System;
using System.Drawing;
using System.Windows.Forms;

namespace ConnectedComponents
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Field = new Field();
        }

        Field Field { get; set; }

        public void Repaint()
        {
            Drawing.Invalidate();
        }

        private void Drawing_Paint(object sender, PaintEventArgs e)
        {
            FieldPainter.Draw(Field, Drawing, e.Graphics);
        }
        
        private void Form1_Resize(object sender, EventArgs e)
        {
            int maxFieldSize = Math.Max(5, Math.Min(Drawing.ClientSize.Width, Drawing.ClientSize.Height));
            FieldSizeSlider.Maximum = maxFieldSize;
            Repaint();
        }

        private void FindComponents_Click(object sender, EventArgs e)
        {
            var components = Algorithm.FindConnectedComponents(Field);
            foreach (var component in components)
            {
                Color random = Field.GetRandomColor();
                foreach (var span in component.Spans)
                {
                    for (int x = span.StartX; x <= span.EndX; x++)
                    {
                        Field[x, span.Y] = random;
                    }
                }
            }
            Status.Text = string.Format("Found {0} connected components", components.Count);
            Repaint();
        }

        public void Regenerate()
        {
            Field = new Field((double)PercentageSlider.Value / PercentageSlider.Maximum, FieldSizeSlider.Value);
            Status.Text = "";
            Repaint();
        }

        private void PercentageSlider_Scroll(object sender, EventArgs e)
        {
            Regenerate();
        }

        private void FieldSizeSlider_Scroll(object sender, EventArgs e)
        {
            Regenerate();
        }
    }
}
