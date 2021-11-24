using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.ScriptableObjects;
using Save;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsView : MonoBehaviour
{
    [SerializeField] private TMP_Text statsValues;
    [SerializeField] private TMP_Text chartMax;
    [SerializeField] private LineRenderer line;
    [SerializeField] private List<Image> icons;
    [SerializeField] private TMP_Text passiveNames, passiveCounts;
    [SerializeField] private SpriteCollection iconSprites;
    [SerializeField] private GameObject chartWindow, passiveWindow;

    private Stats stats;
    
    private void Start()
    {
        stats = new Stats();

        statsValues.text = GetContent();
        var best = stats.Data.GetBestScore();
        chartMax.text = best.ToString();
        
        DrawChart(best);
        PopulatePassives();
    }

    private void PopulatePassives()
    {
        var passives = stats.Data.GetMostUsedPassives();

        if (passives.Count < 3)
        {
            passiveWindow.SetActive(false);
            return;
        }
        
        passiveNames.text = "";
        passiveCounts.text = "";
        
        for (var i = 0; i < passives.Count; i++)
        {
            var p = passives[i].passive;
            var details = Passives.GetDetails(p);
            passiveNames.text += details.name + "\n";
            passiveCounts.text += passives[i].count + "\n";
            icons[i].sprite = iconSprites.Get((int)p);
        }
    }

    private void DrawChart(int best)
    {
        var scores = stats.Data.GetScores();

        if (!scores.Any())
        {
            chartWindow.SetActive(false);
            return;
        }
        
        var amount = scores.Count;
        line.positionCount = amount;

        for (var i = 0; i < amount; i++)
        {
            var x = -0.5f + 1f * i / (amount - 1);
            var y = best > 0 ? -0.5f + 1f * scores[i] / best : 0;
            line.SetPosition(i, new Vector3(x, y, 0));
        }
    }

    private string GetContent()
    {
        var content = "";
        content += $"{stats.Data.GetRunCount()}\n";
        content += $"{stats.Data.GetBestScore()}\n";
        content += $"{stats.Data.GetTotalScore()}\n";
        content += $"{decimal.Round((decimal)stats.Data.GetAverageScore(), 2)}\n";
        content += "\n";
        content += $"{stats.Data.GetFurthestStage()}\n";
        content += $"{stats.Data.GetBiggestSingleTurn()}\n";
        content += "\n";
        content += $"{stats.Data.GetDailyRunCount()}\n";
        content += $"{stats.Data.GetBestDailyScore()}\n";
        content += $"{stats.Data.GetTotalDailyScore()}\n";
        content += $"{decimal.Round((decimal)stats.Data.GetAverageDailyScore(), 2)}\n";
        return content;
    }
}