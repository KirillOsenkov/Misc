﻿using System;
using System.IO;

class FolderTree
{
    static void Main(string[] args)
    {
        string root = Environment.CurrentDirectory;
        if (args.Length == 1 && Directory.Exists(args[0]))
        {
            root = args[0];
        }

        Console.WriteLine(Path.GetFileName(root));
        PrintTree(root);
    }

    private static void PrintTree(string root, string prefix = "")
    {
        var subdirectories = Directory.GetDirectories(root);
        var files = Directory.GetFiles(root);

        for (int i = 0; i < subdirectories.Length; i++)
        {
            if (i == subdirectories.Length - 1 && files.Length == 0)
            {
                Console.WriteLine(prefix + "└──" + Path.GetFileName(subdirectories[i]));
                PrintTree(subdirectories[i], prefix + "   ");
            }
            else
            {
                Console.WriteLine(prefix + "├──" + Path.GetFileName(subdirectories[i]));
                PrintTree(subdirectories[i], prefix + "│  ");
            }
        }

        for (int i = 0; i < files.Length; i++)
        {
            if (i == files.Length - 1)
            {
                Console.WriteLine(prefix + "└──" + Path.GetFileName(files[i]));
            }
            else
            {
                Console.WriteLine(prefix + "├──" + Path.GetFileName(files[i]));
            }
        }
    }
}
