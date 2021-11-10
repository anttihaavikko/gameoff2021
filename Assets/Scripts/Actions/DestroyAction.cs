using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Actions
{
    public class DestroyAction : BaseAction
    {
        private readonly List<Card> cards;
        
        public DestroyAction(Card card)
        {
            cards = new List<Card>{ card };
        }

        public DestroyAction(IEnumerable<Card> cards)
        {
            this.cards = cards.ToList();
        }
        
        public override IEnumerator Activate(Field field)
        {
            cards.ForEach(field.RemoveCard);
            yield return new WaitForSeconds(0.5f);
        }

        public override IEnumerable<Card> GetCards()
        {
            return cards;
        }
    }
}