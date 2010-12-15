using System;
using SilinoronParser.Enums;

namespace SilinoronParser.SQLOutput
{
    public sealed class Creature
    {
        public Creature()
        {
            Name = new FourStrings();
            DisplayIDs = new FourInts();
            QuestItems = new SixInts();
        }

        public int Entry { get; set; }
        public FourStrings Name { get; private set; }
        public string SubName { get; set; }
        public string IconName { get; set; }
        public CreatureTypeFlag TypeFlags { get; set; }
        public CreatureType Type { get; set; }
        public CreatureFamily Family { get; set; }
        public CreatureRank Rank { get; set; }
        public int KillCredit1 { get; set; }
        public int KillCredit2 { get; set; }
        public FourInts DisplayIDs { get; private set; }
        public float HealthModifier { get; set; }
        public float ManaModifier { get; set; }
        public bool RacialLeader { get; set; }
        public SixInts QuestItems { get; private set; }
        public int MovementID { get; set; }
        public int Exp { get; set; }

        public string ToSQL()
        {
            string sql = "REPLACE INTO creaturecache VALUES (";
            sql += Entry + ", ";
            sql += Name.ToSQL() + ", ";
            sql += "'" + SubName.ToSQL() + "',";
            sql += "'" + IconName.ToSQL() + "',";
            sql += (int)TypeFlags + ", ";
            sql += (int)Type + ", ";
            sql += (int)Family + ", ";
            sql += (int)Rank + ", ";
            sql += KillCredit1 + ", ";
            sql += KillCredit2 + ", ";
            sql += DisplayIDs.ToSQL() + ", ";
            sql += HealthModifier + ", ";
            sql += ManaModifier + ", ";
            sql += (RacialLeader ? 1 : 0) + ", ";
            sql += QuestItems.ToSQL() + ", ";
            sql += MovementID + ", ";
            sql += Exp + ");";
            return sql;
        }
    }
}
