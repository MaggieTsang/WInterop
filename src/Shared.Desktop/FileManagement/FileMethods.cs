﻿// ------------------------
//    WInterop Framework
// ------------------------

// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Win32.SafeHandles;
using System;
using WInterop.Authentication.Types;
using WInterop.ErrorHandling;
using WInterop.ErrorHandling.Types;
using WInterop.FileManagement.BufferWrappers;
using WInterop.FileManagement.Types;
using WInterop.Support;
using WInterop.Support.Buffers;

namespace WInterop.FileManagement
{
    public static partial class FileMethods
    {
        /// <summary>
        /// Get the long (non 8.3) path version of the given path.
        /// </summary>
        public static string GetLongPathName(string path)
        {
            var wrapper = new LongPathNameWrapper { Path = path };
            return BufferHelper.ApiInvoke(ref wrapper, path);
        }

        /// <summary>
        /// Get the short (8.3) path version of the given path.
        /// </summary>
        public static string GetShortPathName(string path)
        {
            var wrapper = new ShortPathNameWrapper { Path = path };
            return BufferHelper.ApiInvoke(ref wrapper, path);
        }

        /// <summary>
        /// Gets a canonical version of the given handle's path.
        /// </summary>
        public static string GetFinalPathNameByHandle(
            SafeFileHandle fileHandle,
            GetFinalPathNameByHandleFlags flags = GetFinalPathNameByHandleFlags.FILE_NAME_NORMALIZED | GetFinalPathNameByHandleFlags.VOLUME_NAME_DOS)
        {
            var wrapper = new FinalPathNameByHandleWrapper { FileHandle = fileHandle, Flags = flags };
            return BufferHelper.ApiInvoke(ref wrapper);
        }

        /// <summary>
        /// Gets a canonical version of the given file's path.
        /// </summary>
        /// <param name="resolveLinks">True to get the destination of links rather than the link itself.</param>
        public static string GetFinalPathName(string path, GetFinalPathNameByHandleFlags finalPathFlags, bool resolveLinks)
        {
            if (path == null) return null;

            // BackupSemantics is needed to get directory handles
            FileFlags flags = FileFlags.FILE_FLAG_BACKUP_SEMANTICS;
            if (!resolveLinks) flags |= FileFlags.FILE_FLAG_OPEN_REPARSE_POINT;

            using (SafeFileHandle fileHandle = CreateFile(path, 0, ShareMode.ReadWrite,
                CreationDisposition.OpenExisting, FileAttributes.NONE, flags))
            {
                return GetFinalPathNameByHandle(fileHandle, finalPathFlags);
            }
        }

        /// <summary>
        /// Gets the file information for the given handle.
        /// </summary>
        public static BY_HANDLE_FILE_INFORMATION GetFileInformationByHandle(SafeFileHandle fileHandle)
        {
            if (!Imports.GetFileInformationByHandle(fileHandle, out BY_HANDLE_FILE_INFORMATION fileInformation))
                throw Errors.GetIoExceptionForLastError();

            return fileInformation;
        }

        /// <summary>
        /// Creates symbolic links.
        /// </summary>
        public static void CreateSymbolicLink(string symbolicLinkPath, string targetPath, bool targetIsDirectory = false)
        {
            if (!Imports.CreateSymbolicLinkW(symbolicLinkPath, targetPath,
                targetIsDirectory ? SYMBOLIC_LINK_FLAG.SYMBOLIC_LINK_FLAG_DIRECTORY : SYMBOLIC_LINK_FLAG.SYMBOLIC_LINK_FLAG_FILE))
                throw Errors.GetIoExceptionForLastError(symbolicLinkPath);
        }

        /// <summary>
        /// CreateFile wrapper. Desktop only. Prefer FileManagement.CreateFile() as it will handle all supported platforms.
        /// </summary>
        /// <remarks>Not available in Windows Store applications.</remarks>
        public static SafeFileHandle CreateFileW(
            string path,
            DesiredAccess desiredAccess,
            ShareMode shareMode,
            CreationDisposition creationDisposition,
            FileAttributes fileAttributes = FileAttributes.NONE,
            FileFlags fileFlags = FileFlags.NONE,
            SecurityQosFlags securityQosFlags = SecurityQosFlags.NONE)
        {
            uint flags = (uint)fileAttributes | (uint)fileFlags | (uint)securityQosFlags;

            unsafe
            {
                SafeFileHandle handle = Imports.CreateFileW(path, desiredAccess, shareMode, null, creationDisposition, flags, IntPtr.Zero);
                if (handle.IsInvalid)
                    throw Errors.GetIoExceptionForLastError(path);
                return handle;
            }
        }

        /// <summary>
        /// CopyFileEx wrapper. Desktop only. Prefer FileManagement.CopyFile() as it will handle all supported platforms.
        /// </summary>
        /// <param name="overwrite">Overwrite an existing file if true.</param>
        public static void CopyFileEx(string source, string destination, bool overwrite = false)
        {
            bool cancel = false;

            if (!Imports.CopyFileExW(
                lpExistingFileName: source,
                lpNewFileName: destination,
                lpProgressRoutine: null,
                lpData: IntPtr.Zero,
                pbCancel: ref cancel,
                dwCopyFlags: overwrite ? 0 : CopyFileFlags.COPY_FILE_FAIL_IF_EXISTS))
            {
                throw Errors.GetIoExceptionForLastError(source);
            }
        }

