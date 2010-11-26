using System;
using SilinoronParser.Enums;

namespace SilinoronParser.Parsing
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class ParserAttribute : Attribute
    {
        public ParserAttribute(ushort i)
        {
            index = i;
        }

        public ParserAttribute(Index i)
        {
            index = (ushort)i;
        }

        public int index { get; private set; }
    }
}
