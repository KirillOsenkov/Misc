using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

class Program
{
    static int Main(string[] args)
    {
        if (args.Length != 1 || args[0] is not string arg || !int.TryParse(arg, out int count))
        {
            Console.Error.WriteLine("Usage: createfiles 1000");
            return 1;
        }

        var directory = Directory.GetCurrentDirectory();
        var files = Directory.GetFiles(directory);
        if (files.Any())
        {
            Console.Error.WriteLine($"Directory is expected to be empty: {directory}");
            return 2;
        }

        for (int i = 0; i < count; i++)
        {
            string path = Path.Combine(directory, $"{i}.txt");
            File.WriteAllText(path, $"{i}");
        }

        return 0;
    }
}