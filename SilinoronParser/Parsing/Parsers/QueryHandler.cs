using System;
using SilinoronParser.Util;
using SilinoronParser.Enums;

namespace SilinoronParser.Parsing.Parsers
{
    public static class QueryHandler
    {
        [Parser(Index.HandleGuildQueryResponseIndex)]
        public static void HandleGuildQueryResponse(Packet packet)
        {
            packet.ReadInt64("Guild ID");
            packet.ReadCString("Name");
            for (byte rank = 0; rank < 10; rank++)
                packet.ReadCString("Rank " + rank + " name");

            for (byte rank = 0; rank < 10; rank++)
                packet.ReadInt32("Rank " + rank + " real rank ID");

            // ignore these as they seem to always be identical to the above loop
            for (byte rank = 0; rank < 10; rank++)
                packet.ReadInt32();

            packet.ReadInt32("Emblem style");
            packet.ReadInt32("Emblem color");
            packet.ReadInt32("Emblem border style");
            packet.ReadInt32("Emblem border color");
            packet.ReadInt32("Emblem background color");
            packet.ReadInt32("Unk int from WotLK");
        }

        [Parser(Index.HandleNameQueryResponseIndex)]
        public static void HandleNameQueryResponse(Packet packet)
        {
            packet.ReadPackedGuid("GUID");

            var end = packet.ReadBoolean("Name Found");

            if (end)
                return;

            packet.ReadCString("Name");
            packet.ReadCString("Realm Name");

            var race = (Race)packet.ReadByte();
            Console.WriteLine("Race: " + race);

            var gender = (Gender)packet.ReadByte();
            Console.WriteLine("Gender: " + gender);

            var cClass = (Class)packet.ReadByte();
            Console.WriteLine("Class: " + cClass);

            var decline = packet.ReadBoolean("Name Declined");

            if (!decline)
                return;

            for (var i = 0; i < 5; i++)
                packet.ReadCString("Declined Name " + i);
        }

        [Parser(Index.HandleCreatureQueryResponseIndex)]
        public static void HandleCreatureQueryResponse(Packet packet)
        {
            var entry = packet.ReadEntryKey("Entry");

            if (entry.Value)
                return;

            for (var i = 0; i < 4; i++)
                packet.ReadCString("Name " + i);

            packet.ReadCString("Sub Name");
            packet.ReadCString("Icon Name");

            var typeFlags = (CreatureTypeFlag)packet.ReadInt32();
            Console.WriteLine("Type Flags: " + typeFlags);

            var type = (CreatureType)packet.ReadInt32();
            Console.WriteLine("Type: " + type);

            var family = (CreatureFamily)packet.ReadInt32();
            Console.WriteLine("Family: " + family);

            var rank = (CreatureRank)packet.ReadInt32();
            Console.WriteLine("Rank: " + rank);

            for (var i = 0; i < 2; i++)
                packet.ReadInt32("Kill Credit " + i);

            for (var i = 0; i < 4; i++)
                packet.ReadInt32("Display ID " + i);

            packet.ReadSingle("Modifier 1");
            packet.ReadSingle("Modifier 2");
            packet.ReadBoolean("Racial Leader");

            for (var i = 0; i < 6; i++)
                packet.ReadInt32("Quest Item " + i);

            packet.ReadInt32("Movement ID");
            packet.ReadInt32("Unk");
        }

        [Parser(Index.HandleGameobjectQueryResponseIndex)]
        public static void GameobjectQueryResponse(Packet packet)
        {
            var entry = packet.ReadEntryKey("Entry");

            if (entry.Value)
                return;

            var type = (GameObjectType)packet.ReadInt32();
            Console.WriteLine("Type: " + type);

            packet.ReadInt32("Display ID");

            for (var i = 0; i < 4; i++)
                packet.ReadCString("Name " + i);

            packet.ReadCString("Icon Name");
            packet.ReadCString("Cast Caption");
            packet.ReadCString("Unk String");

            for (var i = 0; i < 24; i++)
                packet.ReadInt32("Data " + i);

            packet.ReadSingle("Size");

            for (var i = 0; i < 6; i++)
                packet.ReadInt32("Quest Item " + i);

            packet.ReadInt32("Unk");
        }

