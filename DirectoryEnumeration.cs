using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

class Program
{
    static void Main(string[] args)
    {
        var program = new Program();
        program.EnumerateDirectory(@"C:\temp\2");
    }

    unsafe void EnumerateDirectory(string directory)
    {
        var _directoryHandle = CreateDirectoryHandle(directory);
        if (_directoryHandle == IntPtr.Zero || _directoryHandle == (IntPtr)(-1))
        {
            return;
        }

        const int _bufferLength = 4096;
        var _buffer = Marshal.AllocHGlobal(_bufferLength);

        FILE_FULL_DIR_INFORMATION* _entry = null;

        do
        {
            _entry = FILE_FULL_DIR_INFORMATION.GetNextInfo(_entry);
            if (_entry == null)
            {
                if (GetData(_directoryHandle, _buffer, _bufferLength))
                {
                    _entry = (FILE_FULL_DIR_INFORMATION*)_buffer;
                }
                else
                {
                    break;
                }
            }

            var attributes = _entry->FileAttributes;
            string fileName = _entry->FileName;
            bool isDirectory = (attributes & FileAttributes.Directory) != 0;
            if (fileName == "." || fileName == "..")
            {
                continue;
            }

        } while (true);

        Marshal.FreeHGlobal(_buffer);

        CloseHandle(_directoryHandle);
    }

    internal enum BOOLEAN : byte
    {
        FALSE,
        TRUE
    }

    internal struct UNICODE_STRING
    {
        /// <summary>
        /// Length in bytes, not including the null terminator, if any.
        /// </summary>
        internal ushort Length;

        /// <summary>
        /// Max size of the buffer in bytes
        /// </summary>
        internal ushort MaximumLength;

        internal IntPtr Buffer;
    }

