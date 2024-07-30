using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

public static class CredentialManager
{
    public static byte[] GetCredentialBytes(string name)
    {
        if (!CredRead(name, CredentialType.Generic, 0, out IntPtr credentialPtr))
        {
            return null;
        }

        using (var handle = new CriticalCredentialHandle(credentialPtr))
        {
            var credential = handle.GetCredential();
            if (credential.CredentialBlobSize > 0)
            {
                byte[] array = new byte[credential.CredentialBlobSize];
                Marshal.Copy(credential.CredentialBlob, array, 0, array.Length);
                return array;
            }
        }

        return null;
    }

    public static string GetCredentialUnicodeString(string name)
    {
        var bytes = GetCredentialBytes(name);
        var text = bytes.GetUnicodeString();
        return text;
    }

    public static void SetCredentialUnicodeString(string name, string value)
    {
        var bytes = Encoding.Unicode.GetBytes(value);
        SetCredentialBytes(name, bytes);
    }

    public static void SetCredentialBytes(string name, byte[] byteArray)
    {
        // XP and Vista: 512;
        // 7 and above: 5*512
        if (Environment.OSVersion.Version < new Version(6, 1) /* Windows 7 */)
        {
            if (byteArray != null && byteArray.Length > 512)
            {
                throw new ArgumentOutOfRangeException("secret", "The secret message has exceeded 512 bytes.");
            }
        }
        else
        {
            if (byteArray != null && byteArray.Length > 512 * 5)
            {
                throw new ArgumentOutOfRangeException("secret", "The secret message has exceeded 2560 bytes.");
            }
        }

        CREDENTIAL credential = new CREDENTIAL();
        credential.AttributeCount = 0;
        credential.Attributes = IntPtr.Zero;
        credential.Comment = null;
        credential.TargetAlias = null;
        credential.Type = (int)CredentialType.Generic;
        credential.Persist = 2; // CredentialPersistence.LocalMachine;
        credential.CredentialBlobSize = byteArray == null ? 0 : byteArray.Length;
        credential.TargetName = name;
        credential.CredentialBlob = byteArray.WriteToNativeMemory();

        credential.UserName = Environment.UserName;

        bool written = CredWrite(ref credential, 0);
        Marshal.FreeHGlobal(credential.CredentialBlob);

        if (!written)
        {
            int lastError = Marshal.GetLastWin32Error();
            throw new Exception(string.Format("CredWrite failed with the error code {0}.", lastError));
        }
    }

    [DllImport("Advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "CredReadW", SetLastError = true)]
    internal static extern bool CredRead(string target, CredentialType type, int reservedFlag, out IntPtr CredentialPtr);

    [DllImport("Advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "CredWriteW", SetLastError = true)]
    internal static extern bool CredWrite([In] ref CREDENTIAL userCredential, [In] UInt32 flags);

    [DllImport("Advapi32.dll", SetLastError = true)]
    internal static extern bool CredFree([In] IntPtr cred);

    public static IntPtr WriteToNativeMemory(this byte[] array)
    {
        var pointer = Marshal.AllocHGlobal(array.Length);
        Marshal.Copy(array, 0, pointer, array.Length);
        return pointer;
    }

    public static string GetUnicodeString(this byte[] array)
    {
        if (array == null)
        {
            return null;
        }

        return Encoding.Unicode.GetString(array);
    }

    public static byte[] Compress(this byte[] bytes)
    {
        if (bytes == null || bytes.Length == 0)
        {
            return bytes;
        }

        using (var memoryStream = new MemoryStream(bytes))
        using (var output = new MemoryStream())
        {
            using (var gzipStream = new GZipStream(output, CompressionLevel.Optimal))
            {
                memoryStream.CopyTo(gzipStream);
            }

            return output.ToArray();
        }
    }

    public static byte[] Decompress(this byte[] bytes)
    {
        if (bytes == null || bytes.Length == 0)
        {
            return bytes;
        }

        using (var memoryStream = new MemoryStream(bytes))
        using (var output = new MemoryStream())
        using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
        {
            gzipStream.CopyTo(output);
            return output.ToArray();
        }
    }

    public static byte[] Compress(this string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return Array.Empty<byte>();
        }

        return Encoding.UTF8.GetBytes(text).Compress();
    }

    public static string DecompressToString(this byte[] bytes)
    {
        if (bytes == null)
        {
            return string.Empty;
        }

        return Encoding.UTF8.GetString(bytes.Decompress());
    }

    internal enum CredentialType : uint
    {
        None,
        Generic,
        DomainPassword,
        DomainCertificate,
        DomainVisiblePassword
    }

    internal sealed class CriticalCredentialHandle : CriticalHandleZeroOrMinusOneIsInvalid
    {
        internal CriticalCredentialHandle(IntPtr preexistingHandle)
        {
            SetHandle(preexistingHandle);
        }

        internal CREDENTIAL GetCredential()
        {
            if (!IsInvalid)
            {
                return (CREDENTIAL)Marshal.PtrToStructure(handle, typeof(CREDENTIAL));
            }

            throw new InvalidOperationException("Invalid CriticalHandle!");
        }

        protected override bool ReleaseHandle()
        {
            if (!IsInvalid)
            {
                CredFree(handle);
                SetHandleAsInvalid();
                return true;
            }

            return false;
        }
    }

    internal struct CREDENTIAL
    {
        public int Flags;

        public int Type;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string TargetName;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string Comment;

        public long LastWritten;

        public int CredentialBlobSize;

        public IntPtr CredentialBlob;

        public int Persist;

        public int AttributeCount;

        public IntPtr Attributes;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string TargetAlias;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string UserName;
    }
}
