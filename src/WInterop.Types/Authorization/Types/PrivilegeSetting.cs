﻿// ------------------------
//    WInterop Framework
// ------------------------

// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;

namespace WInterop.Authorization.Types
{
    [DebuggerDisplay("{Privilege} : {Attributes}")]
    public struct PrivilegeSetting
    {
        public PrivilegeSetting(string privilege, PrivilegeAttributes attributes)
        {
            if (!Enum.TryParse(privilege, out Privilege))
                Privilege = Privileges.UnknownPrivilege;

            Attributes = attributes;
        }

        public Privileges Privilege;
        public PrivilegeAttributes Attributes;
    }
}
