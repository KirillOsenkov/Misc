using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine(@"Usage: FileLocker.exe <path-to-file.dll>
    Locks the file until you press a button.");
            return;
        }

        try
        {
            string filePath = args[0];
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File doesn't exist: {filePath}");
                return;
            }

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);

            Console.WriteLine("File is now locked. Press any key to unlock and exit...");

            Console.ReadKey();

            stream.Dispose();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.ToString());
        }
    }
}