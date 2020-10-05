using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

class ListCred
{
    static void Main(string[] args)
    {
        string credentialName = "";
        if (string.IsNullOrEmpty(credentialName))
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: listcred <credname>");
                return;
            }

            credentialName = args[0];
        }

        var value = GuiLabs.CredentialManager.GetCredentialValue(credentialName);
        Console.WriteLine(value);
    }
}


namespace GuiLabs
{
    /// <summary>
    /// Adapted from https://archive.codeplex.com/?p=credentialmanagement, Apache 2.0
    /// </summary>
    public class CredentialManager
    {
        public static string GetCredentialValue(string name)
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
                    var password = Marshal.PtrToStringUni(credential.CredentialBlob, credential.CredentialBlobSize / 2);
                    return password;
                }
            }

            return null;
        }

        [DllImport("Advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "CredReadW", SetLastError = true)]
        internal static extern bool CredRead(string target, CredentialType type, int reservedFlag, out IntPtr CredentialPtr);

        [DllImport("Advapi32.dll", SetLastError = true)]
        internal static extern bool CredFree([In] IntPtr cred);

        public enum CredentialType : uint
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
}