using System;
using System.IO;
using System.Threading;

/*
Win10 NTFS:

Created: original.txt~RF46c2c0f.TMP
Deleted: original.txt~RF46c2c0f.TMP
Renamed: original.txt~RF46c2c0f.TMP (original.txt)
Renamed: original.txt (replacement.txt)
Deleted: original.txt~RF46c2c0f.TMP

Win11 ReFS:

Created: original.txt~RF39c2f1b3.TMP
Deleted: original.txt
Changed: original.txt~RF39c2f1b3.TMP
Renamed: original.txt (replacement.txt)
Deleted: original.txt~RF39c2f1b3.TMP

*/

class Program
{
    static void Main(string[] args)
    {
        string directory = Path.GetTempPath();
        directory = Path.Combine(directory, Path.GetRandomFileName());
        Directory.CreateDirectory(directory);

        string originalFilePath = Path.Combine(directory, "original.txt");
        string replacementFilePath = Path.Combine(directory, "replacement.txt");

        File.WriteAllText(originalFilePath, "original");
        File.WriteAllText(replacementFilePath, "replacement");

        var watcher = new FileSystemWatcher(directory)
        {
            EnableRaisingEvents = true,
            IncludeSubdirectories = false
        };
        watcher.Changed += (s, e) => Process(e);
        watcher.Created += (s, e) => Process(e);
        watcher.Deleted += (s, e) => Process(e);
        watcher.Renamed += (s, e) => Process(e);

        File.Replace(replacementFilePath, originalFilePath, destinationBackupFileName: null, ignoreMetadataErrors: true);

        // give events time to arrive
        Thread.Sleep(1000);

        watcher.Dispose();

        Directory.Delete(directory, recursive: true);
    }

    private static void Process(FileSystemEventArgs e)
    {
        string renameText = "";
        if (e is RenamedEventArgs renamed)
        {
            renameText = $" ({renamed.OldName})";
        }

        Console.WriteLine($"{e.ChangeType}: {e.Name}{renameText}");
    }
}
