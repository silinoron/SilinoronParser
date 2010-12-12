using System.IO;
using System.Collections.Generic;

namespace SilinoronParser.SQLOutput
{
    public sealed class CreatureSpawn
    {
        public uint Entry { get; set; }
        public int Map { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float O { get; set; }
        public string ToSQL()
        {
            string sql = "REPLACE INTO creaturesniffedspawns VALUES (";
            sql += "NULL, "; // guid, let it auto_increment
            sql += Entry + ", ";
            sql += Map + ", ";
            sql += X + ", ";
            sql += Y + ", ";
            sql += Z + ", ";
            sql += O + ");";
            return sql;
        }
    }

    public sealed class GameObjectSpawn
    {
        public uint Entry { get; set; }
        public int Map { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float O { get; set; }
        public string ToSQL()
        {
            string sql = "REPLACE INTO gameobjectsniffedspawns VALUES (";
            sql += "NULL, "; // guid, let it auto-increment
            sql += Entry + ", ";
            sql += Map + ", ";
            sql += X + ", ";
            sql += Y + ", ";
            sql += Z + ", ";
            sql += O + ");";
            return sql;
        }
    }

    public sealed class CreatureSpawnStorage : SQLStorage<CreatureSpawn>
    {
        private static readonly CreatureSpawnStorage instance = new CreatureSpawnStorage();
        public static CreatureSpawnStorage GetSingleton() { return instance; }
        private CreatureSpawnStorage() { }
        private List<CreatureSpawn> spawns = new List<CreatureSpawn>();

        public override void Add(CreatureSpawn entry)
        {
            if (entry.Entry != 0)
                spawns.Add(entry);
        }

        public override void Output(string toFile)
        {
            TextWriter tw = new StreamWriter(toFile);
            foreach (CreatureSpawn spawn in spawns)
                tw.WriteLine(spawn.ToSQL());
            tw.Close();
        }
    }

    public sealed class GameObjectSpawnStorage : SQLStorage<GameObjectSpawn>
    {
        private static readonly GameObjectSpawnStorage instance = new GameObjectSpawnStorage();
        public static GameObjectSpawnStorage GetSingleton() { return instance; }
        private GameObjectSpawnStorage() { }
        private List<GameObjectSpawn> spawns = new List<GameObjectSpawn>();

        public override void Add(GameObjectSpawn entry)
        {
            if (entry.Entry != 0)
                spawns.Add(entry);
        }

        public override void Output(string toFile)
        {
            TextWriter tw = new StreamWriter(toFile);
            foreach (GameObjectSpawn spawn in spawns)
                tw.WriteLine(spawn.ToSQL());
            tw.Close();
        }
    }
}
