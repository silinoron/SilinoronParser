using System;
using System.Collections.Generic;
using System.Data.SQLite;
using SilinoronParser.Enums;
using SilinoronParser.Util;

namespace SilinoronParser.Loading.Loaders
{
    [Loader("tiawps")]
    public sealed class TiawpsLoader : Loader
    {
        public TiawpsLoader(string file)
            : base(file)
        {
        }

        public override IEnumerable<Packet> ParseFile()
        {
            var packets = new List<Packet>();
            var conn = new SQLiteConnection("Data Source=" + FileToParse);

            conn.Open();

            var cmd = new SQLiteCommand("SELECT opcode, direction, timestamp, data FROM packets", conn);
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ushort opcode = (ushort)(reader.GetInt32(0));
                byte direction = (byte)(reader.GetInt32(1));
                var timestamp = reader.GetDateTime(2);
                var data = (byte[])reader.GetValue(3);

                var packet = new Packet(data, opcode, timestamp, direction);
                packets.Add(packet);
            }

            return packets;
        }
    }
}
