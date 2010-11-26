using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SilinoronParser.Enums;
using SilinoronParser.Util;

namespace SilinoronParser.Parsing
{
    public static class Handler
    {
        static Handler()
        {
            var asm = Assembly.GetExecutingAssembly();
            var types = asm.GetTypes();
            foreach (var type in types)
            {
                if (!type.IsAbstract)
                    continue;

                if (!type.IsPublic)
                    continue;

                var methods = type.GetMethods();

                foreach (var method in methods)
                {
                    if (!method.IsPublic)
                        continue;

                    var attrs = (ParserAttribute[])method.GetCustomAttributes(typeof(ParserAttribute),
                        false);

                    if (attrs.Length <= 0)
                        continue;

                    var parms = method.GetParameters();

                    if (parms.Length <= 0)
                        continue;

                    if (parms[0].ParameterType != typeof(Packet))
                        continue;

                    foreach (var attr in attrs)
                    {
                        var index = attr.index;

                        var del = (Action<Packet>)Delegate.CreateDelegate(typeof(Action<Packet>), method);

                        Handlers[index] = del;
                    }
                }
            }
        }

        public static void InitializeLogFile(string file, string nodump)
        {
            _noDump = nodump.Equals(bool.TrueString, StringComparison.InvariantCultureIgnoreCase);
            if (_noDump)
                return;

            File.Delete(file);
            _file = new StreamWriter(file, true);
            Console.SetOut(_file);
        }

        private static bool _noDump;

        private static StreamWriter _file;

        private static readonly Dictionary<int, Action<Packet>> Handlers =
            new Dictionary<int, Action<Packet>>();

        public static void WriteToFile()
        {
            if (_noDump)
                return;

            _file.Flush();
            _file.Close();
            _file = null;
        }

        public static void Parse(Packet packet)
        {
            var opcode = packet.GetOpcode();
            var offset = opcode & 3 | ((opcode & 8 | ((opcode & 0x20 | ((opcode & 0x300 | (opcode >> 1) & 0x7C00) >> 2)) >> 1)) >> 1);
            var time = packet.GetTime();
            var direction = packet.GetDirection();
            var length = packet.GetLength();

            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine("{0}: {1} (0x{2}) Length: {3} Time: {4}", (direction == 0) ? "Client->Server" : "Server->Client",
                opcode, ((int)opcode).ToString("X4"), length, time);

            Console.ForegroundColor = ConsoleColor.White;

            var handlerFound = false;
            if (Handlers.ContainsKey(offset))
            {
                var handler = Handlers[offset];

                try
                {
                    handlerFound = true;
                    handler(packet);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.GetType());
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
            else
                Console.WriteLine(Utilities.DumpPacketAsHex(packet));

#if DEBUG
            if (handlerFound && packet.GetPosition() < packet.GetLength())
            {
                Console.ForegroundColor = ConsoleColor.Red;

                var pos = packet.GetPosition();
                var len = packet.GetLength();
                Console.WriteLine("Packet not fully read! Current position is {0}, length is {1}, and diff is {2}.",
                    pos, len, len - pos);

                Console.ForegroundColor = ConsoleColor.White;
            }
#endif

            Console.ResetColor();
            Console.WriteLine();
        }
    }
}
