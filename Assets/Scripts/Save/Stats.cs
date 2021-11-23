using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Utils;
using UnityEngine;

namespace Save
{
    public class Stats
    {
        private const string Key = "Stats";

        public StatsData Data { get; }

        public Stats()
        {
            Data = Saver.LoadOrCreate<StatsData>(Key);
        }

        public void AddPassive(Passive passive)
        {
            Data.AddPassive(passive);
            Saver.Save(Data, Key);
        }

        public void AddScore(int score, bool daily = false)
        {
            Data.AddScore(score);
            Saver.Save(Data, Key);
        }
    }

    [Serializable]
    public class StatsData
    {
        [SerializeField] private List<PassiveTimes> passiveTimes;
        [SerializeField] private List<int> scoreHistory, dailyScoreHistory;
        [SerializeField] private int furthestStage, bestTurnScore;

        public StatsData()
        {
            passiveTimes = new List<PassiveTimes>();
            scoreHistory = new List<int>();
            dailyScoreHistory = new List<int>();
        }

        public void AddPassive(Passive passive)
        {
            if (passiveTimes.Any(p => p.passive == passive))
            {
                passiveTimes.Where(p => p.passive == passive).ToList().ForEach(p => p.count++);
                return;
            }
            
            passiveTimes.Add(new PassiveTimes
            {
                passive = passive,
                count = 1
            });
        }

        public void AddScore(int score, bool daily = false)
        {
            if (daily)
            {
                dailyScoreHistory.Add(score);
                return;
            }
            
            scoreHistory.Add(score);
        }
        
        public int GetRunCount()
        {
            return scoreHistory.Count;
        }

        public int GetDailyRunCount()
        {
            return dailyScoreHistory.Count;
        }

        public List<PassiveTimes> GetMostUsedPassives()
        {
            return passiveTimes.OrderByDescending(p => p.count).Take(3).ToList();
        }

        public int GetTotalScore()
        {
            return scoreHistory.Sum();
        }
        
        public int GetTotalDailyScore()
        {
            return dailyScoreHistory.Sum();
        }
        
        public double GetAverageScore()
        {
            return scoreHistory.Any() ? scoreHistory.Average() : 0;
        }
        
        public double GetAverageDailyScore()
        {
            return dailyScoreHistory.Any() ? dailyScoreHistory.Average() : 0;
        }

        public int GetBestScore()
        {
            return scoreHistory.Any() ? scoreHistory.Max() : 0;
        }
        
        public int GetBestDailyScore()
        {
            return dailyScoreHistory.Any() ? dailyScoreHistory.Max() : 0;
        }

        public int GetFurthestStage()
        {
            return furthestStage;
        }

        public int GetBiggestSingleTurn()
        {
            return bestTurnScore;
        }

        public void AddStageScore(int score)
        {
            bestTurnScore = Mathf.Max(bestTurnScore, score);
        }

        public void AddStageNumber(int stage)
        {
            furthestStage = Mathf.Max(furthestStage, stage);
        }

        public List<int> GetScores()
        {
            return scoreHistory;
        }
    }

    [Serializable]
    public class PassiveTimes
    {
        public Passive passive;
        public int count;
    }
}