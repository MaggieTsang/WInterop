﻿// ------------------------
//    WInterop Framework
// ------------------------

// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using WInterop.ErrorHandling;
using WInterop.ErrorHandling.Types;
using Xunit;

namespace DesktopTests
{
    public partial class ErrorHandlingTests
    {
        [Fact]
        public void BasicBeep()
        {
            ErrorMethods.Beep(frequency: 262, duration: 200);
        }

        [Theory,
            InlineData(BeepType.SimpleBeep),
            InlineData(BeepType.Ok),
            InlineData(BeepType.Question),
            InlineData(BeepType.Warning),
            InlineData(BeepType.Information),
            InlineData(BeepType.Error)
            ]
        public void BasicMessageBeep(BeepType type)
        {
            ErrorMethods.MessageBeep(type);
        }

        [Fact]
        public void GetProcessErrorMode()
        {
            ErrorMode mode = ErrorMethods.GetProcessErrorMode();
        }

        [Fact]
        public void BasicThreadErrorMode()
        {
            ErrorMode mode = ErrorMethods.GetThreadErrorMode();
            ErrorMode newMode = mode ^ ErrorMode.SEM_NOOPENFILEERRORBOX;
            ErrorMode oldMode = ErrorMethods.SetThreadErrorMode(newMode);
            try
            {
                oldMode.Should().Be(mode);
                ErrorMethods.GetThreadErrorMode().Should().Be(newMode);
            }
            finally
            {
                ErrorMethods.SetThreadErrorMode(mode).Should().Be(newMode);
            }
        }
    }
}
