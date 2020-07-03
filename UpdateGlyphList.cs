using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        var source = @"D:\VS\src\vscommon\vsimages\Png";
        var destination = @"C:\GlyphList\src\img\images";

        var pngs = Directory.GetFiles(source, "*.16.16.png");
        foreach (var png in pngs)
        {
            var destinationPng = Path.Combine(destination, Path.GetFileName(png).Replace(".16.16", ""));
            if (!File.Exists(destinationPng))
            {
                File.Copy(png, destinationPng);
            }
        }
    }
}