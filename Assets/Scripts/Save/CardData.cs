using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Save
{
    [Serializable]
    public class CardData
    {
        public List<int> pips;
        public List<int> stars;
        public CardType type;
        public List<int> directions;

        public bool IsRotator => type == CardType.RotateLeft || type == CardType.RotateRight;
        public bool RotatesClockwise => type == CardType.RotateRight;
        
        public static CardData Starter()
        {
            var data = new CardData
            {
                pips = GetBase().ToList(),
                stars = new List<int>(),
                type = CardType.Normal,
                directions = new List<int>()
            };
            
            data.RotateRandomly();

            return data;
        }

        public CardData Clone()
        {
            return new CardData
            {
                pips = pips.ToList(),
                stars = stars.ToList(),
                type = type,
                directions = directions.ToList()
            };
        }

        public static CardData GetRandom()
        {
            var c = Starter();

            if (Random.value < 0.5f)
            {
                c.AddPip();
            }
            
            if (Random.value < 0.2f)
            {
                c.AddPip();
            }
            
            if (Random.value < 0.1f)
            {
                c.AddStar();
            }
            
            if (Random.value < 0.2f)
            {
                c.RemovePip();
            }
            
            if (Random.value < 0.2f)
            {
                c.pips.Clear();
                c.stars.Clear();
                c.type = Random.value < 0.5f ?  CardType.RotateRight : CardType.RotateLeft;
                c.directions = GetRandomDirections();
            }

            return c;
        }

        private static List<int> GetRandomDirections()
        {
            var all = new List<int> { 0, 1, 2, 3 }.OrderBy(_ => Random.value);
            return all.Take(GetDirectionCount()).ToList();
        }

        private static int GetDirectionCount()
        {
            var roll = Random.value;
            if (roll < 0.01f) return 4;
            if (roll < 0.07f) return 3;
            if (roll < 0.2f) return 2;
            return 1;
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

        private void RotateRandomly()
        {
            var rotations = Random.Range(0, 4);
            pips = pips.Select(p => RotatePip(p, rotations)).ToList();
        }

        public void Rotate(int direction)
        {
            var rotations = direction > 0 ? 1 : 3;
            pips = pips.Select(p => RotatePip(p, rotations)).ToList();
            stars = stars.Select(p => RotatePip(p, rotations)).ToList();
        }
        
        private static int RotatePip(int index, int times)
        {
            for (var i = 0; i < times; i++)
            {
                index = RotatePip(index);
            }

            return index;
        }

        private static int RotatePip(int index)
        {
            return index switch
            {
                0 => 2,
                1 => 5,
                2 => 8,
                5 => 7,
                8 => 6,
                7 => 3,
                6 => 0,
                3 => 1,
                4 => 4,
                _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
            };
        }

        private void AddPip()
        {
            var index = Random.Range(0, 9);
            if (!pips.Contains(index))
            {
                pips.Add(index);
            }
        }

        private void RemovePip()
        {
            if (!pips.Any()) return;
            
            var index = Random.Range(0, pips.Count);
            pips.RemoveAt(index);
        }
        
        private void AddStar()
        {
            var index = Random.Range(0, 9);
            if (!stars.Contains(index))
            {
                stars.Add(index);
            }
        }
    }

    public enum CardType
    {
        Normal,
        RotateRight,
        RotateLeft,
        Push,
        Pull
    }
}