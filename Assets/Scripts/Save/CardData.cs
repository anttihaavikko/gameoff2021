using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Extensions;

namespace Save
{
    [System.Serializable]
    public class CardData
    {
        public List<int> pips;
        public List<int> stars;
        
        public static CardData Starter()
        {
            return new CardData
            {
                pips = GetBase().ToList(),
                stars = new List<int>()
            };
        }

        public static CardData Random()
        {
            var c = Starter();

            if (UnityEngine.Random.value < 0.5f)
            {
                c.AddPip();
            }

            return c;
        }
        
        private static int[] GetBase()
        {
            return new []
            {
                new []{ 0, 1, 2, 4 },
                new []{ 0, 1, 4, 7 },
                new []{ 2, 1, 4, 7 },
                new []{ 0, 1, 3, 4 },
                new []{ 0, 1, 4, 5 },
                new []{ 1, 2, 3, 4 },
                new []{ 3, 4, 5, 7 }
            }.Random();
        }

        private void AddPip()
        {
            var index = UnityEngine.Random.Range(0, 9);
            if (!pips.Contains(index))
            {
                pips.Add(index);
            }
        }
    }
}