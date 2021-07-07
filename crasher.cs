using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using Microsoft.Win32;

class Program
{
    static void Main(string[] args)
    {
        Log("Current directory: " + Environment.CurrentDirectory);
        Log("IsAdmin: " + IsAdmin());

        using var registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\Windows Error Reporting\LocalDumps");
        DumpRegistryKey(registryKey);

        using var aeDebugKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\AeDebug");
        DumpRegistryKey(aeDebugKey);

        M();
    }

    public static void DumpRegistryKey(RegistryKey key)
    {
        Log(key.Name);

        foreach (var valueName in key.GetValueNames())
        {
            var value = key.GetValue(valueName);
            Log($"    {valueName}={value}");
        }

        foreach (var subkeyName in key.GetSubKeyNames())
        {
            using var subkey = key.OpenSubKey(subkeyName);
            DumpRegistryKey(subkey);
        }
    }

    static bool IsAdmin()
    {
        using var identity = WindowsIdentity.GetCurrent();
        WindowsPrincipal principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    private static void M()
    {
        M();
    }

    static void Log(object message)
    {
        Console.WriteLine($"{message}");
        //var path = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "crasherLog.txt");
        //File.AppendAllText(path, Convert.ToString(message) + Environment.NewLine);
    }
}
