using System;
using System.Collections;
using System.Collections.Generic;
using SilinoronParser.Util;
using SilinoronParser.Enums;
using Guid = SilinoronParser.Util.Guid;
using SilinoronParser.SQLOutput;

namespace SilinoronParser.Parsing.Parsers
{
    public static class ObjectHandler
    {
        public static readonly Dictionary<int, Dictionary<Guid, WowObject>> Objects = new Dictionary<int, Dictionary<Guid, WowObject>>();
        private static readonly Dictionary<uint, int> minLevels = new Dictionary<uint, int>();
        private static readonly Dictionary<uint, int> maxLevels = new Dictionary<uint, int>();

        [Parser(Index.HandleUpdateObjectIndex)]
        public static void HandleUpdateObject(Packet packet)
        {
            var map = packet.ReadInt16("Map");
            var count = packet.ReadInt32("Count");

            long sposition = packet.GetPosition();
            var unkByte = packet.ReadByte();
            if (unkByte != 3)
                packet.SetPosition(sposition);
            else
            {
                Console.WriteLine("firstType: " + unkByte);
                var guidCount = packet.ReadInt32("GUID Count");
                if (guidCount > 0) {
                    for (uint i = 0; i < guidCount; i++)
                        packet.ReadPackedGuid("GUID " + (i + 1));
                }
            }

            for (var i = 0; i < (count - ((unkByte == 3) ? 1 : 0)); i++)
            {
                var type = packet.ReadEnum<UpdateType>("Update Type #" + (i + 1));

                switch (type)
                {
                    case UpdateType.Values:
                        {
                            var guid = packet.ReadPackedGuid("GUID");
                            var updates = ReadValuesUpdateBlock(packet);

                            WowObject obj;
                            if (Objects.ContainsKey(map) && Objects[map].TryGetValue(guid, out obj))
                                HandleUpdateFieldChangedValues(false, guid, obj.Type, updates, obj.Movement);
                            break;
                        }
                    case UpdateType.Movement:
                        {
                            var guid = packet.ReadPackedGuid("GUID");
                            packet.ReadEnum<ObjectType>("Object type");
                            ReadMovementUpdateBlock(packet, guid);
                            ReadValuesUpdateBlock(packet);
                            break;
                        }
                    case UpdateType.CreateObject1:
                    case UpdateType.CreateObject2:
                        {
                            var guid = packet.ReadPackedGuid("GUID");
                            ReadCreateObjectBlock(packet, guid, map);
                            break;
                        }
                    case UpdateType.FarObjects:
                    case UpdateType.NearObjects:
                        {
                            var objCount = packet.ReadInt32("Object Count");

                            for (var j = 0; j < objCount; j++)
                                packet.ReadPackedGuid("Object GUID");
                            break;
                        }
                }
            }
        }

        public static void ReadCreateObjectBlock(Packet packet, Guid guid, short map)
        {
            var objType = packet.ReadEnum<ObjectType>("Object Type");
            var moves = ReadMovementUpdateBlock(packet, guid);
            var updates = ReadValuesUpdateBlock(packet);

            var obj = new WowObject(guid, objType, moves);
            obj.Position = moves.Position;

            if (!Objects.ContainsKey(map))
                Objects.Add(map, new Dictionary<Guid, WowObject>());
            Objects[map].Add(guid, obj);
            HandleUpdateFieldChangedValues(true, guid, objType, updates, moves);
        }

        public static Dictionary<int, UpdateField> ReadValuesUpdateBlock(Packet packet)
        {
            var maskSize = packet.ReadByte("Block Count");

            var updateMask = new int[maskSize];
            for (var i = 0; i < maskSize; i++)
            {
                var blockIdx = packet.ReadInt32();
                updateMask[i] = blockIdx;
            }

            var mask = new BitArray(updateMask);
            var dict = new Dictionary<int, UpdateField>();

            for (var i = 0; i < mask.Count; i++)
            {
                if (!mask[i])
                    continue;

                var blockVal = packet.ReadUpdateField();
                Console.WriteLine("Block Value " + i + ": " + blockVal.Int32Value + "/" +
                    blockVal.SingleValue);

                dict.Add(i, blockVal);
            }

            return dict;
        }

