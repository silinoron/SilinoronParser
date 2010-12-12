using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SilinoronParser.SQLOutput
{
    public sealed class FourStrings
    {
        public static readonly int DATA_SIZE = 4;
        private string[] data = new string[DATA_SIZE];
        public string this[int index]
        {
            get
            {
                return data[index];
            }
            set
            {
                data[index] = value;
            }
        }

        public string ToSQL()
        {
            string sql = "";
            for (int i = 0; i < DATA_SIZE - 1; i++)
                sql += "'" + data[i].ToSQL() + "', ";
            sql += "'" + data[DATA_SIZE - 1].ToSQL() + "'";
            return sql;
        }
    }

    public sealed class FourInts
    {
        public static readonly int DATA_SIZE = 4;
        private int[] data = new int[DATA_SIZE];
        public int this[int index]
        {
            get
            {
                return data[index];
            }
            set
            {
                data[index] = value;
            }
        }

        public string ToSQL()
        {
            string sql = "";
            for (int i = 0; i < DATA_SIZE - 1; i++)
                sql += data[i] + ", ";
            sql += data[DATA_SIZE - 1];
            return sql;
        }
    }

    public sealed class SixInts
    {
        public static readonly int DATA_SIZE = 6;
        private int[] data = new int[DATA_SIZE];
        public int this[int index]
        {
            get
            {
                return data[index];
            }
            set
            {
                data[index] = value;
            }
        }

        public string ToSQL()
        {
            string sql = "";
            for (int i = 0; i < DATA_SIZE - 1; i++)
                sql += data[i] + ", ";
            sql += data[DATA_SIZE - 1];
            return sql;
        }
    }

    public sealed class FiveInts
    {
        public static readonly int DATA_SIZE = 5;
        private int[] data = new int[DATA_SIZE];
        public int this[int index]
        {
            get
            {
                return data[index];
            }
            set
            {
                data[index] = value;
            }
        }

        public string ToSQL()
        {
            string sql = "";
            for (int i = 0; i < DATA_SIZE - 1; i++)
                sql += data[i] + ", ";
            sql += data[DATA_SIZE - 1];
            return sql;
        }
    }

    public sealed class TwentyFourInts
    {
        public static readonly int DATA_SIZE = 24;
        private int[] data = new int[DATA_SIZE];
        public int this[int index]
        {
            get
            {
                return data[index];
            }
            set
            {
                data[index] = value;
            }
        }

        public string ToSQL()
        {
            string sql = "";
            for (int i = 0; i < DATA_SIZE - 1; i++)
                sql += data[i] + ", ";
            sql += data[DATA_SIZE - 1];
            return sql;
        }
    }
}
