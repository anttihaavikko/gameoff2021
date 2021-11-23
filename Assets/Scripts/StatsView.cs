using System;
using Save;
using TMPro;
using UnityEngine;

public class StatsView : MonoBehaviour
{
    [SerializeField] private TMP_Text statsValues;
    
    private Stats stats;
    
    private void Start()
    {
        stats = new Stats();

        statsValues.text = GetContent();
    }

    private string GetContent()
    {
        var content = "";
        content += $"{stats.Data.GetRunCount()}\n";
        content += $"{stats.Data.GetBestScore()}\n";
        content += $"{stats.Data.GetTotalScore()}\n";
        content += $"{stats.Data.GetAverageScore()}\n\n";
        
        content += $"{stats.Data.GetFurthestStage()}\n";
        content += $"{stats.Data.GetBiggestSingleTurn()}\n";
        
        content += $"{stats.Data.GetDailyRunCount()}\n";
        content += $"{stats.Data.GetBestDailyScore()}\n";
        content += $"{stats.Data.GetTotalDailyScore()}\n";
        content += $"{stats.Data.GetBestDailyScore()}\n";
        
        return content;
    }
}