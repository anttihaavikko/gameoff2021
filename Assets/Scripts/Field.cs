using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Field : MonoBehaviour
{
    [SerializeField] private TMP_Text output, totalScoreField;
    [SerializeField] private TextWithBackground scorePopPrefab;

    private TileGrid<Pip> grid;
    private int totalScore;

    private void Start()
    {
        grid = new TileGrid<Pip>(15, 15);
    }

    public void Place(Card card)
    {
        grid.All().Where(p => p != null).ToList().ForEach(p => p.sprite.color = Color.black);
        
        var pips = card.GetPoints().ToList();
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
            
            pip.sprite.color = state ? Color.red : Color.black;
            pip.sprite.transform.localScale = Vector3.one * (state ? 0.2f : 0.18f);

            if (state)
            {
                if (pip.isStar)
                {
                    multi += 1;
                }
                total++;
                pop.SetText((total * multi).ToString());
                var pt = pop.transform;
                sum += pip.sprite.transform.position;
                amount++;
                pt.position = sum / amount;
            }

            yield return null;
        }

        totalScore += total * multi;

        if (pop)
        {
            StartCoroutine(DestroyAfter(pop.gameObject));
        }
    }

    private static IEnumerator DestroyAfter(GameObject pop)
    {
        yield return new WaitForSeconds(0.5f);
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
}