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
            Console.WriteLine("Guild query response");
            var unkLong = packet.ReadInt64();
            Console.WriteLine("Guild ID: " + unkLong);
            var guildName = packet.ReadCString();
            Console.WriteLine("Name: " + guildName);
            for (byte rank = 0; rank < 10; rank++) {
                var rankname = packet.ReadCString();
                Console.WriteLine("Rank " + rank + " name: " + rankname);
            }

            for (byte rank = 0; rank < 10; rank++)
            {
                var realRank = packet.ReadInt32();
                Console.WriteLine("Rank " + rank + " real rank ID: " + realRank);
            }

            for (byte rank = 0; rank < 10; rank++)
            {
                var unk = packet.ReadInt32();
                // ignore these as they seem to always be identical to the above loop
                //Console.WriteLine("Rank " + rank + " unk int: " + unk);
            }

            var emblemStyle = packet.ReadInt32();
            Console.WriteLine("Emblem style: " + emblemStyle);
            var emblemColor = packet.ReadInt32();
            Console.WriteLine("Emblem color: " + emblemColor);
            var emblemBorderStyle = packet.ReadInt32();
            Console.WriteLine("Emblem border style: " + emblemBorderStyle);
            var emblemBorderColor = packet.ReadInt32();
            Console.WriteLine("Emblem border color: " + emblemBorderColor);
            var emblemBackgroundColor = packet.ReadInt32();
            Console.WriteLine("Emblem background color: " + emblemBackgroundColor);
            var unkWotlk = packet.ReadInt32();
            Console.WriteLine("Unk int from wotlk: " + unkWotlk);
        }

        [Parser(Index.HandleNameQueryResponseIndex)]
        public static void HandleNameQueryResponse(Packet packet)
        {
            Console.WriteLine("Name query response");
            var pguid = packet.ReadPackedGuid();
            Console.WriteLine("GUID: " + pguid);

            var end = packet.ReadBoolean();
            Console.WriteLine("Name Found: " + !end);

            if (end)
                return;

            var name = packet.ReadCString();
            Console.WriteLine("Name: " + name);

            var realmName = packet.ReadCString();
            Console.WriteLine("Realm Name: " + realmName);

            var race = (Race)packet.ReadByte();
            Console.WriteLine("Race: " + race);

            var gender = (Gender)packet.ReadByte();
            Console.WriteLine("Gender: " + gender);

            var cClass = (Class)packet.ReadByte();
            Console.WriteLine("Class: " + cClass);

            var decline = packet.ReadBoolean();
            Console.WriteLine("Name Declined: " + decline);

            if (!decline)
                return;

            for (var i = 0; i < 5; i++)
            {
                var declinedName = packet.ReadCString();
                Console.WriteLine("Declined Name " + i + ": " + declinedName);
            }
        }

        [Parser(Index.HandleCreatureQueryResponseIndex)]
        public static void HandleCreatureQueryResponse(Packet packet)
        {
            Console.WriteLine("Creature query response");

            var entry = packet.ReadEntry();
            Console.WriteLine("Entry: " + entry.Key);

            if (entry.Value)
                return;

            var name = new string[4];
            for (var i = 0; i < 4; i++)
            {
                name[i] = packet.ReadCString();
                Console.WriteLine("Name " + i + ": " + name[i]);
            }

            var subName = packet.ReadCString();
            Console.WriteLine("Sub Name: " + subName);

            var iconName = packet.ReadCString();
            Console.WriteLine("Icon Name: " + iconName);

            var typeFlags = (CreatureTypeFlag)packet.ReadInt32();
            Console.WriteLine("Type Flags: " + typeFlags);

            var type = (CreatureType)packet.ReadInt32();
            Console.WriteLine("Type: " + type);

            var family = (CreatureFamily)packet.ReadInt32();
            Console.WriteLine("Family: " + family);

            var rank = (CreatureRank)packet.ReadInt32();
            Console.WriteLine("Rank: " + rank);

            var killCredit = new int[2];
            for (var i = 0; i < 2; i++)
            {
                killCredit[i] = packet.ReadInt32();
                Console.WriteLine("Kill Credit " + i + ": " + killCredit[i]);
            }

            var dispId = new int[4];
            for (var i = 0; i < 4; i++)
            {
                dispId[i] = packet.ReadInt32();
                Console.WriteLine("Display ID " + i + ": " + dispId[i]);
            }

            var mod1 = packet.ReadSingle();
            Console.WriteLine("Modifier 1: " + mod1);

            var mod2 = packet.ReadSingle();
            Console.WriteLine("Modifier 2: " + mod2);

            var racialLeader = packet.ReadBoolean();
            Console.WriteLine("Racial Leader: " + racialLeader);

            var qItem = new int[6];
            for (var i = 0; i < 6; i++)
            {
                qItem[i] = packet.ReadInt32();
                Console.WriteLine("Quest Item " + i + ": " + qItem[i]);
            }

            var moveId = packet.ReadInt32();
            Console.WriteLine("Movement ID: " + moveId);

            var unk = packet.ReadInt32();
            Console.WriteLine("Unk: " + unk);
        }

        [Parser(Index.HandleGameobjectQueryResponseIndex)]
        public static void GameobjectQueryResponse(Packet packet)
        {
            Console.WriteLine("Gameobject Query Response");

            var entry = packet.ReadEntry();
            Console.WriteLine("Entry: " + entry.Key);

            if (entry.Value)
                return;

            var type = (GameObjectType)packet.ReadInt32();
            Console.WriteLine("Type: " + type);

            var dispId = packet.ReadInt32();
            Console.WriteLine("Display ID: " + dispId);

            var name = new string[4];
            for (var i = 0; i < 4; i++)
            {
                name[i] = packet.ReadCString();
                Console.WriteLine("Name " + i + ": " + name[i]);
            }

            var iconName = packet.ReadCString();
            Console.WriteLine("Icon Name: " + iconName);

            var castCaption = packet.ReadCString();
            Console.WriteLine("Cast Caption: " + castCaption);

            var unkStr = packet.ReadCString();
            Console.WriteLine("Unk String: " + unkStr);

            var data = new int[24];
            for (var i = 0; i < 24; i++)
            {
                data[i] = packet.ReadInt32();
                Console.WriteLine("Data " + i + ": " + data[i]);
            }

            var size = packet.ReadSingle();
            Console.WriteLine("Size: " + size);

            var qItem = new int[6];
            for (var i = 0; i < 6; i++)
            {
                qItem[i] = packet.ReadInt32();
                Console.WriteLine("Quest Item " + i + ": " + qItem[i]);
            }

            var unk = packet.ReadInt32();
            Console.WriteLine("Unk: " + unk);
        }
    }
}
