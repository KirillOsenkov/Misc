using System;
using System.Windows.Forms;
using GuiLabs.Utils;

namespace GuiLabs.Wallpaper
{
    public partial class ColorChooser : UserControl
    {
        public ColorChooser()
        {
            InitializeComponent();
            this.BackColorChanged += ColorChooser_BackColorChanged;
            SetRandom();
        }

        public void SetRandom()
        {
            this.BackColor = Colors.GetRandom();
        }

        void ColorChooser_BackColorChanged(object sender, EventArgs e)
        {
            this.ButtonChooseColor.BackColor = this.BackColor;
        }

        private void ButtonChooseColor_Click(object sender, EventArgs e)
        {
            ColorDialog c = new ColorDialog();
            c.AnyColor = true;
            c.FullOpen = true;
            c.Color = this.BackColor;
            if (c.ShowDialog() != DialogResult.Cancel)
            {
                this.BackColor = c.Color;
            }
        }
    }
}
