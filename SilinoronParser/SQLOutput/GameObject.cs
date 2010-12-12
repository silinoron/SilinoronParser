using System;
using System.Collections.Generic;
using SilinoronParser.Enums;

namespace SilinoronParser.SQLOutput
{
    public sealed class GameObject
    {
        public GameObject()
        {
            Name = new FourStrings();
            Data = new TwentyFourInts();
            QuestItems = new SixInts();
        }

        public int Entry { get; set; }
        public GameObjectType Type { get; set; }
        public int DisplayID { get; set; }
        public FourStrings Name { get; private set; }
        public string IconName { get; set; }
        public string CastCaption { get; set; }
        public string UnkString { get; set; }
        public TwentyFourInts Data { get; private set; }
        public float Size { get; set; }
        public SixInts QuestItems { get; private set; }
        public int Unk { get; set; }

        public string ToSQL()
        {
            string sql = "REPLACE INTO gameobjectcache VALUES (";
            sql += Entry + ", ";
            sql += (int)Type + ", ";
            sql += DisplayID + ", ";
            sql += Name.ToSQL() + ", ";
            sql += "\"" + IconName + "\",";
            sql += "\"" + CastCaption + "\",";
            sql += "\"" + UnkString + "\",";
            sql += Data.ToSQL() + ", ";
            sql += Size + ", ";
            sql += QuestItems.ToSQL() + ", ";
            sql += Unk + ");";
            return sql;
        }
    }
}
