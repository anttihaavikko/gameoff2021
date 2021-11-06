using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Utils;

namespace Save
{
    [Serializable]
    public class SaveData
    {
        public Deck deck;
        public List<Passive> passives;
        public int score;
        public int level;

        public SaveData()
        {
            deck = new Deck();
            passives = new List<Passive>();
        }

        public void Save()
        {
            Saver.Save(this);
        }

        public static SaveData LoadOrCreate()
        {
            return Saver.Exists() ? Saver.Load<SaveData>() : Create();
        }

        private static SaveData Create()
        {
            var data = new SaveData();

            for (var i = 0; i < 5; i++)
            {
                data.deck.Add(CardData.Starter());
            }

            return data;
        }

        public bool HasPassive(Passive passive)
        {
            return passives.Contains(passive);
        }

        public int GetPassiveLevel(Passive passive)
        {
            return passives.Count(p => p == passive);
        }
    }
}