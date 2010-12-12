using System;
using System.Collections.Generic;
using System.IO;

namespace SilinoronParser.SQLOutput
{
    public sealed class CreatureStorage : SQLStorage<Creature>
    {
        private static readonly CreatureStorage instance = new CreatureStorage();
        public static CreatureStorage GetSingleton() { return instance; }
        private CreatureStorage() { }
        private Dictionary<int, Creature> creatures = new Dictionary<int, Creature>();

        public override void Add(Creature entry)
        {
            if (creatures.ContainsKey(entry.Entry))
                creatures[entry.Entry] = entry;
            else
                creatures.Add(entry.Entry, entry);
        }

        public override void Output(string toFile)
        {
            TextWriter tw = new StreamWriter(toFile);
            foreach (Creature creature in creatures.Values)
                tw.WriteLine(creature.ToSQL());
            tw.Close();
        }
    }
}
