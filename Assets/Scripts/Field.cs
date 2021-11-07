using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Actions;
using AnttiStarterKit.Animations;
using TMPro;
using UnityEngine;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Utils;
using Save;
using Random = UnityEngine.Random;

public class Field : MonoBehaviour
{
    [SerializeField] private TMP_Text output, totalScoreField, levelField, parField;
    [SerializeField] private TextWithBackground scorePopPrefab;
    [SerializeField] private ConnectionLines connectionLines;
    [SerializeField] private LayerMask cardLayer;
    [SerializeField] private Hand hand;
    [SerializeField] private Appearer spinner;
    [SerializeField] private Color markColor;

    private TileGrid<Pip> grid;
    private int totalScore;
    private ActionQueue actionQueue;

    private void Start()
    {
        actionQueue = new ActionQueue();
        grid = new TileGrid<Pip>(15, 15);
        totalScore = hand.GetScore();
        UpdateScore();
        levelField.text = $"STAGE {hand.Level}";
        parField.text = $"PAR {hand.Level * 30}";
    }

    private void UpdateScore()
    {
        totalScoreField.text = hand.GetScore().ToString();
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
        hand.AddCard();
        hand.LockCards(false);
        spinner.Hide();
    }

    public void Activate(Card card, int multi = 1)
    {
        var pos = card.GetCoordinates();

        if (pos.y == 4)
        {
            multi += hand.GetPassiveLevel(Passive.MultiOnBottom);
        }
        
        if (pos.y == 0)
        {
            multi += hand.GetPassiveLevel(Passive.MultiOnTop);
        }

        if (multi > 1)
        {
            ShowTextAt($"x{multi}", card.transform.position + Vector3.right * 0.3f);
        }
        
        actionQueue.Add(new ScoreAction(card, multi));

        if (card.IsRotator)
        {
            var neighbours = GetNeighboursFor(card).ToList();
            actionQueue.Add(new RotateAction(neighbours, multi, card.RotatesClockwise));
            neighbours.ForEach(n => actionQueue.Add(new ActivateAction(n, multi + 1)));
        }
    }

    public IEnumerator ScoreCard(Card card, int multi)
    {
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

    public void RemoveCard(Card card)
    {
        // TODO: explode
        ClearPipsFromGrid(card);
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

    private IEnumerable<Card> GetNeighboursFor(Card card)
    {
        var pos = card.transform.position;
        return card.GetDirections().Select(dir => GetNeighbourFor(pos, dir)).Where(c => c != null);
    }
    
    private Card GetNeighbourFor(Vector3 pos, Vector3 dir)
    {
        var target = Physics2D.OverlapCircle(pos + dir * 0.75f, 0.1f, cardLayer);
        return !target ? null : target.GetComponent<Card>();
    }

    public void Rotate(Card card, bool clockwise)
    {
        ClearPipsFromGrid(card);
        card.Rotate(clockwise);
        PlacePipsToGrid(card);
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
                if (pip.isStar)
                {
                    multi += 1;
                    ShowTextAt($"x{multi}", pip.sprite.transform.position + Vector3.right * 0.3f);
                }

                if (pip.isBomb)
                {
                    if(pip.isShaking)
                    {
                        actionQueue.Add(new DestroyAction(pip.GetCard()));
                    }
                    else
                    {
                        pip.StartShaking();
                        pip.isShaking = true;
                    }
                }
                
                total++;
                pop.SetText((total * multi).ToString());
                var pt = pop.transform;
                sum += pip.sprite.transform.position;
                amount++;
                pt.position = sum / amount;
            }

            yield return new WaitForSeconds(0.02f);
        }
        
        AddScore(total * multi);

        if (pop)
        {
            StartCoroutine(DestroyAfter(pop.gameObject));
        }
    }

    private void AddScore(int amount)
    {
        totalScore += amount;
        hand.SetScore(totalScore);
        UpdateScore();
    }

    private void ShowTextAt(string text, Vector3 pos)
    {
        var pop = Instantiate(scorePopPrefab, pos, Quaternion.identity);
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
}