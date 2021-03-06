﻿// ------------------------
//    WInterop Framework
// ------------------------

// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using WInterop.Support.Buffers;

namespace WInterop.FileManagement.BufferWrappers
{
    public struct FullPathNameWrapper : IBufferFunc<StringBuffer, uint>
    {
        public string Path;

        uint IBufferFunc<StringBuffer, uint>.Func(StringBuffer buffer)
        {
            return FileMethods.Imports.GetFullPathNameW(Path, buffer.CharCapacity, buffer, IntPtr.Zero);
        }
    }
}
