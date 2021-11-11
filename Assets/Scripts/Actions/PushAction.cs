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
        private TileGrid<Card> grid;

        public PushAction(IEnumerable<Card> cards, Vector3 from, bool mega = false)
        {
            this.cards = cards.ToList();
            this.from = from;
            this.mega = mega;
        }
        
        public override IEnumerator Activate(Field field)
        {
            grid = new TileGrid<Card>(5, 5);
            var sorted = cards.OrderByDescending(c => (c.transform.position - from).magnitude).ToList();
            sorted.ForEach(AddToGrid);
            sorted.ForEach(c => PushSingle(field, c));
            yield return new WaitForSeconds(0.35f);
        }

        private void AddToGrid(Card card)
        {
            var p = card.GetMirroredCoordinates();
            grid.Set(card, p.x, p.y);
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
            
            var end = GetFurthestNeighbour(start, dir);
            grid.Set(c, end.x, end.y);
            field.Move(c, end.ToVector() - start.ToVector(), Passive.StarOnPush, mega);
        }

        private IntPair GetFurthestNeighbour(IntPair p, Vector3 dir)
        {
            var safe = 0;
            
            while (true)
            {
                var next = new IntPair(p.x + Mathf.RoundToInt(dir.x), p.y + Mathf.RoundToInt(dir.y));

                if (!grid.IsInBounds(next.x, next.y) || grid.Get(next.x, next.y) != default)
                {
                    return p;
                }
                
                p = next;
                safe++;

                if (safe > 100) return p;
            }
        }

        public override IEnumerable<Card> GetCards()
        {
            return cards;
        }
    }
}