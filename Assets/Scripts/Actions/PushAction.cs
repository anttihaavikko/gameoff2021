using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Save;
using UnityEngine;

namespace Actions
{
    public class PushAction : BaseAction
    {
        private readonly List<Card> cards;
        private readonly Vector3 from;

        public PushAction(IEnumerable<Card> cards, Vector3 from)
        {
            this.cards = cards.ToList();
            this.from = from;
        }
        
        public override IEnumerator Activate(Field field)
        {
            cards.ForEach(c => field.Move(c, c.transform.position - from, Passive.StarOnPush));
            yield return new WaitForSeconds(0.35f);
        }

        public override IEnumerable<Card> GetCards()
        {
            return cards;
        }
    }
}