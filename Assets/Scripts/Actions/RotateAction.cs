using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Actions
{
    public class RotateAction : BaseAction
    {
        private readonly List<Card> cards;
        private readonly int multiplier;
        private readonly bool clockwise;
        
        public RotateAction(IEnumerable<Card> cards, int multiplier, bool clockwise)
        {
            this.cards = cards.ToList();
            this.multiplier = multiplier;
            this.clockwise = clockwise;
        }
        
        public override IEnumerator Activate(Field field)
        {
            cards.ForEach(c => field.Rotate(c, clockwise));
            yield return new WaitForSeconds(0.35f);
        }
    }
}