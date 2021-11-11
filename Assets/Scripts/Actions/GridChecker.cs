using UnityEngine;

namespace Actions
{
    public class GridChecker
    {
        private readonly TileGrid<Card> grid;

        public GridChecker()
        {
            grid = new TileGrid<Card>(5, 5);
        }
        
        public void AddToGrid(Card card)
        {
            var p = card.GetMirroredCoordinates();
            grid.Set(card, p.x, p.y);
        }

        public void Set(Card card, int x, int y)
        {
            grid.Set(card, x, y);
        }
        
        public IntPair GetFurthestNeighbour(IntPair p, Vector3 dir)
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
    }
}