        /// <summary>
        /// Gets file attributes for the given path.
        /// </summary>
        /// <remarks>Not available in Store apps, use FileMethods.GetFileInfo instead.</remarks>
        public static FileAttributes GetFileAttributes(string path)
        {
            FileAttributes attributes = Imports.GetFileAttributesW(path);
            if (attributes == FileAttributes.INVALID_FILE_ATTRIBUTES)
                throw Errors.GetIoExceptionForLastError(path);

            return attributes;
        }

        public static string GetFileName(SafeFileHandle fileHandle)
        {
            // https://msdn.microsoft.com/en-us/library/windows/hardware/ff545817.aspx

            //  typedef struct _FILE_NAME_INFORMATION
            //  {
            //      ULONG FileNameLength;
            //      WCHAR FileName[1];
            //  } FILE_NAME_INFORMATION, *PFILE_NAME_INFORMATION;

            return GetFileInformationString(fileHandle, FILE_INFORMATION_CLASS.FileNameInformation);
        }

        public static string GetVolumeName(SafeFileHandle fileHandle)
        {
            // Same basic structure as FILE_NAME_INFORMATION
            return GetFileInformationString(fileHandle, FILE_INFORMATION_CLASS.FileVolumeNameInformation);
        }

        /// <summary>
        /// This is the short name for the file/directory name, not the path. Available from WindowsStore.
        /// </summary>
        public static string GetShortName(SafeFileHandle fileHandle)
        {
            // Same basic structure as FILE_NAME_INFORMATION
            return GetFileInformationString(fileHandle, FILE_INFORMATION_CLASS.FileAlternateNameInformation);
        }

        private unsafe static string GetFileInformationString(SafeFileHandle fileHandle, FILE_INFORMATION_CLASS fileInformationClass)
        {
            return BufferHelper.BufferInvoke((HeapBuffer buffer) =>
            {
                NTSTATUS status = NTSTATUS.STATUS_BUFFER_OVERFLOW;

                // Start with MAX_PATH
                uint byteLength = 260 * sizeof(char);

                TrailingString.SizedInBytes* value = null;

                while (status == NTSTATUS.STATUS_BUFFER_OVERFLOW)
                {
                    // Add space for the FileNameLength
                    buffer.EnsureByteCapacity(byteLength + sizeof(uint));

                    unsafe
                    {
                        status = Imports.NtQueryInformationFile(
                            FileHandle: fileHandle,
                            IoStatusBlock: out _,
                            FileInformation: buffer.VoidPointer,
                            Length: checked((uint)buffer.ByteCapacity),
                            FileInformationClass: fileInformationClass);
                    }

                    if (status == NTSTATUS.STATUS_SUCCESS || status == NTSTATUS.STATUS_BUFFER_OVERFLOW)
                    {
                        value = (TrailingString.SizedInBytes*)buffer.VoidPointer;
                        byteLength = value->SizeInBytes;
                    }
                }

                if (status != NTSTATUS.STATUS_SUCCESS)
                    throw ErrorMethods.GetIoExceptionForNTStatus(status);

                return value->Value;
            });
        }

        unsafe private static void GetFileInformation(SafeFileHandle fileHandle, FILE_INFORMATION_CLASS fileInformationClass, void* value, uint size)
        {
            NTSTATUS status = Imports.NtQueryInformationFile(
                FileHandle: fileHandle,
                IoStatusBlock: out _,
                FileInformation: value,
                Length: size,
                FileInformationClass: fileInformationClass);

            if (status != NTSTATUS.STATUS_SUCCESS)
                throw ErrorMethods.GetIoExceptionForNTStatus(status);
        }

        /// <summary>
        /// Gets the file mode for the given handle.
        /// </summary>
        public static FILE_MODE_INFORMATION GetFileMode(SafeFileHandle fileHandle)
        {
            FILE_MODE_INFORMATION info;
            unsafe
            {
                GetFileInformation(fileHandle, FILE_INFORMATION_CLASS.FileModeInformation, &info, sizeof(FILE_MODE_INFORMATION));
            }
            return info;
        }

        /// <summary>
        /// Return whether or not the given expression matches the given name. Takes standard
        /// Windows wildcards (*, ?, &lt;, &gt; &quot;).
        /// </summary>
        public unsafe static bool IsNameInExpression(string expression, string name, bool ignoreCase)
        {
            // If ignore case is set, the API will uppercase the name *if* an UpcaseTable
            // is not provided. It then flips to case-sensitive. In this state the expression
            // has to be uppercase to match as expected.

            fixed (char* e = ignoreCase ? expression.ToUpperInvariant() : expression)
            fixed (char* n = name)
            {
                UNICODE_STRING* eus = null;
                UNICODE_STRING* nus = null;

                if (e != null)
                {
                    var temp = new UNICODE_STRING(e, expression);
                    eus = &temp;
                }
                if (n != null)
                {
                    var temp = new UNICODE_STRING(n, name);
                    nus = &temp;
                }

                return Imports.RtlIsNameInExpression(eus, nus, ignoreCase, IntPtr.Zero);
            }
        }
    }
}
