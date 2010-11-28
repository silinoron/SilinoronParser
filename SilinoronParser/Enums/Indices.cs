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
    }

    // opcodes that are confirmed; but this enum should remain short
    public enum Opcode : ushort
    {
        SMSG_MONSTER_MOVE_TRANSPORT = 0x777C,
        SMSG_COMPRESSED_UPDATE_OBJECT = 0x6C7D
    }
}
