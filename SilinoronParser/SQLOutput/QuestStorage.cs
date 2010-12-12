using System;
using System.Collections.Generic;
using System.IO;

namespace SilinoronParser.SQLOutput
{
    public sealed class QuestStorage : SQLStorage<Quest>
    {
        private static readonly QuestStorage instance = new QuestStorage();
        public static QuestStorage GetSingleton() { return instance; }
        private QuestStorage() { }
        private Dictionary<int, Quest> quests = new Dictionary<int, Quest>();

        public override void Add(Quest entry)
        {
            if (quests.ContainsKey(entry.Entry))
                quests[entry.Entry] = entry;
            else
                quests.Add(entry.Entry, entry);
        }

        public override void Output(string toFile)
        {
            TextWriter tw = new StreamWriter(toFile);
            foreach (Quest quest in quests.Values)
                tw.WriteLine(quest.ToSQL());
            tw.Close();
        }
    }
}
