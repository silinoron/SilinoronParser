using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using SilinoronParser.Enums;
using SilinoronParser.Loading;
using SilinoronParser.Loading.Loaders;
using SilinoronParser.Util;
using SilinoronParser.Parsing;

namespace SilinoronParser
{
    public class Program
    {
        public static CommandLine CmdLine { get; private set; }

        private static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            CmdLine = new CommandLine(args);

            string file;
            string loader;
            string nodump;
            string nohex;

            try
            {
                file = CmdLine.GetValue("-file");
                loader = CmdLine.GetValue("-loader");
                nodump = CmdLine.GetValue("-nodump");
                nohex = CmdLine.GetValue("-nohex");
            }
            catch (IndexOutOfRangeException)
            {
                PrintUsage("All command line options require an argument.");
                return;
            }

            try
            {
                var packets = Reader.Read(loader, file);
                if (packets == null)
                {
                    PrintUsage("Could not open file " + file + " for reading.");
                    return;
                }

                if (packets.Count() > 0)
                {
                    var fullPath = Utilities.GetPathFromFullPath(file);
                    Handler.InitializeLogFile(Path.Combine(fullPath, file + ".txt"), nodump, nohex);

                    foreach (var packet in packets)
                        Handler.Parse(packet);
                    Handler.WriteToFile();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetType());
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            Console.ResetColor();
        }

        public static void PrintUsage(string error)
        {
            var n = Environment.NewLine;

            if (!string.IsNullOrEmpty(error))
                Console.WriteLine(error + n);

            var usage = "Usage: SilinoronParser -file <input file> -loader <loader type> " +
                "[-nodump <boolean>] [-nohex <boolean>]" + n + n +
                "-file\t\tThe file to read packets from." + n +
                "-loader\t\tThe loader to use (zor4xx/tiawps/izidor)." + n +
                "-nodump\t\tSet to True to disable file logging." + n +
                "-nohex\t\tSet to True to not print out hex dumps.";

            Console.WriteLine(usage);
        }
    }
}
