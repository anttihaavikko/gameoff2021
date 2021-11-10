using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Actions;
using AnttiStarterKit.Animations;
using TMPro;
using UnityEngine;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Managers;
using AnttiStarterKit.Utils;
using AnttiStarterKit.Visuals;
using Save;
using Random = UnityEngine.Random;

public class Field : MonoBehaviour
{
    [SerializeField] private TMP_Text output, totalScoreField, levelField, parField;
    [SerializeField] private TMP_Text totalScoreFieldShadow, levelFieldShadow, parFieldShadow;
    [SerializeField] private TextWithBackground scorePopPrefab;
    [SerializeField] private ConnectionLines connectionLines;
    [SerializeField] private LayerMask cardLayer, fieldLayer;
    [SerializeField] private Hand hand;
    [SerializeField] private Appearer spinner;
    [SerializeField] private Color markColor;
    [SerializeField] private EffectCamera cam;

    private TileGrid<Pip> grid;
    private int totalScore;
    private ActionQueue actionQueue;

    private void Start()
    {
        cam.BaseEffect(0.1f);
        actionQueue = new ActionQueue();
        grid = new TileGrid<Pip>(15, 15);
        totalScore = hand.GetScore();
        UpdateScore();
        levelField.text = $"STAGE {hand.Level}";
        levelFieldShadow.text = levelField.text;
        parField.text = $"PAR {GetPar(hand.Level)}";
        parFieldShadow.text = parField.text;
    }

    private int GetPar(int level)
    {
        if (level == 1) return 30;
        var mod = Mathf.FloorToInt(level / 5f) + 1;
        return GetPar(level - 1) + level * 30 * mod;
    }

    private void UpdateScore()
    {
        totalScoreField.text = hand.GetScore().ToString();
        totalScoreFieldShadow.text = totalScoreField.text;
    }

    public void Place(Card card)
    {
        spinner.Show();
        connectionLines.Hide();

        PlacePipsToGrid(card);

        output.text = grid.DataAsString();
        
        hand.LockCards(true);
        
        actionQueue.Add(new ActivateAction(card, 1));
        StartCoroutine(ProcessQueue());
    }

    private IEnumerator ProcessQueue()
    {
        yield return actionQueue.Process(this);
        hand.NextTurn();
        spinner.Hide();
    }

    public void Activate(Card card, int multi = 1)
    {
        if (!card) return;
        var pos = card.GetCoordinates();
        
        card.SetBorderColorTo(Color.black);

        if (pos.y == 4)
        {
            multi += hand.GetPassiveLevel(Passive.MultiOnBottom);
        }
        
        if (pos.y == 0)
        {
            multi += hand.GetPassiveLevel(Passive.MultiOnTop);
        }
        
        if (pos.x == 0)
        {
            multi += hand.GetPassiveLevel(Passive.MultiOnLeft);
        }
        
        if (pos.x == 4)
        {
            multi += hand.GetPassiveLevel(Passive.MultiOnRight);
        }

        if (multi > 1)
        {
            ShowTextAt($"x{multi}", card.transform.position + Vector3.right * 0.3f);
        }
        
        actionQueue.Add(new ScoreAction(card, multi));

        if (card.IsRotator)
        {
            var neighbours = GetNeighboursFor(card).ToList();
            actionQueue.Add(new RotateAction(neighbours, card.RotatesClockwise));
            neighbours.ForEach(n => actionQueue.Add(new ActivateAction(n, multi + 1)));
        }

        if (card.IsPusher)
        {
            var p = card.transform.position;
            var neighbours = GetNeighboursFor(card).ToList();
            actionQueue.Add(new PushAction(neighbours, p));
            neighbours.ForEach(n => actionQueue.Add(new ActivateAction(n, multi + 1)));
        }
        
        if (card.IsPuller)
        {
            var p = card.transform.position;
            var neighbours = GetNeighboursFor(card, 2).ToList();
            actionQueue.Add(new PullAction(neighbours, p));
            neighbours.ForEach(n => actionQueue.Add(new ActivateAction(n, multi + 1)));
        }
    }

    public IEnumerator ScoreCard(Card card, int multi)
    {
        if (!card) yield break;
        var allVisited = new List<Pip>();
        var pips = card.GetPoints().ToList();
        foreach (var pip in pips)
        {
            if (allVisited.Contains(pip)) continue;

            var visited = new List<Pip>();
            Fill(pip, visited);
            visited = visited.Distinct().OrderBy(p => p.GetDistanceTo(pip)).ToList();
            yield return MarkCoroutine(visited, multi);
            allVisited.AddRange(visited);
        }
    }

