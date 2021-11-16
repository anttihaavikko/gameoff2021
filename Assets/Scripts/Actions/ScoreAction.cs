using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Actions
{
    public class ScoreAction : BaseAction
    {
        public override bool IsPriority => true;
        
        private readonly Card card;
        private readonly int multiplier;
        
        public ScoreAction(Card card, int multiplier)
        {
            this.card = card;
            this.multiplier = multiplier;
        }
        
        public override IEnumerator Activate(Field field)
        {
            card.SetBorderColorTo(Color.black);
            yield return field.ScoreCard(card, multiplier);
        }

        public override IEnumerable<Card> GetCards()
        {
            return new[] { card };
        }
    }
}