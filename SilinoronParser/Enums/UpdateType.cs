using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SilinoronParser.Enums
{
    public enum UpdateType : byte
    {
        Values = 0,
        Movement = 1,
        CreateObject1 = 2,
        CreateObject2 = 3,
        FarObjects = 4,
        NearObjects = 5
    }
}