    public void RemoveCard(Card card, int multiplier)
    {
        if (!card) return;
        EffectManager.AddEffects(new []{ 0, 1}, card.transform.position);
        EffectManager.AddEffect(2, card.GetExplosionPosition());
        cam.BaseEffect(0.3f);
        ClearPipsFromGrid(card);

        if (hand.HasPassive(Passive.Detonator))
        {
            var neighbours = GetNeighboursFor(card, 1, true).ToList();
            neighbours.ForEach(n => actionQueue.Add(new ActivateAction(n, multiplier)));
        }

        if (hand.HasPassive(Passive.Replacement))
        {
            var next = hand.GetRandomCard(card.GetPositionBeforeShaking());
            next.draggable.enabled = false;
            PlacePipsToGrid(next);
            actionQueue.Add(new ActivateAction(next, 1));
            output.text = grid.DataAsString();
        }
        
        Destroy(card.gameObject);
    }

    private void PlacePipsToGrid(Card card)
    { 
        var pips = card.GetPoints(true).ToList();
        pips.ForEach(pip => { grid.Set(pip, pip.x, pip.y); });
    }

    private void ClearPipsFromGrid(Card card)
    {
        var pips = card.GetPoints(false).ToList();
        pips.ForEach(pip =>
        {
            grid.Set(null, pip.x, pip.y); 
        });
    }

    private IEnumerable<Card> GetNeighboursFor(Card card, int distance = 1, bool all = false)
    {
        var pos = card.transform.position;
        return card.GetDirections(all).Select(dir => GetNeighbourFor(pos, dir, distance)).Where(c => c != null);
    }
    
    private Card GetNeighbourFor(Vector3 pos, Vector3 dir, int maxSteps = 1)
    {
        for (var i = 1; i <= maxSteps; i++)
        {
            var diff = (i - 0.25f) * dir;
            var target = Physics2D.OverlapCircle(pos + diff, 0.1f, cardLayer);
            if (target)
            {
                return target.GetComponent<Card>();
            }
        }

        return null;
    }

    private bool IsOnArea(Vector3 pos)
    {
        return Physics2D.OverlapCircle(pos, 0.1f, fieldLayer);
    }

    public void Rotate(Card card, bool clockwise)
    {
        if (!card) return;
        ClearPipsFromGrid(card);
        card.Rotate(clockwise);
        card.ResetBombs();
        PlacePipsToGrid(card);

        if (hand.HasPassive(Passive.StarOnRotate))
        {
            card.MakeRandomStar();
        }
    }

    private IEnumerator MarkCoroutine(List<Pip> pips, int multi)
    {
        yield return MarkPip(pips, true, multi);
        yield return new WaitForSeconds(0.5f);
        yield return MarkPip(pips, false, 0);
    }

    private IEnumerator MarkPip(List<Pip> pips, bool state, int startMulti)
    {
        if (!pips.Any()) yield break;
        
        var total = 0;
        var sum = Vector3.zero;
        var amount = 0;
        var multi = startMulti;
        
        var pos = pips.First().sprite.transform.position;
        var pop = state ? Instantiate(scorePopPrefab, pos, Quaternion.identity) : null;

        foreach (var pip in pips)
        {
            if (pip == null) continue;

            var targetColor = state ? markColor : Color.black;
            var targetSize = Vector3.one * (state ? 0.18f : 0.15f);
            const float duration = 0.15f;
            Tweener.ColorToBounceOut(pip.sprite, targetColor, duration);
            Tweener.ScaleToBounceOut(pip.sprite.transform, targetSize, duration);

            if (state)
            {
                var skipBombActivation = false;
                
                if (pip.isStar)
                {
                    multi += 1;
                    ShowTextAt($"x{multi}", pip.sprite.transform.position + Vector3.right * 0.3f);

                    if (hand.HasPassive(Passive.BombTransformer))
                    {
                        pip.MakeBomb();
                        skipBombActivation = true;
                    }
                }

                if (pip.isBomb && !skipBombActivation)
                {
                    if(pip.isShaking)
                    {
                        actionQueue.Add(new DestroyAction(pip.GetCard(), multi));
                        pip.GetCard().Shake();
                    }
                    else
                    {
                        pip.StartShaking();
                        pip.isShaking = true;
                    }
                    
                    if (hand.HasPassive(Passive.StarTransformer))
                    {
                        pip.GetCard().ResetBombs();
                        pip.MakeStar();
                    }
                }

                if (hand.HasPassive(Passive.Tactician))
                {
                    multi = hand.IsEvenTurn ? 3 : 0;
                }
                
                total++;
                pop.SetText((total * multi).ToString());
                var pt = pop.transform;
                sum += pip.sprite.transform.position;
                amount++;
                pt.position = sum / amount;
            }

            CheckPipTransforms(total, pip);

            yield return new WaitForSeconds(0.02f);
        }
        
        AddScore(total * multi);

        if (pop)
        {
            StartCoroutine(DestroyAfter(pop.gameObject));
        }
    }

