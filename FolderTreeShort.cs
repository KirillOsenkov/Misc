using System;
using System.IO;

class FolderTree
{
    static void Main()
    {
        PrintTree(Environment.CurrentDirectory);
    }

    static void PrintTree(string root, string prefix = "")
    {
        var subdirectories = Directory.GetDirectories(root);

        for (int i = 0; i < subdirectories.Length; i++)
        {
            bool isLast = i == subdirectories.Length - 1;
            string spacer = isLast ? "└───" : "├───";
            Console.WriteLine(prefix + spacer + Path.GetFileName(subdirectories[i]));

            spacer = isLast ? "    " : "│   ";
            PrintTree(subdirectories[i], prefix + spacer);
        }
    }
}
