using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SilinoronParser.Enums
{
    public enum ObjectType : byte
    {
        Object = 0,
        Item = 1,
        Container = 2,
        Unit = 3,
        Player = 4,
        GameObject = 5,
        DynamicObject = 6,
        Corpse = 7
    }
}
