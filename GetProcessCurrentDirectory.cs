using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;

class GetProcessParameters
{
    static void Main(string[] args)
    {
        var dir = GetCurrentDirectory(19948, out bool is32Bit);
    }

    public static string GetCurrentDirectory(int processId, out bool is32bit)
    {
        is32bit = false;

        try
        {
            return GetProcessParametersString(processId, PEB_OFFSET.CurrentDirectory, out is32bit);
        }
        catch
        {
            return null;
        }
    }

    private static string GetProcessParametersString(int processId, PEB_OFFSET Offset, out bool is32Bit)
    {
        is32Bit = false;

        var handle = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, inherit: false, processId);
        if (handle == IntPtr.Zero)
        {
            return null;
        }

        bool IsTargetWow64Process = GetProcessIsWow64(handle);
        bool IsTarget64BitProcess = Is64BitOperatingSystem && !IsTargetWow64Process;
        is32Bit = !IsTarget64BitProcess;

        long offset;
        long processParametersOffset = IsTarget64BitProcess ? 0x20 : 0x10;
        switch (Offset)
        {
            case PEB_OFFSET.CurrentDirectory:
                offset = IsTarget64BitProcess ? 0x38 : 0x24;
                break;
            case PEB_OFFSET.CommandLine:
            default:
                return null;
        }

