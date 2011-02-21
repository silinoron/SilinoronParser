using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SilinoronParser.Util;

namespace SilinoronParser.Loading.Loaders
{
    [Loader("synric")]
    public sealed class SynricLoader : Loader
    {
        public SynricLoader(string file)
            : base(file)
        {
        }

        public override IEnumerable<Packet> ParseFile()
        {
            var packets = new List<Packet>();
            int lineNum = 1;
            try
            {
                using (TextReader tr = new StreamReader(FileToParse))
                {
                    byte direction = 0;
                    ushort opcode = 0;
                    DateTime time = DateTime.Now;
                    bool inPacket = false;
                    string packetData = "";
                    while (tr.Peek() != -1)
                    {
                        string line = tr.ReadLine();

                        if (line.Length == 0)
                        {
                            lineNum++;
                            continue; // skip the useless empty lines
                        }

                        if (line.StartsWith("{"))
                        {
                            // packet metadata
                            //         [0]         [1]     [2]      [3]     [4]        [5] [6]
                            // format: {DIRECTION} Packet: (OPCODE) UNKNOWN PacketSize = SIZE
                            string[] metadata = line.Split(' ');
                            string directionStr = metadata[0].Substring(1, metadata[0].Length - 2);
                            direction = (byte)(directionStr == "SERVER" ? 0 : 1);
                            time = DateTime.Now; // not saved in file
                            byte[] opcodeData = ParseHex(metadata[2].Substring(1, metadata[2].Length - 2));
                            opcode = (ushort)((((ushort)opcodeData[0] * 0x100) + (ushort)opcodeData[1]));
                            for (int i = 0; i < 7; i++)
                            {
                                lineNum++;
                                tr.ReadLine();
                            }
                            inPacket = true;
                        }
                        else if (line.StartsWith("|"))
                        {
                            if (!inPacket)
                            {
                                Console.WriteLine("Packet data without packet! Possibly corrupted data? Line num {0}", lineNum);
                                lineNum++;
                                continue;
                            }
                            // right. We're in a packet.
                            // looks like this:
                            // |B5 9B 87 9F 91 AC D7 D2 66 DF D8 8F 53 7E A6 2C |
                            string lineData = line.Split('|')[1];
                            lineData = lineData.Replace(" ", "");
                            lineData = lineData.Replace("|", "");
                            packetData += lineData;
                        }
                        else if (line.StartsWith("-"))
                        {
                            byte[] byteData = ParseHex(packetData);
                            Packet p = new Packet(byteData, opcode, time, direction);
                            packets.Add(p);
                            inPacket = false;
                            packetData = "";
                            direction = 0;
                            opcode = 0;
                        }
                        lineNum++;
                    }
                }
            }
            catch
            {
                Console.WriteLine("Exception while reading line {0} - probably a spliced packet", lineNum);
            }

            return packets;
        }

        public static byte[] ParseHex(string hex)
        {
            int offset = hex.StartsWith("0x") ? 2 : 0;

            if ((hex.Length % 2) != 0)
                throw new ArgumentException("Invalid length: " + hex.Length);

            byte[] ret = new byte[(hex.Length - offset) / 2];

            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = (byte)((ParseNybble(hex[offset]) << 4) | ParseNybble(hex[offset + 1]));
                offset += 2;
            }
            return ret;
        }

        static int ParseNybble(char c)
        {
            if (c >= '0' && c <= '9')
                return c - '0';
            else if (c >= 'A' && c <= 'F')
                return c - 'A' + 10;
            else if (c >= 'a' && c <= 'f')
                return c - 'a' + 10;
            else
                throw new ArgumentException("Invalid hex digit: " + c);
        }

    }
}
