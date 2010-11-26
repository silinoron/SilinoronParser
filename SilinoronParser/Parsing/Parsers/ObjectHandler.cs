using System;
using System.Collections;
using System.Collections.Generic;
using SilinoronParser.Util;
using SilinoronParser.Enums;
using Guid = SilinoronParser.Util.Guid;

namespace SilinoronParser.Parsing.Parsers
{
    public static class ObjectHandler
    {
        public static readonly Dictionary<int, Dictionary<Guid, WowObject>> Objects = new Dictionary<int, Dictionary<Guid, WowObject>>();

        // temporarily disable this...
        //[Parser(Index.HandleUpdateObjectIndex)]
        public static void HandleUpdateObject(Packet packet)
        {
            Console.WriteLine("Update Object");
            var map = packet.ReadUInt16();
            Console.WriteLine("Map: " + map);
            var count = packet.ReadInt32();
            Console.WriteLine("Count: " + count);

            long sposition = packet.GetPosition();
            var unkByte = packet.ReadByte();
            if (unkByte != 3)
                packet.SetPosition(sposition);
            else
            {
                Console.WriteLine("unkByte: " + unkByte);
                var unkInt = packet.ReadUInt32();
                Console.WriteLine("Unk int: " + unkInt);
                long position = packet.GetPosition();
                if (unkInt != 0) {
                    // stuff needs doing
                    for (uint i = 0; i < unkInt; i++) {
                        var guid = packet.ReadPackedGuid();
                        Console.WriteLine("Unk guid: " + guid);
                    }
                }
                // the client does it AGAIN for some reason
                //packet.SetPosition(position);
                packet.SetPosition(sposition);
            }

            for (var i = 0; i < count; i++)
            {
                var type = (UpdateType)packet.ReadByte();
                Console.WriteLine("Update Type: " + type);

                switch (type)
                {
                    case UpdateType.Values:
                        {
                            var guid = packet.ReadPackedGuid();
                            Console.WriteLine("GUID: " + guid);

                            var updates = ReadValuesUpdateBlock(packet);

                            WowObject obj;
                            if (Objects[MovementHandler.CurrentMapId].TryGetValue(guid, out obj))
                                HandleUpdateFieldChangedValues(false, guid, obj.Type, updates, obj.Movement);
                            break;
                        }
                    case UpdateType.Movement:
                        {
                            var guid = packet.ReadPackedGuid();
                            Console.WriteLine("GUID: " + guid);

                            ReadMovementUpdateBlock(packet, guid);
                            break;
                        }
                    case UpdateType.CreateObject1:
                    case UpdateType.CreateObject2:
                        {
                            var guid = packet.ReadPackedGuid();
                            Console.WriteLine("GUID: " + guid);

                            ReadCreateObjectBlock(packet, guid, map);
                            break;
                        }
                    case UpdateType.FarObjects:
                    case UpdateType.NearObjects:
                        {
                            var objCount = packet.ReadInt32();
                            Console.WriteLine("Object Count: " + objCount);

                            for (var j = 0; j < objCount; j++)
                            {
                                var guid = packet.ReadPackedGuid();
                                Console.WriteLine("Object GUID: " + guid);
                            }
                            break;
                        }
                }
            }
        }

        public static void ReadCreateObjectBlock(Packet packet, Guid guid, ushort map)
        {
            var objType = (ObjectType)packet.ReadByte();
            Console.WriteLine("Object Type: " + objType);

            var moves = ReadMovementUpdateBlock(packet, guid);
            var updates = ReadValuesUpdateBlock(packet);

            var obj = new WowObject(guid, objType, moves);
            obj.Position = moves.Position;

            var objects = Objects[map];
            var shouldAdd = true;
            foreach (var woObj in objects.Values)
            {
                if (woObj.Position != obj.Position && woObj.Guid != guid)
                    continue;

                shouldAdd = false;
                break;
            }

            if (!shouldAdd)
                return;

            objects.Add(guid, obj);

            HandleUpdateFieldChangedValues(true, guid, objType, updates, moves);
        }

        public static Dictionary<int, UpdateField> ReadValuesUpdateBlock(Packet packet)
        {
            var maskSize = packet.ReadByte();
            Console.WriteLine("Block Count: " + maskSize);

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
                                fieldName = "minlevel = " + val.Int32Value + ", maxlevel";
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
                                fieldName = "faction_A = " + val.Int32Value + ", faction_H";
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
        }