        [Parser(Index.HandleQuestQueryResponseIndex)]
        public static void HandleQuestQueryResponse(Packet packet)
        {
            packet.ReadInt32("Entry");
            var method = (QuestMethod)packet.ReadInt32();
            Console.WriteLine("Method: {0}", method);
            packet.ReadInt32("Level");
            packet.ReadInt32("MinLevel");
            var sort = packet.ReadInt32();
            if (sort >= 0)
                Console.WriteLine("Zone: {0}", sort);
            else
                Console.WriteLine("Sort: {0}", (QuestSort)(-sort));
            var questType = (QuestType)packet.ReadInt32();
            Console.WriteLine("Type: {0}", questType);
            packet.ReadInt32("GroupNum");
            packet.ReadInt32("RepObjFaction1");
            packet.ReadInt32("RepObjValue1");
            packet.ReadInt32("RepObjFaction2");
            packet.ReadInt32("RepObjValue2");
            packet.ReadInt32("NextQuestID");
            packet.ReadInt32("XPID");
            packet.ReadInt32("Money");
            packet.ReadInt32("MoneyAtMaxLevel");
            packet.ReadInt32("Spell");
            packet.ReadInt32("SpellCast");
            packet.ReadInt32("Honor");
            packet.ReadSingle("Honor (2)");
            packet.ReadInt32("Source Item ID");
            var questFlags = (QuestFlag)packet.ReadInt32();
            Console.WriteLine("Flags: {0} ({1})", (int)questFlags, questFlags);
            packet.ReadInt32("Unk");
            packet.ReadInt32("RewardTitleID");
            packet.ReadInt32("PlayersSlain");
            packet.ReadInt32("RewardTalentPoints");
            packet.ReadInt32("RewardArenaPoints");
            packet.ReadInt32("RewardSkillLineID");
            packet.ReadInt32("RewardSkillPoints");
            packet.ReadInt32("RewardFactionMask?");
            packet.ReadInt32("QuestGiverPortraitID");
            packet.ReadInt32("QuestTurnInPortraitID");
            for (int i = 0; i < 4; i++)
            {
                packet.ReadInt32("RewardItem[" + i + "]");
                packet.ReadInt32("RewardItemCount[" + i + "]");
            }
            for (int i = 0; i < 6; i++)
            {
                packet.ReadInt32("RewardItemChoice[" + i + "]");
                packet.ReadInt32("RewardItemChoiceCount[" + i + "]");
            }
            for (int i = 0; i < 5; i++)
                packet.ReadInt32("RewardRepFactionID[" + i + "]");
            for (int i = 0; i < 5; i++)
                packet.ReadInt32("RewardRepValueID[" + i + "]");
            for (int i = 0; i < 5; i++)
                packet.ReadInt32("RewardRepValue[" + i + "]");
            packet.ReadInt32("PointMapID");
            packet.ReadSingle("PointX");
            packet.ReadSingle("PointY");
            packet.ReadInt32("PointOption");
            packet.ReadCString("Title");
            packet.ReadCString("ObjectiveText");
            packet.ReadCString("Description");
            packet.ReadCString("EndText");
            // not sure...
            packet.ReadCString("CompletionText_Standard");
            for (int i = 0; i < 4; i++)
            {
                packet.ReadInt32("RequiredCreatureOrGOID[" + i + "]");
                packet.ReadInt32("RequiredCreatureOrGOCount[" + i + "]");
                packet.ReadInt32("ItemDropIntermediateID[" + i + "]");
                packet.ReadInt32("ItemDropIntermediateCount[" + i + "]");
            }
            for (int i = 0; i < 6; i++)
            {
                packet.ReadInt32("RequiredItemID[" + i + "]");
                packet.ReadInt32("RequiredItemCount[" + i + "]");
            }
            packet.ReadInt32("CriteriaSpellID");
            for (int i = 0; i < 4; i++)
                packet.ReadCString("ObjectiveTexts[" + i + "]");
            for (int i = 0; i < 4; i++)
            {
                packet.ReadInt32("RewardCurrencyID[" + i + "]");
                packet.ReadInt32("RewardCurrencyValue[" + i + "]");
            }
            for (int i = 0; i < 4; i++)
            {
                packet.ReadInt32("RequiredCurrencyID[" + i + "]");
                packet.ReadInt32("RequiredCurrencyValue[" + i + "]");
            }
            packet.ReadCString("QuestGiverPortraitText");
            packet.ReadCString("QuestGiverPortraitUnk");
            packet.ReadCString("QuestTurnInPortraitText");
            packet.ReadCString("QuestTurnInPortrainUnk");
            packet.ReadInt32("Sound field 1");
            packet.ReadInt32("Sound field 2");
        }

        [ClientToServerParser(Opcode.CMSG_QUEST_QUERY)]
        public static void HandleQuestQuery(Packet packet)
        {
            packet.ReadInt32("Quest ID");
        }
    }
}
