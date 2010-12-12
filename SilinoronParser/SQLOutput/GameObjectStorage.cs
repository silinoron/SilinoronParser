using System.Collections.Generic;
using System.IO;

namespace SilinoronParser.SQLOutput
{
    public sealed class GameObjectStorage : SQLStorage<GameObject>
    {
        private static readonly GameObjectStorage instance = new GameObjectStorage();
        public static GameObjectStorage GetSingleton() { return instance; }
        private GameObjectStorage() { }
        private Dictionary<int, GameObject> gameobjects = new Dictionary<int, GameObject>();

        public override void Add(GameObject entry)
        {
            if (gameobjects.ContainsKey(entry.Entry))
                gameobjects[entry.Entry] = entry;
            else
                gameobjects.Add(entry.Entry, entry);
        }

        public override void Output(string toFile)
        {
            TextWriter tw = new StreamWriter(toFile);
            foreach (GameObject gameobject in gameobjects.Values)
                tw.WriteLine(gameobject.ToSQL());
            tw.Close();
        }
    }
}
