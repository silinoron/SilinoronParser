using System;
using SilinoronParser.Util;
using SilinoronParser.Enums;
using SilinoronParser.SQLOutput;

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

            var race = packet.ReadEnum<Race>("Race");
            var gender = packet.ReadEnum<Gender>("Gender");
            var cClass = packet.ReadEnum<Class>("Class");

            var decline = packet.ReadBoolean("Name Declined");

            if (!decline)
                return;

            for (var i = 0; i < 5; i++)
                packet.ReadCString("Declined Name " + i);
        }

        [Parser(Index.HandleCreatureQueryResponseIndex)]
        public static void HandleCreatureQueryResponse(Packet packet)
        {
            Creature c = new Creature();
            var entry = packet.ReadEntryKey("Entry");

            if (entry.Value)
                return;

            c.Entry = entry.Key;

            for (var i = 0; i < 4; i++)
                c.Name[i] = packet.ReadCString("Name " + i);

            c.SubName = packet.ReadCString("Sub Name");
            c.IconName = packet.ReadCString("Icon Name");

            c.TypeFlags = packet.ReadEnum<CreatureTypeFlag>("Type Flags");
            c.Type = packet.ReadEnum<CreatureType>("Type");
            c.Family = packet.ReadEnum<CreatureFamily>("Family");
            c.Rank = packet.ReadEnum<CreatureRank>("Rank");
            c.KillCredit1 = packet.ReadInt32("Kill Credit 1");
            c.KillCredit2 = packet.ReadInt32("Kill Credit 2");

            for (var i = 0; i < 4; i++)
                c.DisplayIDs[i] = packet.ReadInt32("Display ID " + i);

            c.HealthModifier = packet.ReadSingle("Health Modifier");
            c.ManaModifier = packet.ReadSingle("Mana Modifier");
            c.RacialLeader = packet.ReadBoolean("Racial Leader");

            for (var i = 0; i < 6; i++)
                c.QuestItems[i] = packet.ReadInt32("Quest Item " + i);

            c.MovementID = packet.ReadInt32("Movement ID");
            c.Exp = packet.ReadInt32("Expansion ID");
            CreatureStorage.GetSingleton().Add(c);
        }

        [Parser(Index.HandleGameobjectQueryResponseIndex)]
        public static void GameobjectQueryResponse(Packet packet)
        {
            GameObject go = new GameObject();
            var entry = packet.ReadEntryKey("Entry");
            go.Entry = entry.Key;
            if (entry.Value)
                return;

            go.Type = packet.ReadEnum<GameObjectType>("Type");
            go.DisplayID = packet.ReadInt32("Display ID");

            for (var i = 0; i < 4; i++)
                go.Name[i] = packet.ReadCString("Name " + i);

            go.IconName = packet.ReadCString("Icon Name");
            go.CastCaption = packet.ReadCString("Cast Caption");
            go.UnkString = packet.ReadCString("Unk String");

            for (var i = 0; i < 24; i++)
                go.Data[i] = packet.ReadInt32("Data " + i);

            go.Size = packet.ReadSingle("Size");

            for (var i = 0; i < 6; i++)
                go.QuestItems[i] = packet.ReadInt32("Quest Item " + i);

            go.Exp = packet.ReadInt32("Expansion ID");
            GameObjectStorage.GetSingleton().Add(go);
        }

        [Parser(Index.HandleQuestQueryResponseIndex)]
        public static void HandleQuestQueryResponse(Packet packet)
        {
            Quest q = new Quest();
            q.Entry = packet.ReadInt32("Entry");
            q.Method = packet.ReadEnum<QuestMethod>("Method");
            q.Level = packet.ReadInt32("Level");
            q.MinLevel = packet.ReadInt32("MinLevel");
            var sort = packet.ReadInt32();
            if (sort >= 0)
                Console.WriteLine("Zone: {0}", sort);
            else
                Console.WriteLine("Sort: {0}", (QuestSort)(-sort));
            q.ZoneOrSort = sort;
            q.Type = packet.ReadEnum<QuestType>("Type");
            q.SuggestedPlayers = packet.ReadInt32("SuggestedPlayers");
            q.RepObjectiveFaction1 = packet.ReadInt32("RepObjFaction1");
            q.RepObjectiveValue1 = packet.ReadInt32("RepObjValue1");
            q.RepObjectiveFaction2 = packet.ReadInt32("RepObjFaction2");
            q.RepObjectiveValue2 = packet.ReadInt32("RepObjValue2");
            q.NextQuestID = packet.ReadInt32("NextQuestID");
            q.XPID = packet.ReadInt32("XPID");
            q.RewardMoney = packet.ReadInt32("Money");
            q.RewardMoneyAtMaxLevel = packet.ReadInt32("MoneyAtMaxLevel");
            q.Spell = packet.ReadInt32("Spell");
            q.SpellCast = packet.ReadInt32("SpellCast");
            q.Honor = packet.ReadInt32("Honor");
            q.HonorMultiplier = packet.ReadSingle("Honor (2)");
            q.SourceItemID = packet.ReadInt32("Source Item ID");
            q.Flags = packet.ReadEnum<QuestFlag>("Flags");
            q.Unk = packet.ReadInt32("Unk");
            q.RewardTitleID = packet.ReadInt32("RewardTitleID");
            q.PlayersSlain = packet.ReadInt32("PlayersSlain");
            q.RewardTalentPoints = packet.ReadInt32("RewardTalentPoints");
            q.RewardArenaPoints = packet.ReadInt32("RewardArenaPoints");
            q.RewardSkillLineID = packet.ReadInt32("RewardSkillLineID");
            q.RewardSkillPoints = packet.ReadInt32("RewardSkillPoints");
            q.RewardFactionMask = packet.ReadInt32("RewardFactionMask");
            q.QuestGiverPortraitID = packet.ReadInt32("QuestGiverPortraitID");
            q.QuestTurnInPortraitID = packet.ReadInt32("QuestTurnInPortraitID");
            for (int i = 0; i < 4; i++)
            {
                q.RewardItem[i] = packet.ReadInt32("RewardItem[" + i + "]");
                q.RewardItemCount[i] = packet.ReadInt32("RewardItemCount[" + i + "]");
            }
            for (int i = 0; i < 6; i++)
            {
                q.RewardItemChoice[i] = packet.ReadInt32("RewardItemChoice[" + i + "]");
                q.RewardItemChoiceCount[i] = packet.ReadInt32("RewardItemChoiceCount[" + i + "]");
            }
            for (int i = 0; i < 5; i++)
                q.RewardRepFactionID[i] = packet.ReadInt32("RewardRepFactionID[" + i + "]");
            for (int i = 0; i < 5; i++)
                q.RewardRepValueID[i] = packet.ReadInt32("RewardRepValueID[" + i + "]");
            for (int i = 0; i < 5; i++)
                q.RewardRepValue[i] = packet.ReadInt32("RewardRepValue[" + i + "]");
            q.PointMapID = packet.ReadInt32("PointMapID");
            q.PointX = packet.ReadSingle("PointX");
            q.PointY = packet.ReadSingle("PointY");
            q.PointOption = packet.ReadInt32("PointOption");
            q.Title = packet.ReadCString("Title");
            q.ObjectiveText = packet.ReadCString("ObjectiveText");
            q.Description = packet.ReadCString("Description");
            q.EndText = packet.ReadCString("EndText");
            q.CompletionText = packet.ReadCString("CompletionText");
            for (int i = 0; i < 4; i++)
            {
                q.RequiredCreatureOrGOID[i] = packet.ReadInt32("RequiredCreatureOrGOID[" + i + "]");
                q.RequiredCreatureOrGOCount[i] = packet.ReadInt32("RequiredCreatureOrGOCount[" + i + "]");
                q.ItemDropIntermediateID[i] = packet.ReadInt32("ItemDropIntermediateID[" + i + "]");
                q.ItemDropIntermediateCount[i] = packet.ReadInt32("ItemDropIntermediateCount[" + i + "]");
            }
            for (int i = 0; i < 6; i++)
            {
                q.RequiredItemID[i] = packet.ReadInt32("RequiredItemID[" + i + "]");
                q.RequiredItemCount[i] = packet.ReadInt32("RequiredItemCount[" + i + "]");
            }
            q.CriteriaSpellID = packet.ReadInt32("CriteriaSpellID");
            for (int i = 0; i < 4; i++)
                q.ObjectiveTexts[i] = packet.ReadCString("ObjectiveTexts[" + i + "]");
            for (int i = 0; i < 4; i++)
            {
                q.RewardCurrencyID[i] = packet.ReadInt32("RewardCurrencyID[" + i + "]");
                q.RewardCurrencyValue[i] = packet.ReadInt32("RewardCurrencyValue[" + i + "]");
            }
            for (int i = 0; i < 4; i++)
            {
                q.RequiredCurrencyID[i] = packet.ReadInt32("RequiredCurrencyID[" + i + "]");
                q.RequiredCurrencyValue[i] = packet.ReadInt32("RequiredCurrencyValue[" + i + "]");
            }
            q.QuestGiverPortraitText = packet.ReadCString("QuestGiverPortraitText");
            q.QuestGiverPortraitUnk = packet.ReadCString("QuestGiverPortraitUnk");
            q.QuestTurnInPortraitText = packet.ReadCString("QuestTurnInPortraitText");
            q.QuestTurnInPortraitUnk = packet.ReadCString("QuestTurnInPortrainUnk");
            q.SoundField1 = packet.ReadInt32("Sound field 1");
            q.SoundField2 = packet.ReadInt32("Sound field 2");
            QuestStorage.GetSingleton().Add(q);
        }

        [ClientToServerParser(Opcode.CMSG_QUEST_QUERY)]
        public static void HandleQuestQuery(Packet packet)
        {
            packet.ReadInt32("Quest ID");
        }
    }
}
