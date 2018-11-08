using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileRangeRename
{
    class Program
    {
        static void Main(string[] args)
        {
            var folder = @"D:\Timelapses\2018\March 17";

            var files = Directory.GetFiles(folder).OrderBy(f => f).ToArray();

            foreach (var file in files)
            {
                int number = GetNumber(file);
                if (number < 177)
                {
                    number += 311;
                    var newName = GetFilePath(folder, number);
                    File.Move(file, newName);
                }
            }
        }

        private static string GetFilePath(string folder, int index)
        {
            return Path.Combine(folder, "IMG_" + index.ToString().PadLeft(4, '0') + ".CR2");
        }

        private static int GetNumber(string file)
        {
            var name = Path.GetFileNameWithoutExtension(file);
            var number = int.Parse(name.Substring(4));
            return number;
        }
    }
}
