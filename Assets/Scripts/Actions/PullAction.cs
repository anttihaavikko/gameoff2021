using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Actions
{
    public class PullAction : BaseAction
    {
        private readonly List<Card> cards;
        private readonly Vector3 target;

        public PullAction(IEnumerable<Card> cards, Vector3 target)
        {
            this.cards = cards.ToList();
            this.target = target;
        }
        
        public override IEnumerator Activate(Field field)
        {
            cards.ForEach(c => field.Move(c, (target - c.transform.position).normalized));
            yield return new WaitForSeconds(0.35f);
        }

        public override IEnumerable<Card> GetCards()
        {
            return cards;
        }
    }
}