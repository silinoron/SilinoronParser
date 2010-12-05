using System;
using SilinoronParser.Enums;

namespace SilinoronParser.Parsing
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class ClientToServerParserAttribute : Attribute
    {
        public ClientToServerParserAttribute(Opcode opcode)
        {
            Opcode = opcode;
        }

        public Opcode Opcode { get; private set; }
    }
}