        public static void HandleUpdateFieldChangedValues(bool creating, Guid guid, ObjectType objType,
                    Dictionary<int, UpdateField> updates, MovementInfo moves)
        {
            bool shouldCommit;
            bool isIntValue;
            bool flags;
            string fieldName = null;

            if (objType == ObjectType.Unit && guid.GetHighType() != HighGuidType.Pet)
            {

                foreach (var upVal in updates)
                {
                    bool shouldOverride = false;
                    int overrideVal = -1;
                    Dictionary<string, string> overrideDict = new Dictionary<string, string>();
                    bool shouldOverrideDict = false;
                    bool isTemplate = false;
                    shouldCommit = true;
                    isIntValue = true;
                    flags = false;

                    var idx = (UnitField)upVal.Key;
                    var val = upVal.Value;

                    switch (idx)
                    {
                        case UnitField.UNIT_CREATED_BY_SPELL:
                        case UnitField.UNIT_FIELD_CREATEDBY:
                        case UnitField.UNIT_FIELD_SUMMONEDBY:
                            {
                                shouldCommit = false;
                                break;
                            }
                        case (UnitField)ObjectField.OBJECT_FIELD_SCALE_X:
                            {
                                fieldName = "scale";
                                isIntValue = false;
                                break;
                            }
                        case UnitField.UNIT_DYNAMIC_FLAGS:
                            {
                                fieldName = "dynamicflags";
                                flags = true;
                                break;
                            }
                        case UnitField.UNIT_NPC_FLAGS:
                            {
                                fieldName = "npcflag";
                                flags = true;
                                break;
                            }
                        case UnitField.UNIT_FIELD_FLAGS:
                            {
                                fieldName = "unit_flags";
                                flags = true;
                                break;
                            }
                        case UnitField.UNIT_FIELD_ATTACK_POWER:
                            {
                                fieldName = "attackpower";
                                break;
                            }
                        case UnitField.UNIT_FIELD_BASEATTACKTIME:
                            {
                                fieldName = "baseattacktime";
                                break;
                            }
                        case UnitField.UNIT_FIELD_LEVEL:
                            {
                                int lvl = val.Int32Value;
                                uint entry = guid.GetEntry();
                                bool addMin = true;
                                bool addMax = true;
                                isTemplate = true;
                                if (minLevels.ContainsKey(entry))
                                    if (lvl >= minLevels[entry])
                                        addMin = false;
                                if (maxLevels.ContainsKey(entry))
                                    if (lvl <= maxLevels[entry])
                                        addMax = false;

                                if (addMin)
                                {
                                    overrideDict.Add("minlevel", lvl.ToString());
                                    minLevels[entry] = lvl;
                                }
                                if (addMax)
                                {
                                    overrideDict.Add("maxlevel", lvl.ToString());
                                    maxLevels[entry] = lvl;
                                }
                                
                                if (!addMin && !addMax)
                                    shouldCommit = false;

                                shouldOverrideDict = true;
                                break;
                            }
                        case UnitField.UNIT_FIELD_RANGED_ATTACK_POWER:
                            {
                                fieldName = "rangedattackpower";
                                break;
                            }
                        case UnitField.UNIT_FIELD_RANGEDATTACKTIME:
                            {
                                fieldName = "rangeattacktime";
                                break;
                            }
                        case UnitField.UNIT_FIELD_FACTIONTEMPLATE:
                            {
                                isTemplate = true;
                                fieldName = "faction_A=" + val.Int32Value + ", faction_H";
                                break;
                            }
                        case UnitField.UNIT_FIELD_BASE_HEALTH:
                            {
                                fieldName = "minhealth = " + val.Int32Value + ", maxhealth";
                                break;
                            }
                        case UnitField.UNIT_FIELD_BASE_MANA:
                            {
                                fieldName = "minmana = " + val.Int32Value + ", maxmana";
                                break;
                            }
                        case UnitField.UNIT_FIELD_BYTES_0:
                            {
                                fieldName = "unitclass";
                                overrideVal = ((val.Int32Value & 0x00FF0000) >> 16);
                                isTemplate = true;
                                shouldOverride = true;
                                break;
                            }
                        default:
                            {
                                shouldCommit = false;
                                break;
                            }
                    }

                    if (!shouldCommit)
                        continue;

                    var finalValue = shouldOverride ? (object) overrideVal : (isIntValue ? val.Int32Value : val.SingleValue);

                    if (flags && finalValue is int)
                        finalValue = "0x" + ((int)finalValue).ToString("X8");

                    if (isTemplate)
                    {
                        Dictionary<string, string> data;
                        if (shouldOverrideDict)
                            data = overrideDict;
                        else {
                            data = new Dictionary<string, string>();
                            data.Add(fieldName, finalValue.ToString());
                        }
                        CreatureTemplateUpdate update = new CreatureTemplateUpdate(guid.GetEntry(), data);
                        CreatureTemplateUpdateStorage.GetSingleton().Add(update);
                    }
                    else
                    {

                    }
                }
            }

            if (objType != ObjectType.GameObject)
                return;

            foreach (var upVal in updates)
            {
                shouldCommit = true;
                isIntValue = true;
                flags = false;

                var idx = (GameObjectField)upVal.Key;
                var val = upVal.Value;

                switch (idx)
                {
                    case GameObjectField.OBJECT_FIELD_CREATED_BY:
                        {
                            shouldCommit = false;
                            break;
                        }
                    case (GameObjectField)ObjectField.OBJECT_FIELD_SCALE_X:
                        {
                            fieldName = "size";
                            isIntValue = false;
                            break;
                        }
                    case GameObjectField.GAMEOBJECT_FACTION:
                        {
                            fieldName = "faction";
                            break;
                        }
                    case GameObjectField.GAMEOBJECT_FLAGS:
                        {
                            fieldName = "flags";
                            flags = true;
                            break;
                        }
                    default:
                        {
                            shouldCommit = false;
                            break;
                        }
                }

                if (!shouldCommit)
                    continue;

                var finalValue = isIntValue ? (object)val.Int32Value : val.SingleValue;

                if (flags)
                    finalValue = "0x" + ((int)finalValue).ToString("X8");
            }

            // add to storage
        }