    [DllImport("ntdll.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    internal unsafe static extern int NtQueryDirectoryFile(
        IntPtr FileHandle,
        IntPtr Event,
        IntPtr ApcRoutine,
        IntPtr ApcContext,
        IO_STATUS_BLOCK* IoStatusBlock,
        IntPtr FileInformation,
        uint Length,
        FILE_INFORMATION_CLASS FileInformationClass,
        BOOLEAN ReturnSingleEntry,
        UNICODE_STRING* FileName,
        BOOLEAN RestartScan);

    internal const uint STATUS_NO_MORE_FILES = 2147483654u;
    internal const uint STATUS_SUCCESS = 0u;
    internal const uint STATUS_FILE_NOT_FOUND = 3221225487u;

    /// <summary>
    /// Fills the buffer with the next set of data.
    /// </summary>
    /// <returns>'true' if new data was found</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe bool GetData(IntPtr directoryHandle, IntPtr buffer, int bufferLength)
    {
        IO_STATUS_BLOCK statusBlock;
        int status = NtQueryDirectoryFile(
            FileHandle: directoryHandle,
            Event: IntPtr.Zero,
            ApcRoutine: IntPtr.Zero,
            ApcContext: IntPtr.Zero,
            IoStatusBlock: &statusBlock,
            FileInformation: buffer,
            Length: (uint)bufferLength,
            FileInformationClass: FILE_INFORMATION_CLASS.FileFullDirectoryInformation,
            ReturnSingleEntry: BOOLEAN.FALSE,
            FileName: null,
            RestartScan: BOOLEAN.FALSE);

        switch ((uint)status)
        {
            case STATUS_NO_MORE_FILES:
                return false;
            case STATUS_SUCCESS:
                return true;
            // FILE_NOT_FOUND can occur when there are NO files in a volume root (usually there are hidden system files).
            case STATUS_FILE_NOT_FOUND:
                return false;
            default:
                return false;
        }
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern bool CloseHandle(IntPtr handle);

    /// <summary>
    /// Simple wrapper to allow creating a file handle for an existing directory.
    /// </summary>
    private IntPtr CreateDirectoryHandle(string path, bool ignoreNotFound = false)
    {
        IntPtr handle = CreateFile_IntPtr(
            path,
            1, // FILE_LIST_DIRECTORY
            FileShare.ReadWrite | FileShare.Delete,
            FileMode.Open,
            33554432); // FILE_FLAG_BACKUP_SEMANTICS

        if (handle == IntPtr.Zero || handle == (IntPtr)(-1))
        {
            return IntPtr.Zero;
        }

        return handle;
    }

    [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "CreateFileW", ExactSpelling = true, SetLastError = true)]
    private unsafe static extern IntPtr CreateFilePrivate_IntPtr(string lpFileName, int dwDesiredAccess, FileShare dwShareMode, SECURITY_ATTRIBUTES* lpSecurityAttributes, FileMode dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);

    internal struct SECURITY_ATTRIBUTES
    {
        internal uint nLength;

        internal IntPtr lpSecurityDescriptor;

        internal BOOL bInheritHandle;
    }

    internal enum BOOL
    {
        FALSE,
        TRUE
    }

    internal const int ERROR_FILE_NOT_FOUND = 2;
    internal const int ERROR_PATH_NOT_FOUND = 3;
    internal const int ERROR_ACCESS_DENIED = 5;
    internal const int ERROR_DIRECTORY = 267;

    internal unsafe static IntPtr CreateFile_IntPtr(string lpFileName, int dwDesiredAccess, FileShare dwShareMode, FileMode dwCreationDisposition, int dwFlagsAndAttributes)
    {
        lpFileName = EnsureExtendedPrefixIfNeeded(lpFileName);
        return CreateFilePrivate_IntPtr(lpFileName, dwDesiredAccess, dwShareMode, null, dwCreationDisposition, dwFlagsAndAttributes, IntPtr.Zero);
    }

    internal static string EnsureExtendedPrefixIfNeeded(string path)
    {
        if (path != null && (path.Length >= 260 || EndsWithPeriodOrSpace(path)))
        {
            return EnsureExtendedPrefix(path);
        }
        return path;
    }

    internal static string EnsureExtendedPrefix(string path)
    {
        if (IsPartiallyQualified(path) || IsDevice(path))
        {
            return path;
        }
        if (path.StartsWith("\\\\", StringComparison.OrdinalIgnoreCase))
        {
            return path.Insert(2, "?\\UNC\\");
        }
        return "\\\\?\\" + path;
    }

    internal unsafe static bool IsDevice(string path)
    {
        if (!IsExtended(path))
        {
            if (path.Length >= 4 && IsDirectorySeparator(path[0]) && IsDirectorySeparator(path[1]) && (path[2] == 46 || path[2] == 63))
            {
                return IsDirectorySeparator(path[3]);
            }
            return false;
        }
        return true;
    }

    internal unsafe static bool IsExtended(string path)
    {
        if (path.Length >= 4 && path[0] == 92 && (path[1] == 92 || path[1] == 63) && path[2] == 63)
        {
            return path[3] == 92;
        }
        return false;
    }

    internal unsafe static bool IsPartiallyQualified(string path)
    {
        if (path.Length < 2)
        {
            return true;
        }
        if (IsDirectorySeparator(path[0]))
        {
            if (path[1] != 63)
            {
                return !IsDirectorySeparator(path[1]);
            }
            return false;
        }
        if (path.Length >= 3 && path[1] == 58 && IsDirectorySeparator(path[2]))
        {
            return !IsValidDriveChar(path[0]);
        }
        return true;
    }

    internal static bool IsValidDriveChar(char value)
    {
        if (value < 'A' || value > 'Z')
        {
            if (value >= 'a')
            {
                return value <= 'z';
            }
            return false;
        }
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsDirectorySeparator(char c)
    {
        if (c != '\\')
        {
            return c == '/';
        }
        return true;
    }

    internal static bool EndsWithPeriodOrSpace(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }
        char c = path[path.Length - 1];
        if (c != ' ')
        {
            return c == '.';
        }
        return true;
    }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct FILE_FULL_DIR_INFORMATION
{
    /// <summary>
    /// Offset in bytes of the next entry, if any.
    /// </summary>
    public uint NextEntryOffset;

    /// <summary>
    /// Byte offset within the parent directory, undefined for NTFS.
    /// </summary>
    public uint FileIndex;

    public LongFileTime CreationTime;

    public LongFileTime LastAccessTime;

    public LongFileTime LastWriteTime;

    public LongFileTime ChangeTime;

    public long EndOfFile;

    public long AllocationSize;

    /// <summary>
    /// File attributes.
    /// </summary>
    /// <remarks>
    /// Note that MSDN documentation isn't correct for this- it can return
    /// any FILE_ATTRIBUTE that is currently set on the file, not just the
    /// ones documented.
    /// </remarks>
    public FileAttributes FileAttributes;

    /// <summary>
    /// The length of the file name in bytes (without null).
    /// </summary>
    public uint FileNameLength;

    /// <summary>
    /// The extended attribute size OR the reparse tag if a reparse point.
    /// </summary>
    public uint EaSize;

    private char _fileName;

    public unsafe string FileName
    {
        get
        {
            fixed (char* ptr = &_fileName)
            {
                return new string((char*)ptr, 0, (int)FileNameLength / 2);
            }
        }
    }

    /// <summary>
    /// Gets the next info pointer or null if there are no more.
    /// </summary>
    public unsafe static FILE_FULL_DIR_INFORMATION* GetNextInfo(FILE_FULL_DIR_INFORMATION* info)
    {
        if (info == null)
        {
            return null;
        }
        uint nextEntryOffset = info->NextEntryOffset;
        if (nextEntryOffset == 0)
        {
            return null;
        }
        return (FILE_FULL_DIR_INFORMATION*)((byte*)info + nextEntryOffset);
    }
}

internal struct LongFileTime
{
    /// <summary>
    /// 100-nanosecond intervals (ticks) since January 1, 1601 (UTC).
    /// </summary>
    internal long TicksSince1601;

    internal DateTimeOffset ToDateTimeOffset()
    {
        return new DateTimeOffset(DateTime.FromFileTimeUtc(TicksSince1601));
    }
}

internal struct IO_STATUS_BLOCK
{
    [StructLayout(LayoutKind.Explicit)]
    public struct IO_STATUS
    {
        /// <summary>
        /// The completion status, either STATUS_SUCCESS if the operation was completed successfully or
        /// some other informational, warning, or error status.
        /// </summary>
        [FieldOffset(0)]
        public uint Status;

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        [FieldOffset(0)]
        public IntPtr Pointer;
    }

    /// <summary>
    /// Status
    /// </summary>
    public IO_STATUS Status;

    /// <summary>
    /// Request dependent value.
    /// </summary>
    public IntPtr Information;
}

internal enum FILE_INFORMATION_CLASS : uint
{
    FileDirectoryInformation = 1u,
    FileFullDirectoryInformation,
    FileBothDirectoryInformation,
    FileBasicInformation,
    FileStandardInformation,
    FileInternalInformation,
    FileEaInformation,
    FileAccessInformation,
    FileNameInformation,
    FileRenameInformation,
    FileLinkInformation,
    FileNamesInformation,
    FileDispositionInformation,
    FilePositionInformation,
    FileFullEaInformation,
    FileModeInformation,
    FileAlignmentInformation,
    FileAllInformation,
    FileAllocationInformation,
    FileEndOfFileInformation,
    FileAlternateNameInformation,
    FileStreamInformation,
    FilePipeInformation,
    FilePipeLocalInformation,
    FilePipeRemoteInformation,
    FileMailslotQueryInformation,
    FileMailslotSetInformation,
    FileCompressionInformation,
    FileObjectIdInformation,
    FileCompletionInformation,
    FileMoveClusterInformation,
    FileQuotaInformation,
    FileReparsePointInformation,
    FileNetworkOpenInformation,
    FileAttributeTagInformation,
    FileTrackingInformation,
    FileIdBothDirectoryInformation,
    FileIdFullDirectoryInformation,
    FileValidDataLengthInformation,
    FileShortNameInformation,
    FileIoCompletionNotificationInformation,
    FileIoStatusBlockRangeInformation,
    FileIoPriorityHintInformation,
    FileSfioReserveInformation,
    FileSfioVolumeInformation,
    FileHardLinkInformation,
    FileProcessIdsUsingFileInformation,
    FileNormalizedNameInformation,
    FileNetworkPhysicalNameInformation,
    FileIdGlobalTxDirectoryInformation,
    FileIsRemoteDeviceInformation,
    FileUnusedInformation,
    FileNumaNodeInformation,
    FileStandardLinkInformation,
    FileRemoteProtocolInformation,
    FileRenameInformationBypassAccessCheck,
    FileLinkInformationBypassAccessCheck,
    FileVolumeNameInformation,
    FileIdInformation,
    FileIdExtdDirectoryInformation,
    FileReplaceCompletionInformation,
    FileHardLinkFullIdInformation,
    FileIdExtdBothDirectoryInformation,
    FileDispositionInformationEx,
    FileRenameInformationEx,
    FileRenameInformationExBypassAccessCheck,
    FileDesiredStorageClassInformation,
    FileStatInformation
}
