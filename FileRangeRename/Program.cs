﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileRangeRename
{
    class Program
    {
        static void Main(string[] args)
        {
            var folder = @"D:\Timelapses\2020\May 6 Sunset";

            var files = Directory.GetFiles(folder).OrderBy(f => f).ToArray();

            foreach (var file in files)
            {
                int number = GetNumber(file);
                if (number < 1000)
                {
                    number += 1000;
                }
                else if (number > 9000)
                {
                    number -= 9000;
                }

                var newName = GetFilePath(folder, number);
                File.Move(file, newName);
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
