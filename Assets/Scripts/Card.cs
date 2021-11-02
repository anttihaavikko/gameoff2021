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
    [SerializeField] private Sprite starSprite;

    private List<Pip> pips;

    private void Start()
    {
        pips = points.Where(_ => Random.value < 0.5f).Select(p =>
        {
            var i = points.IndexOf(p);
            p.gameObject.SetActive(true);
            
            var isStar = Random.value < 0.1f;
            if (isStar)
            {
                p.sprite = starSprite;
                p.transform.Rotate(0, 0, Random.Range(0f, 360f));
            }
            
            return new Pip(p, i % 3, i / 3, isStar);
        }).ToList();
        
        Debug.Log($"Card has {pips.Count} pips");
    }

    public IEnumerable<Pip> GetPoints()
    {
        var pos = GetBasePosition();
        pips.ForEach(p => p.AddBase(pos));
        return pips;
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
    public bool isStar;

    public Pip(SpriteRenderer sprite, int x, int y, bool isStar)
    {
        this.sprite = sprite;
        this.x = x;
        this.y = y;
        this.isStar = isStar;
    }

    public float GetDistanceTo(Pip pip)
    {
        return Mathf.Pow(pip.x - x, 2) + Mathf.Pow(pip.y - y, 2);
    }
    
    public void AddBase(IntPair offset)
    {
        x += offset.x;
        y += offset.y;
    }
}