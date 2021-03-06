﻿// ------------------------
//    WInterop Framework
// ------------------------

// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using WInterop.Support.Buffers;

namespace WInterop.DirectoryManagement.BufferWrappers
{
    public struct TempPathWrapper : IBufferFunc<StringBuffer, uint>
    {
        uint IBufferFunc<StringBuffer, uint>.Func(StringBuffer buffer)
        {
            return DirectoryMethods.Imports.GetCurrentDirectoryW(buffer.CharCapacity, buffer);
        }
    }
}
