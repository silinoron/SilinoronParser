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

            info.Flags = packet.ReadEnum<MoveFlag>("Movement Flags");

            var flags2 = packet.ReadEnum<MoveFlagExtra>("Extra Movement Flags");

            packet.ReadInt32("Time");
            var pos = packet.ReadVector4("Position");
            info.Position = new Vector3(pos.X, pos.Y, pos.Z);
            info.Orientation = pos.O;

            if (info.Flags.HasFlag(MoveFlag.OnTransport))
            {
                packet.ReadPackedGuid("Transport GUID");
                packet.ReadVector4("Transport Position");
                packet.ReadInt32("Transport Time");
                packet.ReadByte("Transport Seat");

                if (flags2.HasFlag(MoveFlagExtra.InterpolatedPlayerMovement))
                    packet.ReadInt32("Transport Time 2");
            }

            if (info.Flags.HasAnyFlag(MoveFlag.Swimming | MoveFlag.Flying) ||
                flags2.HasFlag(MoveFlagExtra.AlwaysAllowPitching))
                packet.ReadSingle("Swim Pitch");

            packet.ReadInt32("Fall Time");

            if (info.Flags.HasFlag(MoveFlag.Falling))
            {
                packet.ReadSingle("Jump Velocity");
                packet.ReadSingle("Jump Sin");
                packet.ReadSingle("Jump Cos");
                packet.ReadSingle("Jump XY Speed");
            }

            if (info.Flags.HasFlag(MoveFlag.SplineElevation))
                packet.ReadSingle("Spline Elevation");

            return info;
        }

        [ClientToServerParser(Opcode.MSG_MOVE_HEARTBEAT)]
        [ClientToServerParser(Opcode.MSG_MOVE_SET_PITCH)]
        [ClientToServerParser(Opcode.MSG_MOVE_SET_FACING)]
        [Parser(Index.HandleHeartbeatIndex)]
        [Parser(Index.HandleSetPitchIndex)]
        [Parser(Index.HandleSetFacingIndex)]
        public static void ParseMovementPackets(Packet packet)
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

            var type = packet.ReadEnum<SplineType>("Spline Type");

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

            var flags = packet.ReadEnum<SplineFlag>("Spline Flags");

            if (flags.HasFlag(SplineFlag.Unknown3)) {
                packet.ReadEnum<MoveAnimationState>("Animation State");
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
