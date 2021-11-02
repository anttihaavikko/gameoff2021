using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Extensions;
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
        var rotates = Random.Range(0, 4);
        var pipIds = GetBasePips().Select(i => RotateIndex(i, rotates));
        pips = points.Where((_, i) => pipIds.Contains(i)).Select(p =>
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
        
        // Debug.Log($"Card has {pips.Count} pips");
    }

    private static int[] GetBasePips()
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

    private int RotateIndex(int index, int times)
    {
        for (var i = 0; i < times; i++)
        {
            index = RotateIndex(index);
        }

        return index;
    }

    private int RotateIndex(int index)
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

    public IEnumerable<Pip> GetPoints(bool addBase = false)
    {
        if (addBase)
        {
            var pos = GetBasePosition();
            pips.ForEach(p => p.AddBase(pos));    
        }
        
        return pips;
    }

    private IntPair GetBasePosition()
    {
        var p = transform.position;
        return GetBasePositionFor(p);
    }

    public IntPair GetBasePositionFor(Vector3 p)
    {
        return new IntPair(((int)p.x + 2) * 3, (-(int)p.y + 2) * 3);
    }

    public IEnumerable<Transform> GetAllPips()
    {
        return points.Select(p => p.transform);
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