    private void CheckPipTransforms(int total, Pip pip)
    {
        if (hand.HasPassive(Passive.Bomberman))
        {
            if ((total + 1) % 20 == 0)
            {
                pip.MakeBomb();
            }
        }

        if (hand.HasPassive(Passive.Starchild))
        {
            if ((total + 1) % 30 == 0)
            {
                pip.MakeStar();
            }
        }
    }

    private void AddScore(int amount)
    {
        totalScore += amount;
        hand.SetScore(totalScore);
        UpdateScore();
    }

    private void ShowTextAt(string text, Vector3 pos, float scale = 1f)
    {
        var pop = Instantiate(scorePopPrefab, pos, Quaternion.identity);
        pop.transform.localScale *= scale;
        pop.SetText(text);
        pop.SetSortOrder(1);
        StartCoroutine(DestroyAfter(pop.gameObject, 1f));
    }

    private static IEnumerator DestroyAfter(GameObject pop, float delay = 0.5f)
    {
        yield return new WaitForSeconds(delay);
        Destroy(pop);
    }

    private void Fill(Pip pip, List<Pip> visited)
    {
        visited.Add(pip);

        if (pip.isBomb && pip.isShaking) return;
        
        grid.GetNeighbours(pip.x, pip.y)
            .Where(n => n != null && !visited.Contains(n))
            .OrderBy(_ => Random.value)
            .ToList()
            .ForEach(n =>
        {
            Fill(n, visited); 
        });
    }

    public void Preview(Card card, Vector3 pos)
    {
        connectionLines.Hide();
        connectionLines.MovePreview(pos);
        
        var pips = card.GetAllPips().ToList();
        
        PreviewLine(0, card, pos, pips[0], 0+0, -1);
        PreviewLine(1, card, pos, pips[0], 0-1, 0);
        PreviewLine(2, card, pos, pips[1], 1+0, -1);
        PreviewLine(3, card, pos, pips[2], 2+0, -1);
        PreviewLine(4, card, pos, pips[2], 2+1, 0);
        PreviewLine(5, card, pos, pips[3], 0-1, 1+0);
        PreviewLine(6, card, pos, pips[5], 2+1, 1+0);
        PreviewLine(7, card, pos, pips[8], 2+0, 2+1);
        PreviewLine(8, card, pos, pips[8], 2+1, 2+0);
        PreviewLine(9, card, pos, pips[7], 1+0, 2+1);
        PreviewLine(10, card, pos, pips[6], 0+0, 2+1);
        PreviewLine(11, card, pos, pips[6], 0-1, 2+0);
    }

    private void PreviewLine(int index, Card card, Vector3 pos, Transform pip, int x, int y)
    {
        var cardPos = card.GetBasePositionFor(pos);
        var targetPip = grid.Get(cardPos.x + x, cardPos.y + y);

        if (pip.gameObject.activeSelf && targetPip != null)
        {
            connectionLines.ShowLine(index, pip.position, targetPip.sprite.transform.position);
        }
    }

    public void HidePreview()
    {
        connectionLines.Hide();
    }

    public void Move(Card card, Vector3 direction, Passive makeStarIf = Passive.None)
    {
        if (!card) return;
        var p = card.transform.position;
        var blocked = GetNeighbourFor(p, direction);
        if (!blocked && IsOnArea(p + direction))
        {
            StartCoroutine(MoveCoroutine(card, direction, makeStarIf));
        }
    }

    private IEnumerator MoveCoroutine(Card card, Vector3 direction, Passive makeStarIf = Passive.None)
    {
        ClearPipsFromGrid(card);
        var t = card.transform;
        Tweener.MoveToBounceOut(t, t.position + direction, 0.3f);
        yield return new WaitForSeconds(0.35f);
        card.PositionPips();
        card.ResetBombs();
        PlacePipsToGrid(card);

        if (hand.HasPassive(makeStarIf))
        {
            card.MakeRandomStar();
        }
        
        output.text = grid.DataAsString();
    }

    public void Tilt()
    {
        ShowTextAt("TILT", cam.cameraRig.position.WhereZ(0), 3f);
        actionQueue.Clear();
        cam.BaseEffect(0.4f);
    }

    public int GetStackSize()
    {
        return 40 + 40 * hand.GetPassiveLevel(Passive.StackSize);
    }
}