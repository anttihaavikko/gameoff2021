using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Utils;
using Random = UnityEngine.Random;

namespace Save
{
    [Serializable]
    public class SaveData
    {
        public Deck deck;
        public List<Passive> passives;
        public int score;
        public int level;
        public int seed;
        public string daily;

        public bool IsDaily => daily != default;

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
            
            data.seed = Random.Range(1, 99999);

            return data;
        }

        public static SaveData Create(List<CardData> cards, List<Passive> passives, int seed, string daily)
        {
            var data = new SaveData();
            cards.ForEach(c => data.deck.Add(c));
            passives.ForEach(p => data.passives.Add(p));
            data.seed = seed;
            data.daily = daily;
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

        public void ApplySeed()
        {
            Random.InitState(seed + level * 66);
        }
    }
}