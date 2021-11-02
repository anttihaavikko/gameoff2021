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

    public IEnumerable<Pip> GetPoints()
    {
        var pos = GetBasePosition();
        
        return points.Where(p => p.gameObject.activeSelf).Select(p =>
        {
            var i = points.IndexOf(p);
            return new Pip(p, i % 3 + pos.x, i / 3 + pos.y);
        });
    }

    public IntPair GetBasePosition()
    {
        var p = transform.position;
        return new IntPair(((int)p.x + 2) * 3, (-(int)p.y + 2) * 3);
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

public class Pip
{
    public SpriteRenderer sprite;
    public int x, y;

    public Pip(SpriteRenderer sprite, int x, int y)
    {
        this.sprite = sprite;
        this.x = x;
        this.y = y;
    }

    public float GetDistanceTo(Pip pip)
    {
        return Mathf.Pow(pip.x - x, 2) + Mathf.Pow(pip.y - y, 2);
    }
}