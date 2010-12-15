using System.Collections.Generic;
using System.IO;

namespace SilinoronParser.SQLOutput
{
    public sealed class CreatureTemplateUpdate
    {
        public string Update { get; private set; }
        public uint Entry { get; private set; }
        public CreatureTemplateUpdate(uint entry, string update)
        {
            Entry = entry;
            Update = update;
        }

        public string ToSQL()
        {
            return "UPDATE creature_template SET " + Update + " WHERE entry=" + Entry + ";";
        }
    }

    public sealed class CreatureTemplateUpdateStorage : SQLStorage<CreatureTemplateUpdate>
    {
        private static readonly CreatureTemplateUpdateStorage instance = new CreatureTemplateUpdateStorage();
        public static CreatureTemplateUpdateStorage GetSingleton() { return instance; }
        private Dictionary<uint, CreatureTemplateUpdate> updates = new Dictionary<uint, CreatureTemplateUpdate>();

        public override void Add(CreatureTemplateUpdate entry)
        {
            if (updates.ContainsKey(entry.Entry))
            {
                CreatureTemplateUpdate oldEntry = updates[entry.Entry];
                if (oldEntry.Update.Contains(entry.Update))
                    return;
                string newVal = entry.Update + ", " + oldEntry.Update;
                CreatureTemplateUpdate newUpdate = new CreatureTemplateUpdate(entry.Entry, newVal);
                updates[entry.Entry] = newUpdate;
            }
            else
                updates.Add(entry.Entry, entry);
        }

        public override void Output(string toFile)
        {
            TextWriter tw = new StreamWriter(toFile);
            foreach (CreatureTemplateUpdate update in updates.Values)
                tw.WriteLine(update.ToSQL());
            tw.Close();
        }
    }
}
