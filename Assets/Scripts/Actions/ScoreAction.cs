using System.Collections;
using System.Collections.Generic;

namespace Actions
{
    public class ScoreAction : BaseAction
    {
        private readonly Card card;
        private readonly int multiplier;
        
        public ScoreAction(Card card, int multiplier)
        {
            this.card = card;
            this.multiplier = multiplier;
        }
        
        public override IEnumerator Activate(Field field)
        {
            yield return field.ScoreCard(card, multiplier);
        }

        public override IEnumerable<Card> GetCards()
        {
            return new[] { card };
        }
    }
}