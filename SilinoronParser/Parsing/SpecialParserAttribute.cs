using System;

namespace SilinoronParser.Parsing
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class SpecialParserAttribute : Attribute
    {
        public SpecialParserAttribute(ushort index, string name)
        {
            Index = index;
            Name = name;
        }

        public int Index { get; private set; }
        public string Name { get; private set; }
    }
}
