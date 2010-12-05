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

            packet.ReadInt32("Time");
            var pos = packet.ReadVector4("Position");
            info.Position = new Vector3(pos.X, pos.Y, pos.Z);
            info.Orientation = pos.O;

            if (moveFlags.HasFlag(MoveFlag.OnTransport))
            {
                packet.ReadPackedGuid("Transport GUID");
                packet.ReadVector4("Transport Position");
                packet.ReadInt32("Transport Time");
                packet.ReadByte("Transport Seat");

                if (flags2.HasFlag(MoveFlagExtra.InterpolatedPlayerMovement))
                    packet.ReadInt32("Transport Time 2");
            }

            if (moveFlags.HasAnyFlag(MoveFlag.Swimming | MoveFlag.Flying) ||
                flags2.HasFlag(MoveFlagExtra.AlwaysAllowPitching))
                packet.ReadSingle("Swim Pitch");

            packet.ReadInt32("Fall Time");

            if (moveFlags.HasFlag(MoveFlag.Falling))
            {
                packet.ReadSingle("Jump Velocity");
                packet.ReadSingle("Jump Sin");
                packet.ReadSingle("Jump Cos");
                packet.ReadSingle("Jump XY Speed");
            }

            if (moveFlags.HasFlag(MoveFlag.SplineElevation))
                packet.ReadSingle("Spline Elevation");

            return info;
        }

        [Parser(Index.HandleHeartbeatIndex)]
        public static void ParseUnkMovementPacket(Packet packet)
        {
            var guid = packet.ReadPackedGuid("Guid");
            ReadMovementInfo(packet, guid);
        }

        [Parser(Index.HandleMonsterMoveIndex)]
        [Parser(Index.HandleMonsterMoveTransportIndex)]
        public static void ParseMonsterMovePackets(Packet packet)
        {
            var guid = packet.ReadPackedGuid("GUID");

            if (packet.GetOpcode() == (ushort)Opcode.SMSG_MONSTER_MOVE_TRANSPORT)
            {
                packet.ReadPackedGuid("Transport GUID");
                packet.ReadByte("Transport Seat");
            }

            packet.ReadBoolean("Unk Boolean");
            var pos = packet.ReadVector3("Position");
            packet.ReadInt32("Move Ticks");

            var type = (SplineType)packet.ReadByte();
            Console.WriteLine("Spline Type: " + type);

            switch (type)
            {
                case SplineType.FacingSpot:
                    {
                        packet.ReadVector3("Facing Spot");
                        break;
                    }
                case SplineType.FacingTarget:
                    {
                        packet.ReadGuid("Facing GUID");
                        break;
                    }
                case SplineType.FacingAngle:
                    {
                        packet.ReadSingle("Facing Angle");
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

                packet.ReadInt32("Unk Int32 1");
            }

            packet.ReadInt32("Move Time");

            if (flags.HasFlag(SplineFlag.Trajectory)) {
                packet.ReadSingle("Unk Single");
                packet.ReadInt32("Unk Int32 2");
            }

            var waypoints = packet.ReadInt32("Waypoints");

            var newpos = packet.ReadVector3("Waypoint 0");

            if (flags.HasFlag(SplineFlag.Flying) || flags.HasFlag(SplineFlag.CatmullRom)) {
                for (var i = 0; i < waypoints - 1; i++) {
                    packet.ReadVector3("Waypoint " + (i + 1));
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
