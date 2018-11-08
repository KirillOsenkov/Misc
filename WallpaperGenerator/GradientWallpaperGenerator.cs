using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GuiLabs.Canvas;
using GuiLabs.Utils;

namespace GuiLabs.Wallpaper
{
    public partial class GradientWallpaperGenerator : Form
    {
        public GradientWallpaperGenerator()
        {
            InitializeComponent();
            Randomize();
        }

        private void drawWindow1_Repaint(GuiLabs.Canvas.Renderer.IRenderer Renderer)
        {
            Renderer.DrawOperations.GradientFill4(
                new Rect(0, 0, drawWindow1.Width, drawWindow1.Height),
                ColorTopLeft.BackColor,
                ColorTopRight.BackColor,
                ColorBottomLeft.BackColor,
                ColorBottomRight.BackColor,
                128);
        }

        private bool suspend = false;

        public void Repaint()
        {
            if (suspend)
            {
                return;
            }

            drawWindow1.Refresh();
        }

        private void ColorTopLeft_BackColorChanged(object sender, EventArgs e)
        {
            Repaint();
        }

        private void colorTopRight_BackColorChanged(object sender, EventArgs e)
        {
            Repaint();
        }

        private void ColorBottomLeft_BackColorChanged(object sender, EventArgs e)
        {
            Repaint();
        }

        private void ColorBottomRight_BackColorChanged(object sender, EventArgs e)
        {
            Repaint();
        }

        private void drawWindow1_Resize(object sender, EventArgs e)
        {
            Repaint();
        }

        public GradientOptions GetColors()
        {
            GradientOptions result = new GradientOptions();
            result.LeftBottom = ColorBottomLeft.BackColor;
            result.LeftTop = ColorTopLeft.BackColor;
            result.RightBottom = ColorBottomRight.BackColor;
            result.RightTop = ColorTopRight.BackColor;
            return result;
        }

        private void ButtonSetAsWallpaper_Click(object sender, EventArgs e)
        {
            GradientOptions options = GetColors();
            // on Windows XP, only bmp seems to be updated correctly
            // on Vista and above, this could be .jpg (gradients are best compressed with .jpg)
            string fileName = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                @"GradientWallpaper.bmp");
            Algorithms.CreateImage(fileName, options);
            API.SetDesktopWallpaper(fileName);
        }

        private void ButtonFillRandom_Click(object sender, EventArgs e)
        {
            Randomize();
        }

        void Randomize()
        {
            suspend = true;
            ColorTopLeft.SetRandom();
            ColorTopRight.SetRandom();
            ColorBottomLeft.SetRandom();
            ColorBottomRight.SetRandom();
            suspend = false;
            Repaint();
        }

        public void SaveToFile()
        {
            SaveToFile("colors.txt");
        }

        public void SaveToFile(string fileName)
        {
            File.AppendAllText(fileName, SaveColors() + Environment.NewLine);
        }

        public string SaveColors()
        {
            return SaveColor(ColorTopLeft) +
                SaveColor(ColorTopRight) +
                SaveColor(ColorBottomLeft) +
                SaveColor(ColorBottomRight);
        }

        public string SaveColor(ColorChooser c)
        {
            return SaveColor(c.BackColor) + Environment.NewLine;
        }

        public string SaveColor(Color c)
        {
            return c.ToString();
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            SaveToFile();
        }
    }
}