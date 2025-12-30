#nullable disable

using System;
using System.Diagnostics;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        foreach (var process in Process.GetProcesses())
        {
            try
            {
                foreach (ProcessModule module in process.Modules)
                {
                    var name = Path.GetFileNameWithoutExtension(module.FileName);
                    if (name.Equals("msshrtmi", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine(process.MainModule.FileName);
                        Console.WriteLine($"  {module.FileName}");
                    }
                }
            }
            catch
            {
            }
        }
    }
}