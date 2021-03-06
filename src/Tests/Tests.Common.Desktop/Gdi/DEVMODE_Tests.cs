﻿// ------------------------
//    WInterop Framework
// ------------------------

// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using System.Runtime.InteropServices;
using WInterop.Gdi.Types;
using Xunit;

namespace DesktopTests.Gdi
{
    public class DEVMODE_Tests
    {
        [Fact]
        public unsafe void ValidateDeviceModeSize()
        {
            // Make sure we're both blittable and the correct size
            GCHandle.Alloc(new DEVMODE(), GCHandleType.Pinned).Free();
            sizeof(DEVMODE).Should().Be(220);
        }

        [Fact]
        public unsafe void DeviceName()
        {
            DEVMODE devMode = new DEVMODE();
            devMode.dmDeviceName.Value.Should().Be(string.Empty);
            devMode.dmDeviceName.Value = "Foo";
            devMode.dmDeviceName.Value.Should().Be("Foo");
            string tooLong = new string('a', 40);
            devMode.dmDeviceName.Value = tooLong;
            devMode.dmDeviceName.Value.Should().Be(new string('a', 31));

            // The next value in the struct- ensuring we didn't write over
            devMode.dmSpecVersion.Should().Be(0);
        }

        [Fact]
        public unsafe void FormName()
        {
            DEVMODE devMode = new DEVMODE();
            devMode.dmFormName.Value.Should().Be(string.Empty);
            devMode.dmFormName.Value = "Bar";
            devMode.dmFormName.Value.Should().Be("Bar");
            string tooLong = new string('z', 40);
            devMode.dmFormName.Value = tooLong;
            devMode.dmFormName.Value.Should().Be(new string('z', 31));

            // The next value in the struct- ensuring we didn't write over
            devMode.dmLogPixels.Should().Be(0);
        }
    }
}
