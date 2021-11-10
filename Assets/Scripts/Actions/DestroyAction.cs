using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Actions
{
    public class DestroyAction : BaseAction
    {
        private readonly List<Card> cards;
        private int multiplier;
        
        public DestroyAction(Card card, int multiplier)
        {
            cards = new List<Card>{ card };
            this.multiplier = multiplier;
        }

        public DestroyAction(IEnumerable<Card> cards, int multiplier)
        {
            this.cards = cards.ToList();
        }
        
        public override IEnumerator Activate(Field field)
        {
            cards.ForEach(c => field.RemoveCard(c, multiplier));
            yield return new WaitForSeconds(0.5f);
        }

        public override IEnumerable<Card> GetCards()
        {
            return cards;
        }
    }
}