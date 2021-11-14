using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Save;
using UnityEngine;

namespace Actions
{
    public class PullAction : BaseAction
    {
        private readonly List<Card> cards;
        private readonly Card puller;
        private readonly GridChecker grid;
        private readonly bool mega;

        public PullAction(IEnumerable<Card> cards, Card puller, bool mega = false)
        {
            grid = new GridChecker();
            grid.AddToGrid(puller);
            
            this.cards = cards.ToList();
            this.puller = puller;
            this.mega = mega;
        }
        
        public override IEnumerator Activate(Field field)
        {
            field.OnCardActivation(puller);
            var sorted = cards.Where(c => c != null)
                .OrderBy(c => (c.transform.position - puller.transform.position).magnitude).ToList();
            sorted.ForEach(c => grid.AddToGrid(c));
            sorted.ForEach(c => PullSingle(field, c));
            yield return new WaitForSeconds(0.35f);
        }
        
        private void PullSingle(Field field, Card c)
        {
            var start = c.GetMirroredCoordinates();
            grid.Set(null, start.x, start.y);
            var dir = (puller.transform.position - c.transform.position).normalized;

            if (!mega)
            {
                field.Move(c, dir, Passive.StarOnPull, mega);
                return;
            }
            
            var end = grid.GetFurthestNeighbour(start, dir);
            grid.Set(c, end.x, end.y);
            field.Move(c, end.ToVector() - start.ToVector(), Passive.StarOnPull, mega);
        }

        public override IEnumerable<Card> GetCards()
        {
            return cards;
        }
    }
}