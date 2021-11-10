using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Actions
{
    public class ActivateAction : BaseAction
    {
        private readonly Card card;
        private readonly int multiplier;
        
        public ActivateAction(Card card, int multiplier)
        {
            this.card = card;
            this.multiplier = multiplier;
            card.ShowQueuedActivation();
        }
        
        public override IEnumerator Activate(Field field)
        {
            field.Activate(card, multiplier);
            yield break;
        }

        public override IEnumerable<Card> GetCards()
        {
            return new[] { card };
        }
    }
}