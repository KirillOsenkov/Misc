using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace ResourceExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            var dll = @"C:\ide\bin\debug\Microsoft.VisualStudio.ImageCatalog.dll";
            var outputPath = @"C:\temp\png";
            var name = AssemblyName.GetAssemblyName(dll);
            var assembly = Assembly.Load(name);
            var manifestResourceNames = assembly.GetManifestResourceNames();
            var first = manifestResourceNames.First();
            var stream = assembly.GetManifestResourceStream(first);
            ResourceSet set = new ResourceSet(stream);
            foreach (DictionaryEntry obj in set)
            {
                var key = obj.Key as string;
                key = key.Replace('/', '\\');
                key = key.Replace(".16.16", "");
                var value = obj.Value as UnmanagedMemoryStream;
                var filePath = Path.Combine(outputPath, key);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                using (var writer = new FileStream(filePath, FileMode.Create))
                {
                    value.CopyTo(writer);
                }
            }
        }
    }
}
