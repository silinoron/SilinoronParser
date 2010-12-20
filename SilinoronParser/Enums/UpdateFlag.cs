using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SilinoronParser.Enums
{
    [Flags]
    enum UpdateFlag : short
    {
        None = 0x000,
        Self = 0x001,
        Transport = 0x002,
        AttackingTarget = 0x004,
        Unknown1 = 0x008,
        LowGuid = 0x010,
        Living = 0x020,
        StationaryObject = 0x040,
        Vehicle = 0x080,
        GOPosition = 0x100,
        GORotation = 0x200,
        Unknown2 = 0x400,
        Unknown3 = 0x800
    }
}
