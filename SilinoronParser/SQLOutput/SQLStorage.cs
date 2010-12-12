using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SilinoronParser.SQLOutput
{
    public abstract class SQLStorage<T>
    {
        public abstract void Add(T entry);
        public abstract void Output(string toFile);
    }
}