        public static MovementInfo ReadMovementUpdateBlock(Packet packet, Guid guid)
        {
            var moveInfo = new MovementInfo();

            var flags = packet.ReadEnum<UpdateFlag>("Update Flags");

            if (flags.HasFlag(UpdateFlag.Living))
            {
                moveInfo = MovementHandler.ReadMovementInfo(packet, guid);
                var moveFlags = moveInfo.Flags;

                for (var i = 0; i < 9; i++)
                {
                    var j = (SpeedType)i;
                    var speed = packet.ReadSingle();
                    Console.WriteLine(j + " Speed: " + speed);

                    switch (j)
                    {
                        case SpeedType.Walk:
                            {
                                moveInfo.WalkSpeed = speed / 2.5f;
                                break;
                            }
                        case SpeedType.Run:
                            {
                                moveInfo.RunSpeed = speed / 7.0f;
                                break;
                            }
                    }
                }

                if (moveFlags.HasFlag(MoveFlag.SplineEnabled))
                {
                    var splineFlags = packet.ReadEnum<SplineFlag>("Spline Flags");

                    if (splineFlags.HasFlag(SplineFlag.FinalPoint))
                        packet.ReadVector3("Final Spline Coords");

                    if (splineFlags.HasFlag(SplineFlag.FinalTarget))
                        packet.ReadGuid("Final Spline Target GUID");

                    if (splineFlags.HasFlag(SplineFlag.FinalOrientation))
                        packet.ReadSingle("Final Spline Orientation");

                    packet.ReadInt32("Spline Time");
                    packet.ReadInt32("Spline Full Time");
                    packet.ReadInt32("Spline Unk Int32 1");
                    packet.ReadSingle("Spline Duration Multiplier");
                    packet.ReadSingle("Spline Unit Interval");
                    packet.ReadSingle("Spline Unk Float 2");
                    packet.ReadInt32("Spline Height Time");

                    var splineCount = packet.ReadInt32("Spline Count");
                    for (var i = 0; i < splineCount; i++)
                        packet.ReadVector3("Spline Waypoint " + i);

                    packet.ReadEnum<SplineMode>("Spline Mode");
                    packet.ReadVector3("Spline Endpoint");
                }
            }
            else
            {
                if (flags.HasFlag(UpdateFlag.GOPosition))
                {
                    packet.ReadPackedGuid("GO Position GUID");
                    var gopos = packet.ReadVector3("GO Position");
                    packet.ReadVector3("GO Transport Position");
                    var goFacing = packet.ReadSingle("GO Orientation");
                    moveInfo.Position = gopos;
                    moveInfo.Orientation = goFacing;
                    packet.ReadSingle("GO Transport Orientation");
                }
                else if (flags.HasFlag(UpdateFlag.StationaryObject))
                    packet.ReadVector4("Stationary Position");
            }

            if (flags.HasFlag(UpdateFlag.Unknown1))
                packet.ReadInt32("Unk Int32");

            if (flags.HasFlag(UpdateFlag.LowGuid))
                packet.ReadInt32("Low GUID");

            if (flags.HasFlag(UpdateFlag.AttackingTarget))
                packet.ReadPackedGuid("Target GUID");

            if (flags.HasFlag(UpdateFlag.Transport))
                packet.ReadInt32("Transport Creation Time");

            if (flags.HasFlag(UpdateFlag.Vehicle))
            {
                packet.ReadInt32("Vehicle ID");
                packet.ReadSingle("Vehicle Orientation");
            }

            if (flags.HasFlag(UpdateFlag.GORotation))
                packet.ReadPackedQuaternion("GO Rotation");

            return moveInfo;
        }

        [Parser(Index.HandleCompressedUpdateObjectIndex)]
        public static void HandleCompressedUpdateObject(Packet packet)
        {
            var decompCount = packet.ReadInt32();
            var pkt = packet.Inflate(decompCount);
            HandleUpdateObject(pkt);

            // otherwise Handler.cs will bitch about not reading the entire packet
            packet.SetPosition(packet.GetLength());
        }

        [Parser(Index.HandleDestroyObjectIndex)]
        public static void HandleDestroyObject(Packet packet)
        {
            packet.ReadGuid("GUID");
            packet.ReadBoolean("Despawn Animation");
        }
    }
}
