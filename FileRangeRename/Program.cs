using System;
using System.IO;
using System.Linq;

namespace FileRangeRename
{
    class Program
    {
        static void Main(string[] args)
        {
            var folder = @"D:\Timelapses\2017\SeptemberGrandViewPark";
            foreach (var file in Directory.GetFiles(folder))
            {
                var name = Path.GetFileNameWithoutExtension(file);
                var number = int.Parse(name.Substring(4));
                if (number > 1000)
                {
                    var newName = Path.Combine(folder, "IMG_" + (number - 1001 + 758).ToString().PadLeft(4, '0') + ".CR2");
                    File.Move(file, newName);
                }
            }
        }
    }
}
