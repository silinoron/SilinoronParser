using System;
using System.Collections.Generic;
using SilinoronParser.Util;

namespace SilinoronParser.Loading
{
    public abstract class Loader
    {
        public readonly string FileToParse;

        protected Loader(string file)
        {
            FileToParse = file;
        }

        public abstract IEnumerable<Packet> ParseFile();
    }
}
