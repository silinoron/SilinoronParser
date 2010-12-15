using System.Collections.Generic;
using System.IO;

namespace SilinoronParser.SQLOutput
{
    public sealed class CreatureTemplateUpdate
    {
        public uint Entry { get; private set; }
        private readonly Dictionary<string, string> updates = new Dictionary<string, string>();
        public CreatureTemplateUpdate(uint entry, Dictionary<string, string> updatesIn)
        {
            Entry = entry;
            foreach (var update in updatesIn)
                updates.Add(update.Key, update.Value);
        }

        public Dictionary<string, string>.Enumerator GetUpdates()
        {
            return updates.GetEnumerator();
        }

        public CreatureTemplateUpdate(uint entry, string fieldName, string value)
        {
            Entry = entry;
            updates.Add(fieldName, value);
        }

        public bool HasUpdateForField(string field)
        {
            return updates.ContainsKey(field);
        }

        public void AddUpdate(string field, string value)
        {
            if (updates.ContainsKey(field))
                updates[field] = value;
            else
                updates.Add(field, value);
        }

        public string ToSQL()
        {
            string sql = "UPDATE creature_template SET ";
            foreach (var update in updates)
                sql += update.Key + "=" + update.Value + ", ";
            sql = sql.Substring(0, sql.Length - 2);
            sql += " WHERE entry=" + Entry + ";";
            return sql;
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
                for (Dictionary<string, string>.Enumerator e = entry.GetUpdates(); e.MoveNext(); )
                {
                    string fieldName = e.Current.Key;
                    string value = e.Current.Value;
                    updates[entry.Entry].AddUpdate(fieldName, value);
                }
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
