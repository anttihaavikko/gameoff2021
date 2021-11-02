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
        var total = 0;
        
        foreach (var pip in pips)
        {
            if (pip != null)
            {
                pip.sprite.color = state ? Color.red : Color.black;
            }

            if (state)
            {
                total++;
                output.text = total.ToString();
            }

            yield return null;
        }

        totalScore += total;
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