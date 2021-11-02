using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Card : MonoBehaviour
{
    public Draggable draggable;
    
    [SerializeField] private List<SpriteRenderer> points;

    private void Start()
    {
        points.ForEach(p => p.gameObject.SetActive(Random.value < 0.5f));
    }

    public IEnumerable<IntPair> GetPoints()
    {
        return points.Where(p => p.gameObject.activeSelf).Select(p =>
        {
            var i = points.IndexOf(p);
            return new IntPair(i % 3, i / 3);
        });
    }
}

public class IntPair
{
    public int x, y;

    public IntPair(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}