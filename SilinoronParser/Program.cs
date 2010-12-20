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
using SilinoronParser.SQLOutput;
using Guid=SilinoronParser.Util.Guid;
using SilinoronParser.Parsing.Parsers;

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
            string tosql;
            string skiplarge;
            bool _toSQL = false;

            try
            {
                file = CmdLine.GetValue("-file");
                loader = CmdLine.GetValue("-loader");
                nodump = CmdLine.GetValue("-nodump");
                nohex = CmdLine.GetValue("-nohex");
                tosql = CmdLine.GetValue("-tosql");
                skiplarge = CmdLine.GetValue("-skiplarge");
                if (tosql.Equals(bool.TrueString, StringComparison.InvariantCultureIgnoreCase))
                    _toSQL = true;
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
                    Handler.InitializeLogFile(Path.Combine(fullPath, file + ".txt"), nodump, nohex, skiplarge);

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
            if (_toSQL)
            {
                var fullPath = Utilities.GetPathFromFullPath(file);
                QuestStorage.GetSingleton().Output(Path.Combine(fullPath, file + "_questcache.sql"));
                CreatureStorage.GetSingleton().Output(Path.Combine(fullPath, file + "_creaturecache.sql"));
                GameObjectStorage.GetSingleton().Output(Path.Combine(fullPath, file + "_gameobjectcache.sql"));
                CreatureTemplateUpdateStorage.GetSingleton().Output(Path.Combine(fullPath, file + "_creaturecacheupdates.sql"));
                CreatureSpawnStorage css = CreatureSpawnStorage.GetSingleton();
                GameObjectSpawnStorage gss = GameObjectSpawnStorage.GetSingleton();
                Dictionary<int, Dictionary<Guid, WowObject>> dict = ObjectHandler.Objects;
                foreach (int map in dict.Keys)
                {
                    Dictionary<Guid, WowObject> objectsInMap = dict[map];
                    foreach (Guid guid in objectsInMap.Keys)
                    {
                        WowObject obj = objectsInMap[guid];
                        if (obj.Type == ObjectType.Unit)
                        {
                            CreatureSpawn spawn = new CreatureSpawn();
                            spawn.Entry = guid.GetEntry();
                            spawn.Map = map;
                            spawn.X = obj.Position.X;
                            spawn.Y = obj.Position.Y;
                            spawn.Z = obj.Position.Z;
                            spawn.O = obj.Movement.Orientation;
                            css.Add(spawn);
                        }
                        else if (obj.Type == ObjectType.GameObject)
                        {
                            GameObjectSpawn spawn = new GameObjectSpawn();
                            spawn.Entry = guid.GetEntry();
                            spawn.Map = map;
                            spawn.X = obj.Position.X;
                            spawn.Y = obj.Position.Y;
                            spawn.Z = obj.Position.Z;
                            spawn.O = obj.Movement.Orientation;
                            gss.Add(spawn);
                        }
                    }
                }
                css.Output(Path.Combine(fullPath, file + "_creaturesniffedspawns.sql"));
                gss.Output(Path.Combine(fullPath, file + "_gameobjectsniffedspawns.sql"));
            }
        }

        public static void PrintUsage(string error)
        {
            var n = Environment.NewLine;

            if (!string.IsNullOrEmpty(error))
                Console.WriteLine(error + n);

            var usage = "Usage: SilinoronParser -file <input file> -loader <loader type> " +
                "[-nodump <boolean>] [-nohex <boolean>] [-tosql <boolean>] [-skiplarge <boolean>]" + n + n +
                "-file\t\tThe file to read packets from." + n +
                "-loader\t\tThe loader to use (zor4xx/tiawps/izidor/synric)." + n +
                "-nodump\t\tSet to True to disable file logging." + n +
                "-nohex\t\tSet to True to not print out hex dumps." + n +
                "-tosql\t\tSet to True to output SQL dumps." + n +
                "-skiplarge\t\tSet to True to avoid printing out LARGE hex dumps.";

            Console.WriteLine(usage);
        }
    }
}
