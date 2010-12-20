using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SilinoronParser.Enums
{
    public enum Index : ushort
    {
        HandleNameQueryResponseIndex = 598,
        HandleCreatureQueryResponseIndex = 1798,
        HandleGameobjectQueryResponseIndex = 27,
        HandleGuildQueryResponseIndex = 420,
        HandleUpdateObjectIndex = 1997,
        HandleDestroyObjectIndex = 891,
        HandleMonsterMoveIndex = 1317,
        HandleMonsterMoveTransportIndex = 956,
        HandleMessageChatIndex = 1071,
        HandleCompressedUpdateObjectIndex = 845, // real opcode is 0x6C7D
        HandleEmoteIndex = 836,
        HandleTextEmoteIndex = 755,
        HandleChannelListIndex = 901,
        HandlePlayedTimeIndex = 532,
        HandleNotificationIndex = 806,
        HandleHeartbeatIndex = 188,
        HandleSetPitchIndex = 108,
        HandleSetFacingIndex = 1061,
        HandleGuildInfoIndex = 975,
        HandleQuestQueryResponseIndex = 935,
    }

    public enum Opcode : ushort
    {
        SMSG_MONSTER_MOVE = 0xA65D,
        SMSG_MONSTER_MOVE_TRANSPORT = 0x777C,
        SMSG_COMPRESSED_UPDATE_OBJECT = 0x6C7D,
        SMSG_UPDATE_OBJECT = 0xFC7D,
        SMSG_MESSAGECHAT = 0x867F,
        SMSG_DESTROY_OBJECT = 0x6F77,
        SMSG_EMOTE = 0x6C5C,
        SMSG_GUILD_QUERY_RESPONSE = 0x365C,
        SMSG_NAME_QUERY_RESPONSE = 0x4D5E,
        SMSG_CREATURE_QUERY_RESPONSE = 0xE45E,
        SMSG_GAMEOBJECT_QUERY_RESPONSE = 0x0577,
        MSG_MOVE_HEARTBEAT = 0x177C,
        CMSG_QUEST_QUERY = 0xFF7D,
        SMSG_QUEST_QUERY_RESPONSE = 0x765F,
        MSG_MOVE_SET_PITCH = 0x0E7C,
        MSG_MOVE_SET_FACING = 0x865D,
    }
}
