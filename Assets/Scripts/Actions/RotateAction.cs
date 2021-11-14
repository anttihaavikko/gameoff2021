using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Actions
{
    public class RotateAction : BaseAction
    {
        private readonly List<Card> cards;
        private readonly bool clockwise;
        private readonly Card card;
        
        public RotateAction(IEnumerable<Card> cards, bool clockwise, Card card)
        {
            this.cards = cards.ToList();
            this.clockwise = clockwise;
            this.card = card;
        }
        
        public override IEnumerator Activate(Field field)
        {
            field.OnCardActivation(card);
            cards.ForEach(c => field.Rotate(c, clockwise));
            yield return new WaitForSeconds(0.35f);
        }

        public override IEnumerable<Card> GetCards()
        {
            return cards;
        }
    }
}