using System;
using System.Windows.Forms;

namespace GuiLabs.Wallpaper
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.Run(new GradientWallpaperGenerator());
        }
    }
}