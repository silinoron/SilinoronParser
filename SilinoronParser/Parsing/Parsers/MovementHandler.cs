using System;
using SilinoronParser.Util;
using SilinoronParser.Enums;
using Guid = SilinoronParser.Util.Guid;

namespace SilinoronParser.Parsing.Parsers
{
    public static class MovementHandler
    {
        public static Vector4 CurrentPosition;

        public static int CurrentMapId;

        public static int CurrentPhaseMask = 1;

        public static MovementInfo ReadMovementInfo(Packet packet, Guid guid)
        {
            var info = new MovementInfo();

            var moveFlags = (MoveFlag)packet.ReadInt32();
            Console.WriteLine("Movement Flags: " + moveFlags);
            info.Flags = moveFlags;

            var flags2 = (MoveFlagExtra)packet.ReadInt16();
            Console.WriteLine("Extra Movement Flags: " + flags2);

            var time = packet.ReadInt32();
            Console.WriteLine("Time: " + time);

            var pos = packet.ReadVector4();
            Console.WriteLine("Position: " + pos);
            info.Position = new Vector3(pos.X, pos.Y, pos.Z);
            info.Orientation = pos.O;

            if (moveFlags.HasFlag(MoveFlag.OnTransport))
            {
                var tguid = packet.ReadPackedGuid();
                Console.WriteLine("Transport GUID: " + tguid);

                var tpos = packet.ReadVector4();
                Console.WriteLine("Transport Position: " + tpos);

                var ttime = packet.ReadInt32();
                Console.WriteLine("Transport Time: " + ttime);

                var seat = packet.ReadByte();
                Console.WriteLine("Transport Seat: " + seat);

                if (flags2.HasFlag(MoveFlagExtra.InterpolatedPlayerMovement))
                {
                    var ttime2 = packet.ReadInt32();
                    Console.WriteLine("Transport Time 2: " + ttime2);
                }
            }

            if (moveFlags.HasAnyFlag(MoveFlag.Swimming | MoveFlag.Flying) ||
                flags2.HasFlag(MoveFlagExtra.AlwaysAllowPitching))
            {
                var swimPitch = packet.ReadSingle();
                Console.WriteLine("Swim Pitch: " + swimPitch);
            }

            var fallTime = packet.ReadInt32();
            Console.WriteLine("Fall Time: " + fallTime);

            if (moveFlags.HasFlag(MoveFlag.Falling))
            {
                var junk = packet.ReadSingle();
                Console.WriteLine("Jump Velocity: " + junk);

                var jsin = packet.ReadSingle();
                Console.WriteLine("Jump Sin: " + jsin);

                var jcos = packet.ReadSingle();
                Console.WriteLine("Jump Cos: " + jcos);

                var jxys = packet.ReadSingle();
                Console.WriteLine("Jump XY Speed: " + jxys);
            }

            if (moveFlags.HasFlag(MoveFlag.SplineElevation))
            {
                var unkSpline = packet.ReadSingle();
                Console.WriteLine("Spline Elevation: " + unkSpline);
            }

            return info;
        }

        [Parser(188)]
        public static void ParseUnkMovementPacket(Packet packet)
        {
            Console.WriteLine("Unk movement packet");
            var guid = packet.ReadPackedGuid();
            Console.WriteLine("Guid: " + guid);
            var movementInfo = ReadMovementInfo(packet, guid);
        }

        [Parser(Index.HandleMonsterMoveIndex)]
        [Parser(Index.HandleMonsterMoveTransportIndex)]
        public static void ParseMonsterMovePackets(Packet packet)
        {
            if (packet.GetOpcode() == (ushort)Opcode.SMSG_MONSTER_MOVE_TRANSPORT)
                Console.WriteLine("SMSG_MONSTER_MOVE_TRANSPORT");
            else
                Console.WriteLine("SMSG_MONSTER_MOVE");

            var guid = packet.ReadPackedGuid();
            Console.WriteLine("GUID: " + guid);

            if (packet.GetOpcode() == (ushort)Opcode.SMSG_MONSTER_MOVE_TRANSPORT)
            {
                var transguid = packet.ReadPackedGuid();
                Console.WriteLine("Transport GUID: " + transguid);

                var transseat = packet.ReadByte();
                Console.WriteLine("Transport Seat: " + transseat);
            }

            var unkByte = packet.ReadBoolean();
            Console.WriteLine("Unk Boolean: " + unkByte);

            var pos = packet.ReadVector3();
            Console.WriteLine("Position: " + pos);

            var curTime = packet.ReadInt32();
            Console.WriteLine("Move Ticks: " + curTime);

            var type = (SplineType)packet.ReadByte();
            Console.WriteLine("Spline Type: " + type);

            switch (type)
            {
                case SplineType.FacingSpot:
                    {
                        var spot = packet.ReadVector3();
                        Console.WriteLine("Facing Spot: " + spot);
                        break;
                    }
                case SplineType.FacingTarget:
                    {
                        var tguid = packet.ReadGuid();
                        Console.WriteLine("Facing GUID: " + tguid);
                        break;
                    }
                case SplineType.FacingAngle:
                    {
                        var angle = packet.ReadSingle();
                        Console.WriteLine("Facing Angle: " + angle);
                        break;
                    }
                case SplineType.Stop:
                    {
                        return;
                    }
            }

            var flags = (SplineFlag)packet.ReadInt32();
            Console.WriteLine("Spline Flags: " + flags);

            if (flags.HasFlag(SplineFlag.Unknown3)) {
                var unkByte3 = (MoveAnimationState)packet.ReadByte();
                Console.WriteLine("Animation State: " + unkByte3);

                var unkInt1 = packet.ReadInt32();
                Console.WriteLine("Unk Int32 1: " + unkInt1);
            }

            var time = packet.ReadInt32();
            Console.WriteLine("Move Time: " + time);

            if (flags.HasFlag(SplineFlag.Trajectory)) {
                var unkFloat = packet.ReadSingle();
                Console.WriteLine("Unk Single: " + unkFloat);

                var unkInt2 = packet.ReadInt32();
                Console.WriteLine("Unk Int32 2: " + unkInt2);
            }

            var waypoints = packet.ReadInt32();
            Console.WriteLine("Waypoints: " + waypoints);

            var newpos = packet.ReadVector3();
            Console.WriteLine("Waypoint 0: " + newpos);

            if (flags.HasFlag(SplineFlag.Flying) || flags.HasFlag(SplineFlag.CatmullRom)) {
                for (var i = 0; i < waypoints - 1; i++) {
                    var vec = packet.ReadVector3();
                    Console.WriteLine("Waypoint " + (i + 1) + ": " + vec);
                }
            }
            else {
                var mid = new Vector3();
                mid.X = (pos.X + newpos.X) * 0.5f;
                mid.Y = (pos.Y + newpos.Y) * 0.5f;
                mid.Z = (pos.Z + newpos.Z) * 0.5f;

                for (var i = 0; i < waypoints - 1; i++) {
                    var vec = packet.ReadPackedVector3();
                    vec.X += mid.X;
                    vec.Y += mid.Y;
                    vec.Z += mid.Z;

                    Console.WriteLine("Waypoint " + (i + 1) + ": " + vec);
                }
            }
        }
    }
}
