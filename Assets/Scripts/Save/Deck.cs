using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Save
{
    [System.Serializable]
    public class Deck
    {
        public List<CardData> cards;

        private Queue<CardData> pile;
        
        public bool IsEmpty => !pile.Any();
        public List<CardData> Preview => pile.OrderBy(_ => Random.value).ToList();

        public Deck()
        {
            cards = new List<CardData>();
            pile = new Queue<CardData>();
        }

        public void Add(CardData card)
        {
            cards.Add(card);
        }

        public void Shuffle()
        {
            pile.Clear();
            cards.OrderBy(_ => Random.value).ToList().ForEach(c => pile.Enqueue(c));
        }

        public CardData Draw()
        {
            return pile.Dequeue();
        }

        public int GetCount()
        {
            return pile.Count;
        }
    }
}