using System;
using SilinoronParser.Util;
using SilinoronParser.Enums;

namespace SilinoronParser.Parsing.Parsers
{
    public static class ChatHandler
    {
        [Parser(Index.HandleMessageChatIndex)]
        public static void HandleMessageChat(Packet packet)
        {
            var type = packet.ReadEnum<ChatMessageType>("Type");
            packet.ReadEnum<Language>("Language");
            packet.ReadGuid("GUID");
            packet.ReadInt32("Unk Int32");

            switch (type)
            {
                case ChatMessageType.Say:
                case ChatMessageType.Yell:
                case ChatMessageType.Party:
                case ChatMessageType.PartyLeader:
                case ChatMessageType.Raid:
                case ChatMessageType.RaidLeader:
                case ChatMessageType.RaidWarning:
                case ChatMessageType.Guild:
                case ChatMessageType.Officer:
                case ChatMessageType.Emote:
                case ChatMessageType.TextEmote:
                case ChatMessageType.Whisper:
                case ChatMessageType.WhisperInform:
                case ChatMessageType.System:
                case ChatMessageType.Channel:
                case ChatMessageType.Battleground:
                case ChatMessageType.BattlegroundNeutral:
                case ChatMessageType.BattlegroundAlliance:
                case ChatMessageType.BattlegroundHorde:
                case ChatMessageType.BattlegroundLeader:
                case ChatMessageType.Achievement:
                case ChatMessageType.GuildAchievement:
                    {
                        if (type == ChatMessageType.Channel)
                            packet.ReadCString("Channel Name");

                        packet.ReadGuid("Sender GUID");
                        break;
                    }
                case ChatMessageType.MonsterSay:
                case ChatMessageType.MonsterYell:
                case ChatMessageType.MonsterParty:
                case ChatMessageType.MonsterEmote:
                case ChatMessageType.MonsterWhisper:
                case ChatMessageType.RaidBossEmote:
                case ChatMessageType.RaidBossWhisper:
                case ChatMessageType.BattleNet:
                    {
                        packet.ReadInt32("Name Length");
                        packet.ReadCString("Name");
                        var target = packet.ReadGuid("Receiver GUID");

                        break;
                    }
            }

            packet.ReadInt32("Text Length");
            packet.ReadCString("Text");
            packet.ReadEnum<ChatTag>("Chat Tag");

            if (type != ChatMessageType.Achievement && type != ChatMessageType.GuildAchievement)
                return;

            packet.ReadInt32("Achievement ID");
        }

        [Parser(Index.HandleEmoteIndex)]
        public static void HandleEmote(Packet packet)
        {
            packet.ReadInt32("Emote");
            packet.ReadGuid("GUID");
        }

        [Parser(Index.HandleTextEmoteIndex)]
        public static void HandleTextEmote(Packet packet)
        {
            packet.ReadGuid("GUID");
            packet.ReadInt32("Text Emote");
            packet.ReadInt32("Emote #");
            packet.ReadInt32("Name Length");
            packet.ReadCString("Name");
        }

        [Parser(Index.HandleChannelListIndex)]
        public static void HandleChannelList(Packet packet)
        {
            packet.ReadByte("Channel Type");
            packet.ReadCString("Channel Name");
            packet.ReadByte("Channel Flags");
            var listSize = packet.ReadInt32("List Size");

            for (int i = 0; i < listSize; i++)
            {
                packet.ReadGuid("Player GUID");
                packet.ReadByte("Player Flags");
            }
        }
    }
}
