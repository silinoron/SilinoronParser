using System.Collections.Generic;
using System.IO;
using SilinoronParser.Util;
using SilinoronParser.Enums;

namespace SilinoronParser.Loading.Loaders
{
    [Loader("zor4xx")]
    public sealed class Zor4xxLoader : Loader
    {
        public Zor4xxLoader(string file) : base(file) { }

        public override IEnumerable<Packet> ParseFile()
        {
            var bin = new BinaryReader(new FileStream(FileToParse, FileMode.Open));
            var packets = new List<Packet>();

            while (bin.BaseStream.Position != bin.BaseStream.Length) {
                ushort opcode = bin.ReadUInt16();
                int length = bin.ReadInt32();
                byte direction = bin.ReadBoolean() ? (byte)0 : (byte)1;
                var time = Utilities.GetDateTimeFromUnixTime(bin.ReadInt64());
                var data = bin.ReadBytes(length);
                Packet packet = new Packet(data, opcode, time, direction);
                packets.Add(packet);
            }

            return packets;
        }
    }
}