        public static MovementInfo ReadMovementUpdateBlock(Packet packet, Guid guid)
        {
            var moveInfo = new MovementInfo();

            var flags = (UpdateFlag)packet.ReadInt16();
            Console.WriteLine("Update Flags: " + flags);

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
                    var splineFlags = (SplineFlag)packet.ReadInt32();
                    Console.WriteLine("Spline Flags: " + splineFlags);

                    if (splineFlags.HasFlag(SplineFlag.FinalPoint))
                    {
                        var splineCoords = packet.ReadVector3();
                        Console.WriteLine("Final Spline Coords: " + splineCoords);
                    }

                    if (splineFlags.HasFlag(SplineFlag.FinalTarget))
                    {
                        var splineTarget = packet.ReadGuid();
                        Console.WriteLine("Final Spline Target GUID: " + splineTarget);
                    }

                    if (splineFlags.HasFlag(SplineFlag.FinalOrientation))
                    {
                        var splineOrient = packet.ReadSingle();
                        Console.WriteLine("Final Spline Orientation: " + splineOrient);
                    }

                    var splineTime = packet.ReadInt32();
                    Console.WriteLine("Spline Time: " + splineTime);

                    var fullTime = packet.ReadInt32();
                    Console.WriteLine("Spline Full Time: " + fullTime);

                    var unk1 = packet.ReadInt32();
                    Console.WriteLine("Spline Unk Int32 1: " + unk1);

                    var duraMul = packet.ReadSingle();
                    Console.WriteLine("Spline Duration Multiplier: " + duraMul);

                    var unk3 = packet.ReadSingle();
                    Console.WriteLine("Spline Unit Interval: " + unk3);

                    var unk4 = packet.ReadSingle();
                    Console.WriteLine("Spline Unk Float 2: " + unk4);

                    var unk5 = packet.ReadInt32();
                    Console.WriteLine("Spline Height Time: " + unk5);

                    var splineCount = packet.ReadInt32();
                    for (var i = 0; i < splineCount; i++)
                    {
                        var coords = packet.ReadVector3();
                        Console.WriteLine("Spline Waypoint " + i + ": " + coords);
                    }

                    var mode = (SplineMode)packet.ReadByte();
                    Console.WriteLine("Spline Mode: " + mode);

                    var endpoint = packet.ReadVector3();
                    Console.WriteLine("Spline Endpoint: " + endpoint);
                }
            }
            else
            {
                if (flags.HasFlag(UpdateFlag.GOPosition))
                {
                    var goguid = packet.ReadPackedGuid();
                    Console.WriteLine("GO Position GUID: " + goguid);

                    var gopos = packet.ReadVector3();
                    Console.WriteLine("GO Position: " + gopos);

                    var gopos2 = packet.ReadVector3();
                    Console.WriteLine("GO Transport Position: " + gopos2);

                    var goFacing = packet.ReadSingle();
                    Console.WriteLine("GO Orientation: " + goFacing);

                    moveInfo.Position = gopos;
                    moveInfo.Orientation = goFacing;

                    var goflt = packet.ReadSingle();
                    Console.WriteLine("GO Transport Orientation: " + goflt);
                }
                else if (flags.HasFlag(UpdateFlag.StationaryObject))
                {
                    var ufpos = packet.ReadVector4();
                    Console.WriteLine("Stationary Position: " + ufpos);
                }
            }

            if (flags.HasFlag(UpdateFlag.Unknown1))
            {
                var lguid = packet.ReadInt32();
                Console.WriteLine("Unk Int32: " + lguid);
            }

            if (flags.HasFlag(UpdateFlag.LowGuid))
            {
                var hguid = packet.ReadInt32();
                Console.WriteLine("Low GUID: " + hguid);
            }

            if (flags.HasFlag(UpdateFlag.AttackingTarget))
            {
                var targetGuid = packet.ReadPackedGuid();
                Console.WriteLine("Target GUID: " + targetGuid);
            }

            if (flags.HasFlag(UpdateFlag.Transport))
            {
                var ttime = packet.ReadInt32();
                Console.WriteLine("Transport Creation Time: " + ttime);
            }

            if (flags.HasFlag(UpdateFlag.Vehicle))
            {
                var vehId = packet.ReadInt32();
                Console.WriteLine("Vehicle ID: " + vehId);

                var vehFacing = packet.ReadSingle();
                Console.WriteLine("Vehicle Orientation: " + vehFacing);
            }

            if (flags.HasFlag(UpdateFlag.GORotation))
            {
                var gorot = packet.ReadPackedQuaternion();
                Console.WriteLine("GO Rotation: " + gorot);
            }

            return moveInfo;
        }

        // need to find this one
        /* [Parser(Opcode.SMSG_COMPRESSED_UPDATE_OBJECT)]
        public static void HandleCompressedUpdateObject(Packet packet)
        {
            var decompCount = packet.ReadInt32();
            var pkt = packet.Inflate(decompCount);
            HandleUpdateObject(pkt);
        } */

        /* [Parser(Index.HandleDestroyObjectIndex)]
        public static void HandleDestroyObject(Packet packet)
        {
            Console.WriteLine("Destroy Object");
            var guid = packet.ReadGuid();
            Console.WriteLine("GUID: " + guid);

            var anim = packet.ReadBoolean();
            Console.WriteLine("Despawn Animation: " + anim);
        } */
    }
}
