using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        var root = Path.GetTempPath();
        foreach (var directory in Directory.GetDirectories(root))
        {
            if (IsEmpty(directory))
            {
                try
                {
                    Directory.Delete(directory);
                }
                catch
                {
                }
            }
        }
    }

    static bool IsEmpty(string directory)
    {
        try
        {
            var directories = Directory.GetDirectories(directory);
            var files = Directory.GetFiles(directory);
            return directories.Length == 0 && files.Length == 0;
        }
        catch
        {
            return false;
        }
    }
}