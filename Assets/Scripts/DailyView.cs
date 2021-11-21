using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AnttiStarterKit.Utils;
using Leaderboards;
using Save;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class DailyView : MonoBehaviour
{
    [SerializeField] private TMP_Text title, titleShadow;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private Transform cardContainer, passiveContainer;
    [SerializeField] private CardPreview cardPrefab;
    [SerializeField] private PassiveIcon passivePrefab;
    [SerializeField] private PassiveTooltip passiveTooltip;

    private DateTime current;
    private List<CardData> cards;
    private List<Passive> passives;

    private string DateString => current.ToString("MMM dd yyyy");
    private int Seed => int.Parse(current.ToString("yyyyMMdd"));

    private void Start()
    {
        current = DateTime.Today;
        DayChanged();
    }

    public void Continue()
    {
        var save = SaveData.Create(cards, passives, Seed, DateString);
        save.Save();
        var scene = PlayerPrefs.HasKey("PlayerName") ? "Main" : "Name";
        SceneChanger.Instance.ChangeScene(scene);
    }

    public void Back()
    {
        SceneChanger.Instance.ChangeScene("Start");
    }

    public void ChangeDate(int direction)
    {
        var next = current.AddDays(direction);
        if (next > DateTime.Today) return;
        current = current.AddDays(direction);
        DayChanged();
    }

    private void DayChanged()
    {
        title.text = titleShadow.text = $"DAILY RUN FOR {DateString}";
        scoreManager.gameName = $"BUG-{DateString}";
        scoreManager.LoadLeaderBoards(0);
        Random.InitState(Seed);
        UpdateStarters();
    }

    private CardData GetCard()
    {
        return Random.value < 0.3f ? CardData.Starter() : CardData.GetRandom();
    }

    private void UpdateStarters()
    {
        RemoveChildren(cardContainer);
        RemoveChildren(passiveContainer);

        cards = new List<CardData>();
        
        var cardCount = Random.Range(4, 10);
        for (var i = 0; i < cardCount; i++)
        {
            var data = GetCard();
            cards.Add(data);
            var card = Instantiate(cardPrefab, cardContainer);
            card.Setup(data);
        }

        var passiveCount = Mathf.Max(0, Random.Range(-2, 4));
        passiveContainer.gameObject.SetActive(passiveCount > 0);
        passives = Passives.GetRandom(new List<Passive>(), passiveCount).ToList();
        passives.ToList().ForEach(p =>
        {
            var icon = Instantiate(passivePrefab, passiveContainer);
            icon.Setup(p, passiveTooltip, 1);
        });
    }

    private void RemoveChildren(Transform t)
    {
        foreach (Transform child in t)
        {
            Destroy(child.gameObject);
        }
    }
}