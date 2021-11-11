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
        private readonly bool mega;
        private readonly GridChecker grid;

        public PushAction(IEnumerable<Card> cards, Vector3 from, bool mega = false)
        {
            grid = new GridChecker();
            this.cards = cards.ToList();
            this.from = from;
            this.mega = mega;
        }
        
        public override IEnumerator Activate(Field field)
        {
            var sorted = cards.OrderByDescending(c => (c.transform.position - from).magnitude).ToList();
            sorted.ForEach(c => grid.AddToGrid(c));
            sorted.ForEach(c => PushSingle(field, c));
            yield return new WaitForSeconds(0.35f);
        }

        private void PushSingle(Field field, Card c)
        {
            var start = c.GetMirroredCoordinates();
            grid.Set(null, start.x, start.y);
            var dir = (c.transform.position - from).normalized;

            if (!mega)
            {
                field.Move(c, dir, Passive.StarOnPush, mega);
                return;
            }
            
            var end = grid.GetFurthestNeighbour(start, dir);
            grid.Set(c, end.x, end.y);
            field.Move(c, end.ToVector() - start.ToVector(), Passive.StarOnPush, mega);
        }

        public override IEnumerable<Card> GetCards()
        {
            return cards;
        }
    }
}