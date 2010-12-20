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
                    DoAttributes(method, typeof(ParserAttribute), Handlers);
                    DoAttributes(method, typeof(SpecialParserAttribute), SpecialHandlers);
                    DoAttributes(method, typeof(ClientToServerParserAttribute), ClientToServerHandlers);
                }
            }
        }

        private static bool DoAttributes(MethodInfo method, Type type, Dictionary<int, Action<Packet>> handlers)
        {
            var tmp = method.GetCustomAttributes(type, false);

            object[] attrs;
            if (type == typeof(ParserAttribute))
                attrs = (ParserAttribute[])tmp;
            else if (type == typeof(SpecialParserAttribute))
                attrs = (SpecialParserAttribute[])tmp;
            else if (type == typeof(ClientToServerParserAttribute))
                attrs = (ClientToServerParserAttribute[])tmp;
            else
                return false;

            if (attrs.Length <= 0)
                return false;

            var parms = method.GetParameters();

            if (parms.Length <= 0)
                return false;

            if (parms[0].ParameterType != typeof(Packet))
                return false;

            foreach (var attr in attrs)
            {
                int index;
                if (type == typeof(ParserAttribute))
                    index = ((ParserAttribute)attr).index;
                else if (type == typeof(SpecialParserAttribute))
                {
                    index = ((SpecialParserAttribute)attr).Index;
                    SpecialHandlerNames[index] = ((SpecialParserAttribute)attr).Name;
                }
                else // if (type == typeof(ClientToServerParserAttribute))
                {
                    index = (int)((ClientToServerParserAttribute)attr).Opcode;
                }

                var del = (Action<Packet>)Delegate.CreateDelegate(typeof(Action<Packet>), method);

                handlers[index] = del;
            }
            return true;
        }

        public static void InitializeLogFile(string file, string nodump, string nohex, string skiplarge)
        {
            _noDump = nodump.Equals(bool.TrueString, StringComparison.InvariantCultureIgnoreCase);
            if (_noDump)
                return;

            _noHex = nohex.Equals(bool.TrueString, StringComparison.InvariantCultureIgnoreCase);
            _skipLarge = skiplarge.Equals(bool.TrueString, StringComparison.InvariantCultureIgnoreCase);

            File.Delete(file);
            _file = new StreamWriter(file, true);
            Console.SetOut(_file);
        }

        private static bool _noDump;

        private static bool _noHex;

        private static bool _skipLarge;

        private static StreamWriter _file;

        private static readonly Dictionary<int, Action<Packet>> Handlers =
            new Dictionary<int, Action<Packet>>();

        private static readonly Dictionary<int, Action<Packet>> SpecialHandlers =
            new Dictionary<int, Action<Packet>>();

        private static readonly Dictionary<int, string> SpecialHandlerNames =
            new Dictionary<int, string>();

        private static readonly Dictionary<int, Action<Packet>> ClientToServerHandlers =
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
            if (packet.GetDirection() == 1)
            {
                ParseClientToServerPacket(packet);
            }
            else
            {
                if ((opcode & 0xCF07) == 0x800)
                    ParseSpecialPacket(packet);
                else
                    ParseStandardPacket(packet);
            }
        }

        public static void ParseSpecialPacket(Packet packet)
        {
            var opcode = packet.GetOpcode();
            var caseNum = (opcode & 0xF8 | (opcode >> 4) & 0x300) >> 3;
            var time = packet.GetTime();
            var direction = packet.GetDirection();
            var length = packet.GetLength();
            bool handlerFound = false;

            if (SpecialHandlers.ContainsKey(caseNum))
            {
                var handler = SpecialHandlers[caseNum];
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine("{0}: {1} (0x{2}, Special) (Case: {3} ({4} / 0x{5})) Length: {6} Time: {7}", (direction == 1) ? "Client->Server" : "Server->Client",
                    (Opcode)opcode, ((int)opcode).ToString("X4"), SpecialHandlerNames[caseNum], (int)caseNum, ((int)caseNum).ToString("X4"), length, time);

                Console.ForegroundColor = ConsoleColor.White;

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
            else if (!_noHex)
            {
                if (!(_skipLarge && length > 10000))
                {
                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine("{0}: {1} (0x{2}, Special) Length: {3} Time: {4}", (direction == 1) ? "Client->Server" : "Server->Client",
                        opcode, ((int)opcode).ToString("X4"), length, time);

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Utilities.DumpPacketAsHex(packet));
                }
                else
                    packet.SetPosition(packet.GetLength());
            }

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
            if (handlerFound || !_noHex)
                Console.WriteLine();
        }

        public static void ParseStandardPacket(Packet packet)
        {
            var opcode = packet.GetOpcode();
            var offset = opcode & 3 | ((opcode & 8 | ((opcode & 0x20 | ((opcode & 0x300 | (opcode >> 1) & 0x7C00) >> 2)) >> 1)) >> 1);
            var time = packet.GetTime();
            var direction = packet.GetDirection();
            var length = packet.GetLength();
            bool handlerFound = false;

            if (Handlers.ContainsKey(offset))
            {
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine("{0}: {1} (0x{2}) (Condensed: {3} ({4} / 0x{5})) Length: {6} Time: {7}", (direction == 1) ? "Client->Server" : "Server->Client",
                    (Opcode)opcode, ((int)opcode).ToString("X4"), (Index)offset, (int)offset, ((int)offset).ToString("X4"), length, time);

                Console.ForegroundColor = ConsoleColor.White;
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
            else if (!_noHex)
            {
                if (!(_skipLarge && length > 10000))
                {
                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine("{0}: {1} (0x{2}) Length: {3} Time: {4}", (direction == 1) ? "Client->Server" : "Server->Client",
                        opcode, ((int)opcode).ToString("X4"), length, time);

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Utilities.DumpPacketAsHex(packet));
                }
                else
                    packet.SetPosition(packet.GetLength());
            }

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
            if (handlerFound || !_noHex)
                Console.WriteLine();
        }

        public static void ParseClientToServerPacket(Packet packet)
        {
            var opcode = packet.GetOpcode();
            var time = packet.GetTime();
            var direction = packet.GetDirection();
            var length = packet.GetLength();
            bool handlerFound = false;

            if (ClientToServerHandlers.ContainsKey(opcode))
            {
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine("Client->Server: {0} (0x{1}) Length: {2} Time: {3}", 
                    (Opcode)opcode, ((int)opcode).ToString("X4"), length, time);

                Console.ForegroundColor = ConsoleColor.White;
                var handler = ClientToServerHandlers[opcode];

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
            else if (!_noHex)
            {
                if (!(_skipLarge && length > 10000))
                {
                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine("Client->Server: {0} ({1} 0x{2}) Length: {3} Time: {4}",
                        (Opcode)opcode, opcode, ((int)opcode).ToString("X4"), length, time);

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Utilities.DumpPacketAsHex(packet));
                }
                else
                    packet.SetPosition(packet.GetLength());
            }

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
            if (handlerFound || !_noHex)
                Console.WriteLine();
        }
    }
}
