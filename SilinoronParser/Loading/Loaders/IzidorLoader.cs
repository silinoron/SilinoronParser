using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SilinoronParser.Util;

namespace SilinoronParser.Loading.Loaders
{
    [Loader("izidor")]
    public sealed class IzidorLoader : Loader
    {
        public IzidorLoader(string file)
            : base(file)
        {
        }

        public override IEnumerable<Packet> ParseFile()
        {
            var packets = new List<Packet>();

            using (TextReader tr = new StreamReader(FileToParse))
            {
                while (tr.Peek() != -1)
                {
                    string line = tr.ReadLine();
                    string[] data = line.Split('<', '>', '"');
                    DateTime time = Utilities.GetDateTimeFromUnixTime(Int32.Parse(data[2]));
                    byte direction = (byte)(data[4] == "StoC" ? 0 : 1);
                    ushort opcode = UInt16.Parse(data[6]);
                    string directdata = data[8];
                    byte[] byteData = ParseHex(directdata);
                    Packet p = new Packet(byteData, opcode, time, direction);
                    packets.Add(p);
                }
            }
            return packets;
        }

        public static byte[] ParseHex(string hex)
        {
            int offset = hex.StartsWith("0x") ? 2 : 0;
            if ((hex.Length % 2) != 0)
            {
                throw new ArgumentException("Invalid length: " + hex.Length);
            }
            byte[] ret = new byte[(hex.Length - offset) / 2];

            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = (byte)((ParseNybble(hex[offset]) << 4)
                                 | ParseNybble(hex[offset + 1]));
                offset += 2;
            }
            return ret;
        }

        static int ParseNybble(char c)
        {
            if (c >= '0' && c <= '9')
            {
                return c - '0';
            }
            if (c >= 'A' && c <= 'F')
            {
                return c - 'A' + 10;
            }
            if (c >= 'a' && c <= 'f')
            {
                return c - 'a' + 10;
            }
            throw new ArgumentException("Invalid hex digit: " + c);
        }

    }
}
