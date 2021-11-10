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
        public List<int> bombs;
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
                bombs = new List<int>(),
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
                bombs = bombs.ToList(),
                type = type,
                directions = directions.ToList()
            };
        }

        public static CardData GetRandom(float luckFactor = 1f)
        {
            var c = Starter();

            for (var i = 0; i < GetExtraPipCount(); i++)
            {
                c.AddPip(luckFactor);
            }

            if (Random.value * luckFactor < 0.1f)
            {
                c.AddStarOrBomb();
            }
            
            if (Random.value * luckFactor < 0.2f)
            {
                c.RemovePip();
            }
            
            if (Random.value * luckFactor < 0.2f)
            {
                MakeSpecial(luckFactor, c);
            }

            return c;
        }

        private static void MakeSpecial(float luckFactor, CardData c)
        {
            c.pips.Clear();
            c.stars.Clear();
            c.bombs.Clear();
            c.type = new [] { CardType.RotateRight, CardType.RotateLeft, CardType.Push, CardType.Pull }.Random();
            // c.type = new [] { CardType.RotateRight, CardType.RotateLeft }.Random();
            c.directions = GetRandomDirections(luckFactor);
        }

        private void AddStarOrBomb()
        {
            if (Random.value < 0.5f)
            {
                AddStar();
                return;
            }

            AddBomb();
        }

        private static List<int> GetRandomDirections(float luckFactor = 1f)
        {
            var all = new List<int> { 0, 1, 2, 3 }.OrderBy(_ => Random.value);
            return all.Take(GetDirectionCount(luckFactor)).ToList();
        }

        private static int GetDirectionCount(float luckFactor = 1f)
        {
            var roll = Random.value * luckFactor;
            if (roll < 0.01f) return 4;
            if (roll < 0.07f) return 3;
            if (roll < 0.2f) return 2;
            return 1;
        }
        
        private static int GetExtraPipCount(float luckFactor = 1f)
        {
            var roll = Random.value * luckFactor;
            if (roll < 0.01f) return 5;
            if (roll < 0.05f) return 4;
            if (roll < 0.2f) return 3;
            if (roll < 0.4f) return 2;
            if (roll < 0.6f) return 1;
            return 0;
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
            bombs = bombs.Select(p => RotatePip(p, rotations)).ToList();
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

        private void AddPip(float luck)
        {
            var index = Random.Range(0, 9);
            if (!pips.Contains(index))
            {
                pips.Add(index);

                if (Random.value * luck < 0.1f)
                {
                    var makeStar = Random.value < 0.6f;
                    if (makeStar)
                    {
                        stars.Add(index);
                        return;
                    }
                    
                    bombs.Add(index);
                }
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
        
        private void AddBomb()
        {
            var index = Random.Range(0, 9);
            if (!bombs.Contains(index))
            {
                bombs.Add(index);
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