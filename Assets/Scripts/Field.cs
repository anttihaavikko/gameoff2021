using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Field : MonoBehaviour
{
    [SerializeField] private TMP_Text output, totalScoreField;
    [SerializeField] private TextWithBackground scorePopPrefab;
    [SerializeField] private ConnectionLines connectionLines;

    private TileGrid<Pip> grid;
    private int totalScore;

    private void Start()
    {
        grid = new TileGrid<Pip>(15, 15);
    }

    public void Place(Card card)
    {
        connectionLines.Hide();
        
        grid.All().Where(p => p != null).ToList().ForEach(p => p.sprite.color = Color.black);
        
        var pips = card.GetPoints(true).ToList();
        pips.ForEach(pip =>
        {
            grid.Set(pip, pip.x, pip.y); 
        });

        var allVisited = new List<Pip>();
        
        pips.ForEach(pip =>
        {
            if (allVisited.Contains(pip)) return;
            
            var visited = new List<Pip>();
            Fill(pip, visited);
            visited = visited.Distinct().OrderBy(p => p.GetDistanceTo(pip)).ToList();
            StartCoroutine(MarkCoroutine(visited));
            allVisited.AddRange(visited);
        });
    }

    private IEnumerator MarkCoroutine(List<Pip> pips)
    {
        yield return MarkPip(pips, true);
        yield return new WaitForSeconds(0.5f);
        totalScoreField.text = totalScore.ToString();
        yield return MarkPip(pips, false);
    }

    private IEnumerator MarkPip(List<Pip> pips, bool state)
    {
        if (!pips.Any()) yield break;
        
        var total = 0;
        var sum = Vector3.zero;
        var amount = 0;
        var multi = 1;

        var pos = pips.First().sprite.transform.position + Vector3.right;
        var pop = state ? Instantiate(scorePopPrefab, pos, Quaternion.identity) : null;

        foreach (var pip in pips)
        {
            if (pip == null) continue;
            
            var targetColor = state ? Color.red : Color.black;
            var targetSize = Vector3.one * (state ? 0.21f : 0.17f);
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
                total++;
                pop.SetText((total * multi).ToString());
                var pt = pop.transform;
                sum += pip.sprite.transform.position;
                amount++;
                pt.position = sum / amount;
            }

            yield return new WaitForSeconds(0.02f);
        }

        totalScore += total * multi;

        if (pop)
        {
            StartCoroutine(DestroyAfter(pop.gameObject));
        }
    }

    private void ShowTextAt(string text, Vector3 pos)
    {
        var pop = Instantiate(scorePopPrefab, pos, Quaternion.identity);
        pop.SetText(text);
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