        try
        {
            long pebAddress = 0;
            if (IsTargetWow64Process) // OS: 64Bit, Current: 32 or 64, Target: 32bit
            {
                IntPtr peb32 = new IntPtr();

                int hr = NtQueryInformationProcess(handle, (int)PROCESSINFOCLASS.ProcessWow64Information, ref peb32, IntPtr.Size, IntPtr.Zero);
                if (hr != 0)
                {
                    return null;
                }

                pebAddress = peb32.ToInt64();

                IntPtr pp = new IntPtr();
                if (!ReadProcessMemory(handle, new IntPtr(pebAddress + processParametersOffset), ref pp, new IntPtr(Marshal.SizeOf(pp)), IntPtr.Zero))
                {
                    return null;
                }

                UNICODE_STRING_32 us = new UNICODE_STRING_32();
                if (!ReadProcessMemory(handle, new IntPtr(pp.ToInt64() + offset), ref us, new IntPtr(Marshal.SizeOf(us)), IntPtr.Zero))
                {
                    return null;
                }

                if ((us.Buffer == 0) || (us.Length == 0))
                {
                    return null;
                }

                string s = new string('\0', us.Length / 2);
                if (!ReadProcessMemory(handle, new IntPtr(us.Buffer), s, new IntPtr(us.Length), IntPtr.Zero))
                {
                    return null;
                }

                return s;
            }
            else if (IsCurrentProcessWOW64) // OS: 64Bit, Current: 32, Target: 64
            {
                // NtWow64QueryInformationProcess64 is an "undocumented API", see issue 1702
                return null;
#if false
                    PROCESS_BASIC_INFORMATION_WOW64 pbi = new PROCESS_BASIC_INFORMATION_WOW64();
                    int hr = NtWow64QueryInformationProcess64(handle, (int)PROCESSINFOCLASS.ProcessBasicInformation, ref pbi, Marshal.SizeOf(pbi), IntPtr.Zero);
                    if (hr != 0)
                    {
                        return null;
                    }

                    pebAddress = pbi.PebBaseAddress;

                    long pp = 0;
                    hr = NtWow64ReadVirtualMemory64(handle, pebAddress + processParametersOffset, ref pp, Marshal.SizeOf(pp), IntPtr.Zero);
                    if (hr != 0)
                    {
                        return null;
                    }

                    UNICODE_STRING_WOW64 us = new UNICODE_STRING_WOW64();
                    hr = NtWow64ReadVirtualMemory64(handle, pp + offset, ref us, Marshal.SizeOf(us), IntPtr.Zero);
                    if (hr != 0)
                    {
                        return null;
                    }

                    if ((us.Buffer == 0) || (us.Length == 0))
                    {
                        return null;
                    }

                    string s = new string('\0', us.Length / 2);
                    hr = NtWow64ReadVirtualMemory64(handle, us.Buffer, s, us.Length, IntPtr.Zero);
                    if (hr != 0)
                    {
                        return null;
                    }

                    return s;
#endif
            }
            else // OS, Current, Target: 64 or 32
            {
                PROCESS_BASIC_INFORMATION pbi = new PROCESS_BASIC_INFORMATION();
                int hr = NtQueryInformationProcess(handle, (int)PROCESSINFOCLASS.ProcessBasicInformation, ref pbi, Marshal.SizeOf(pbi), IntPtr.Zero);
                if (hr != 0)
                {
                    return null;
                }

                pebAddress = pbi.PebBaseAddress.ToInt64();

                IntPtr pp = new IntPtr();
                if (!ReadProcessMemory(handle, new IntPtr(pebAddress + processParametersOffset), ref pp, new IntPtr(Marshal.SizeOf(pp)), IntPtr.Zero))
                {
                    return null;
                }

                UNICODE_STRING us = new UNICODE_STRING();
                if (!ReadProcessMemory(handle, new IntPtr((long)pp + offset), ref us, new IntPtr(Marshal.SizeOf(us)), IntPtr.Zero))
                {
                    return null;
                }

                if (us.Buffer == IntPtr.Zero || us.Length == 0)
                {
                    return null;
                }

                string s = new string('\0', us.Length / 2);
                if (!ReadProcessMemory(handle, us.Buffer, s, new IntPtr(us.Length), IntPtr.Zero))
                {
                    return null;
                }

                return s;
            }
        }
        finally
        {
            CloseHandle(handle);
        }
    }

    [Flags]
    public enum PEB_OFFSET
    {
        CurrentDirectory,
        //DllPath,
        //ImagePathName,
        CommandLine,
        //WindowTitle,
        //DesktopInfo,
        //ShellInfo,
        //RuntimeData,
        //TypeMask = 0xffff,
        //Wow64 = 0x10000,
    };

    [StructLayout(LayoutKind.Sequential)]
    private struct UNICODE_STRING_32
    {
        public short Length;
        public short MaximumLength;
        public int Buffer;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct UNICODE_STRING
    {
        public short Length;
        public short MaximumLength;
        public IntPtr Buffer;
    }

    public enum PROCESSINFOCLASS : int
    {
        ProcessBasicInformation = 0, // 0, q: PROCESS_BASIC_INFORMATION, PROCESS_EXTENDED_BASIC_INFORMATION
        ProcessWow64Information = 26, // q: ULONG_PTR
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct PROCESS_BASIC_INFORMATION
    {
        public IntPtr Reserved1;
        public IntPtr PebBaseAddress;
        public IntPtr Reserved2_0;
        public IntPtr Reserved2_1;
        public IntPtr UniqueProcessId;
        public IntPtr Reserved3;
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [ResourceExposure(ResourceScope.None)]
    public static extern IntPtr OpenProcess(int access, bool inherit, int processId);

    [DllImport("kernel32.dll")]
    private static extern bool CloseHandle(IntPtr hObject);

    [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWow64Process([In] IntPtr processHandle, [Out, MarshalAs(UnmanagedType.Bool)] out bool wow64Process);

    public const int PROCESS_QUERY_INFORMATION = 0x0400;
    private const int PROCESS_VM_READ = 0x10;

    public static bool GetProcessIsWow64(IntPtr hProcess)
    {
        if ((OSVersion.Major == 5 && OSVersion.Minor >= 1) || OSVersion.Major >= 6)
        {
            bool retVal;
            if (!IsWow64Process(hProcess, out retVal))
            {
                return false;
            }

            return retVal;
        }
        else
        {
            return false;
        }
    }

    public static readonly Version OSVersion = Environment.OSVersion.Version;
    public static readonly bool Is64BitOperatingSystem = Environment.Is64BitOperatingSystem;
    public static readonly bool IsCurrentProcessWOW64 = Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess;

    [DllImport("ntdll.dll")]
    private static extern int NtQueryInformationProcess(IntPtr ProcessHandle, int ProcessInformationClass, ref PROCESS_BASIC_INFORMATION ProcessInformation, int ProcessInformationLength, IntPtr ReturnLength);

    [DllImport("ntdll.dll")]
    private static extern int NtQueryInformationProcess(IntPtr ProcessHandle, int ProcessInformationClass, ref IntPtr ProcessInformation, int ProcessInformationLength, IntPtr ReturnLength);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref IntPtr lpBuffer, IntPtr dwSize, IntPtr lpNumberOfBytesRead);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref UNICODE_STRING lpBuffer, IntPtr dwSize, IntPtr lpNumberOfBytesRead);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref UNICODE_STRING_32 lpBuffer, IntPtr dwSize, IntPtr lpNumberOfBytesRead);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [MarshalAs(UnmanagedType.LPWStr)] string lpBuffer, IntPtr dwSize, IntPtr lpNumberOfBytesRead);



}