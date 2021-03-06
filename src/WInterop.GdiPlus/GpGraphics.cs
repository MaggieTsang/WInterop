﻿// ------------------------
//    WInterop Framework
// ------------------------

// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using WInterop.Gdi.Types;
using WInterop.Handles.Types;

namespace WInterop.GdiPlus
{
    public class GpGraphics : HandleZeroOrMinusOneIsInvalid
    {
        public GpGraphics()
            : base(ownsHandle: true) { }

        protected override bool ReleaseHandle()
        {
            return GdiPlusMethods.Imports.GdipDeleteGraphics(handle) == GpStatus.Ok;
        }
    }
}
