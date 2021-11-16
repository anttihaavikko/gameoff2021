using System.Collections;
using System.Linq;
using Actions;
using AnttiStarterKit.Extensions;
using Save;
using UnityEngine;

namespace Curses
{
    public class PusherCurse : Curse
    {
        private readonly Meanie meanie;
        private int currentSpot = 2;
        private readonly GridChecker grid;

        private readonly int[] spots = { 0, 1, 2, 3, 4 };
        
        public PusherCurse(Meanie meanie)
        {
            this.meanie = meanie;
            MoveMeanie(1f);
            grid = new GridChecker();
        }

        public override IEnumerator Apply(Field field)
        {
            var from = new Vector3(currentSpot, 3, 0);
            var cards = field.GetCardsOnAxisX(currentSpot)
                .OrderByDescending(c => (c.transform.position - from).magnitude).ToList();
            cards.ForEach(c => grid.AddToGrid(c));
            var amt = 0f;
            cards.ForEach(c =>
            {
                var start = c.GetMirroredCoordinates();
                grid.Set(null, start.x, start.y);
                var end = grid.GetFurthestNeighbour(start, Vector3.down);
                grid.Set(c, end.x, end.y);
                var diff = end.ToVector() - start.ToVector();
                field.Move(c, diff, Passive.None, true);
                amt += diff.magnitude;
            });

            if (amt > 0)
            {
                meanie.Push();
            }
            
            yield return new WaitForSeconds(0.35f);
            currentSpot = spots.Where(s => s != currentSpot).ToList().Random();
            MoveMeanie(Random.Range(0.7f, 0.9f));
            yield return new WaitForSeconds(0.5f);
        }

        public override string GetTutorial()
        {
            return "This stage has a bit of a (pest) problem! That (tiny bugger) is sure to cause some (grief)...";
        }

        private void MoveMeanie(float duration = 1f)
        {
            meanie.MoveTo(new Vector3(currentSpot - 2f, 2.663f, 0), duration);
        }
    }
}