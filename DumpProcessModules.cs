#nullable disable

using System;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        foreach (var process in Process.GetProcesses())
        {
            try
            {
                if (!process.MainModule.FileName.EndsWith("iisexpress.exe", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                Console.WriteLine(process.MainModule.FileName);
                foreach (ProcessModule module in process.Modules)
                {
                    Console.WriteLine($"  {module.FileName}");
                }
            }
            catch
            {
            }
        }
    }
}