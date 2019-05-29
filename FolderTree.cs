using System;
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

    // see also:
    // https://github.com/terrajobst/minsk/blob/a68d4dd880d1679a1f2f56c339ade5c95e73bf91/src/Minsk/CodeAnalysis/Binding/BoundNode.cs#L62
    private static void PrintTree(string root, string prefix = "")
    {
        var subdirectories = Directory.GetDirectories(root);
        var files = Directory.GetFiles(root);

        for (int i = 0; i < subdirectories.Length; i++)
        {
            bool isLast = i == subdirectories.Length - 1 && files.Length == 0;
            PrintEntry(subdirectories[i], isLast);

            string spacer = isLast ? "   " : "│  ";
            PrintTree(subdirectories[i], prefix + spacer);
        }

        for (int i = 0; i < files.Length; i++)
        {
            bool isLast = i == files.Length - 1;
            PrintEntry(files[i], isLast);
        }

        void PrintEntry(string fullPath, bool isLast)
        {
            string spacer = isLast ? "└──" : "├──";
            Console.WriteLine(prefix + spacer + Path.GetFileName(fullPath));
        }
    }
}
