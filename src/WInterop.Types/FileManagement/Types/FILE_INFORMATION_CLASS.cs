﻿// ------------------------
//    WInterop Framework
// ------------------------

// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace WInterop.FileManagement.Types
{
    // https://msdn.microsoft.com/en-us/library/windows/hardware/ff728840.aspx
    public enum FILE_INFORMATION_CLASS : uint
    {
        FileDirectoryInformation = 1,
        FileFullDirectoryInformation = 2,
        FileBothDirectoryInformation = 3,
        FileBasicInformation = 4,
        FileStandardInformation = 5,
        FileInternalInformation = 6,
        FileEaInformation = 7,
        FileAccessInformation = 8,
        FileNameInformation = 9,
        FileRenameInformation = 10,
        FileLinkInformation = 11,
        FileNamesInformation = 12,
        FileDispositionInformation = 13,
        FilePositionInformation = 14,
        FileFullEaInformation = 15,
        FileModeInformation = 16,
        FileAlignmentInformation = 17,
        FileAllInformation = 18,
        FileAllocationInformation = 19,
        FileEndOfFileInformation = 20,
        FileAlternateNameInformation = 21,
        FileStreamInformation = 22,
        FilePipeInformation = 23,
        FilePipeLocalInformation = 24,
        FilePipeRemoteInformation = 25,
        FileMailslotQueryInformation = 26,
        FileMailslotSetInformation = 27,
        FileCompressionInformation = 28,
        FileObjectIdInformation = 29,
        FileCompletionInformation = 30,
        FileMoveClusterInformation = 31,
        FileQuotaInformation = 32,
        FileReparsePointInformation = 33,
        FileNetworkOpenInformation = 34,
        FileAttributeTagInformation = 35,
        FileTrackingInformation = 36,
        FileIdBothDirectoryInformation = 37,
        FileIdFullDirectoryInformation = 38,
        FileValidDataLengthInformation = 39,
        /// <summary>
        /// This is used for setting, not getting the short name
        /// </summary>
        FileShortNameInformation = 40,
        FileIoCompletionNotificationInformation = 41,
        FileIoStatusBlockRangeInformation = 42,
        FileIoPriorityHintInformation = 43,
        FileSfioReserveInformation = 44,
        FileSfioVolumeInformation = 45,
        FileHardLinkInformation = 46,
        FileProcessIdsUsingFileInformation = 47,
        FileNormalizedNameInformation = 48,
        FileNetworkPhysicalNameInformation = 49,
        FileIdGlobalTxDirectoryInformation = 50,
        FileIsRemoteDeviceInformation = 51,
        FileUnusedInformation = 52,
        FileNumaNodeInformation = 53,
        FileStandardLinkInformation = 54,
        FileRemoteProtocolInformation = 55,
        // The following are available starting with Windows 10
        FileRenameInformationBypassAccessCheck = 56,
        FileLinkInformationBypassAccessCheck = 57,
        FileVolumeNameInformation = 58,
        FileIdInformation = 59,
        FileIdExtdDirectoryInformation = 60,
        FileReplaceCompletionInformation = 61, // Windows 8.1
        FileHardLinkFullIdInformation = 62,
        FileIdExtdBothDirectoryInformation = 63,
        FileMaximumInformation = 64
    }
}
