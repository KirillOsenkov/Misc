using System.IO;
using System.IO.Compression;

class ZipEntryViewer
{
    static void Main(string[] args)
    {
        const string filePath = @"C:\Temp\Microsoft.VisualStudio.WebTools.WebToolingAddin_7.5.mpack";

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
}
