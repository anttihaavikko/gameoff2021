using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Managers;
using Save;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class Card : MonoBehaviour
{
    public Draggable draggable;
    public CardHover hoverer;
    
    [SerializeField] private List<SpriteRenderer> points;
    [SerializeField] private Sprite starSprite, bombSprite, circleSprite;
    [SerializeField] private Transform visualContents;
    [SerializeField] private GameObject rotateIcon, pushIcon, pullIcon;
    [SerializeField] private List<GameObject> directionIndicators;
    [SerializeField] private SpriteRenderer borderSprite;
    [SerializeField] private Color markColor;
    [SerializeField] private GameObject marking;

    private List<Pip> pips;
    private CardData data;
    private List<Pip> shakingPips;
    private bool shaking;
    private Vector3 originalPos;

    public bool IsRotator => data.IsRotator;
    public bool RotatesClockwise => data.RotatesClockwise;
    public bool IsPusher => data.type == CardType.Push;
    public bool IsPuller => data.type == CardType.Pull;
    
    public void Setup(CardData cardData)
    {
        shakingPips = new List<Pip>();
        data = cardData.Clone();

        if (data.IsRotator)
        {
            rotateIcon.SetActive(true);
            ActivateDirections();
        }
        
        if (data.type == CardType.RotateLeft)
        {
            rotateIcon.transform.Mirror();
        }

        if (data.type == CardType.Push)
        {
            pushIcon.SetActive(true);
            ActivateDirections();
        }
        
        if (data.type == CardType.Pull)
        {
            pullIcon.SetActive(true);
            ActivateDirections();
        }

        PositionPips();
    }

    private void ActivateDirections()
    {
        data.directions.ForEach(d => directionIndicators[d].SetActive(true));
    }

    public void Shake(bool state = true)
    {
        shaking = state;
        originalPos = transform.position;

        if (!state)
        {
            transform.position = originalPos;
        }
    }

    public Vector3 GetPositionBeforeShaking()
    {
        return originalPos;
    }

    private void Update()
    {
        if (shakingPips.Count > 0)
        {
            shakingPips.ForEach(p => p.Shake());
        }

        if (shaking)
        {
            transform.position = originalPos.RandomOffset(0.02f);
        }
    }

    public void ShakePip(Pip pip)
    {
        shakingPips.Add(pip);
    }

    public void StarPip(Pip pip)
    {
        data.stars.Add(pip.index);
        pip.sprite.sprite = starSprite;
        pip.isBomb = false;
        pip.isStar = true;

        if (pip.isShaking)
        {
            ResetBombs();
        }
    }
    
    public void BombPip(Pip pip)
    {
        data.bombs.Add(pip.index);
        pip.sprite.sprite = bombSprite;
        pip.isBomb = true;
        pip.isStar = false;
    }

    public void PositionPips()
    {
        points.ForEach(ResetPip);
        
        pips = points.Where((_, i) => data.pips.Contains(i)).Select(p =>
        {
            var i = points.IndexOf(p);
            p.gameObject.SetActive(true);

            var isStar = data.stars.Contains(i);
            if (isStar)
            {
                p.sprite = starSprite;
                p.transform.Rotate(0, 0, Random.Range(0f, 360f));
            }

            var isBomb = !isStar && data.bombs.Contains(i);
            if (isBomb)
            {
                p.sprite = bombSprite;
                p.transform.Rotate(0, 0, Random.Range(-40f, 40f));
            }

            return new Pip(i, this, p, i % 3, i / 3, isStar, isBomb);
        }).ToList();
    }

    private void ResetPip(SpriteRenderer sr)
    {
        sr.gameObject.SetActive(false);
        sr.sprite = circleSprite;
    }

    public void Rotate(bool clockWise)
    {
        AudioManager.Instance.PlayEffectAt(3, transform.position, 1f);
        var dir = clockWise ? 1 : -1;
        VisualRotate(dir);
        data.Rotate(dir);
        PositionPips();
    }

    private void VisualRotate(int dir)
    {
        if (pips.Any())
        {
            visualContents.rotation = Quaternion.Euler(0, 0, 90f * dir);
            Tweener.RotateToBounceOut(visualContents, Quaternion.identity, 0.3f);
            return;
        }

        var angle = visualContents.rotation.eulerAngles.z - dir * 90f;
        Tweener.RotateToBounceOut(visualContents, Quaternion.Euler(0, 0, angle), 0.3f);
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

    public void ShowQueuedActivation()
    {
        SetBorderColorTo(markColor);
    }

    public void SetBorderColorTo(Color color)
    {
        if (borderSprite)
        {
            borderSprite.color = color;
            // Tweener.ColorToQuad(borderSprite, color, 0.2f);   
        }
    }

    private IntPair GetBasePosition()
    {
        var p = transform.position;
        return GetBasePositionFor(p);
    }

    public IntPair GetCoordinates()
    {
        var p = transform.position;
        var coords = GetBasePositionFor(p);
        return new IntPair(coords.x / 3, coords.y / 3);
    }

    public IntPair GetMirroredCoordinates()
    {
        var coords = GetCoordinates();
        return new IntPair(coords.x, Mathf.Abs(4 - coords.y));
    }

    public IntPair GetBasePositionFor(Vector3 p)
    {
        return new IntPair(((int)p.x + 2) * 3, (-(int)p.y + 2) * 3);
    }

    public IEnumerable<Transform> GetAllPips()
    {
        return points.Select(p => p.transform);
    }

    public IEnumerable<Vector3> GetDirections(bool all = false)
    {
        var q = visualContents.rotation;
        var baseDirections = new List<Vector3> { Vector3.up, Vector3.right, Vector3.down, Vector3.left };

        return all ? baseDirections : data.directions.Select(i => q * baseDirections[i]);
    }

    public void Debug()
    {
        print(string.Join(",", data.bombs));
    }

    public void ResetBombs()
    {
        pips.ForEach(p => p.isShaking = false);
        shakingPips.Clear();
    }

    public Vector3 GetExplosionPosition()
    {
        return pips.Any(p => p.isBomb && p.isShaking) ? 
            pips.First(p => p.isBomb && p.isShaking).sprite.transform.position : 
            transform.position;
    }

    public void MakeRandomStar()
    {
        var options = pips.Where(p => !p.isStar).OrderBy(_ => Random.value).ToList();
        if (options.Any())
        {
            options.Random().MakeStar();
        }
    }

    public bool IsOnSameAxisAs(IntPair center, IEnumerable<Vector3> dirs)
    {
        var p = GetMirroredCoordinates();
        var diff = (Vector3)(p.ToVector() - center.ToVector()).normalized;
        return (p.x == center.x || p.y == center.y) && dirs.Any(p => AreVectorsClose(p, diff));
    }

    private static bool AreVectorsClose(Vector3 a, Vector3 b)
    {
        return Mathf.RoundToInt(a.x) == Mathf.RoundToInt(b.x) && Mathf.RoundToInt(a.y) == Mathf.RoundToInt(b.y);
    }

    public string GetInfo()
    {
        if (IsRotator)
        {
            return "That card (rotates) and (activates) the cards in the (marked) direction.";
        }
        
        if (IsPusher)
        {
            return "That card (pushes) and activates the cards in the (marked) direction.";
        }
        
        if (IsPuller)
        {
            return "That card (pulls) and (activates) the cards in the (marked) direction.";
        }

        var hasStars = pips.Any(p => p.isStar);
        var hasBombs = pips.Any(p => p.isBomb);

        if (hasStars && hasBombs)
        {
            return "Those (star) pips increase (multiplier) on activation. The (bomb) pips (explode) and destroy the card.";
        }

        if (hasStars)
        {
            return "Those (star) pips increase (multiplier) on activation.";
        }

        if (hasBombs)
        {
            return "Those (bomb) pips (explode) after two activations (destroying) the card.";
        }

        return null;
    }

    public bool IsOnSameAxisY(int y)
    {
        return GetMirroredCoordinates().y == y;
    }

    public bool IsOnSameAxisX(int x)
    {
        return GetMirroredCoordinates().x == x;
    }
    
    public void SetMarking(bool state)
    {
        marking.SetActive(state);
    }

    public void PlaceEffect()
    {
        EffectManager.AddEffects(new []{ 4 }, transform.position);
    }

    public void AddRandomStar()
    {
        if (!pips.Any()) return;
        var pip = pips.Random();
        StarPip(pip);
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

    public Vector2 ToVector()
    {
        return new Vector2(x, y);
    }
}

public class Pip
{
    public SpriteRenderer sprite;
    public int x, y;
    public bool isStar, isBomb;
    public bool isShaking;
    public int index;
    public int pathIndex;

    private Vector3 origin;
    private Card card;

    public Pip(int index, Card card, SpriteRenderer sprite, int x, int y, bool isStar, bool isBomb)
    {
        this.index = index;
        this.sprite = sprite;
        this.x = x;
        this.y = y;
        this.isStar = isStar;
        this.isBomb = isBomb;
        this.card = card;
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

    public void StartShaking()
    {
        origin = sprite.transform.position;
        card.ShakePip(this);
    }

    public void Shake()
    {
        sprite.transform.position = origin.RandomOffset(0.02f);
    }

    public Card GetCard()
    {
        return card;
    }

    public void MakeStar()
    {
        card.StarPip(this);
    }

    public void MakeBomb()
    {
        card.BombPip(this);
    }
}