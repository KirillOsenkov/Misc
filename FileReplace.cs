using System;
using System.IO;
using System.Threading;

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
