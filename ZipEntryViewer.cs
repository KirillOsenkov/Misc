using System.IO;
using System.IO.Compression;

class ZipEntryViewer
{
    static void Main(string[] args)
    {
        const string filePath = @"C:\Temp\1.zip";

        using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        using (var zipArchive = new ZipArchive(stream, ZipArchiveMode.Read))
        {
            foreach (var entry in zipArchive.Entries)
            {
                var fullName = entry.FullName;
                if (fullName.IndexOf('\\') != -1)
                {
                    System.Console.WriteLine(fullName);
                }
                else if (fullName.IndexOf('/') != -1)
                {
                    System.Console.WriteLine(fullName);
                }
            }
        }
    }

    static void SetPermissions(string filePath, string entryFullName, int attributes = 755)
    {
        using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
        using (var zipArchive = new ZipArchive(stream, ZipArchiveMode.Read | ZipArchiveMode.Update))
        {
            foreach (var entry in zipArchive.Entries)
            {
                if (entry.FullName == entryFullName)
                {
                    entry.ExternalAttributes = 755;
                }
            }
        }
    }    
}
