using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Utils;
using Save;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Hand : MonoBehaviour
{
    [SerializeField] private Card cardPrefab;
    [SerializeField] private Field field;
    [SerializeField] private Appearer cardPicks;
    [SerializeField] private GameObject drawPile;
    [SerializeField] private TMP_Text drawPileNumber;
    [SerializeField] private PassiveIcon passiveIconPrefab;
    [SerializeField] private Transform passiveIconContainer;
    [SerializeField] private PassiveTooltip passiveTooltip;

    private List<Card> cards;
    private SaveData save;

    public bool HasPassive(Passive passive) => save.HasPassive(passive);
    public int GetPassiveLevel(Passive passive) => save.GetPassiveLevel(passive);
    public int Level => save.level + 1;

    private void Awake()
    {
        save = SaveData.LoadOrCreate();
    }

    private void Start()
    {
        cards = new List<Card>();
        
        save.deck.Shuffle();
        UpdateDrawPile();

        for (var i = 0; i < GetPassiveLevel(Passive.BiggerHand) + 1; i++)
        {
            Invoke(nameof(AddCard), 1.5f + i * 0.3f);
        }
        
        ShowPassives();
    }

    private void ShowPassives()
    {
        save.passives.ForEach(p =>
        {
            var icon = Instantiate(passiveIconPrefab, passiveIconContainer);
            icon.Setup(p, passiveTooltip);
        });
    }

    private void Update()
    {
        DebugControls();
    }

    private void DebugControls()
    {
        if (!Application.isEditor) return;

        if (Input.GetKeyDown(KeyCode.A))
        {
            save.deck.Add(CardData.GetRandom());
            save.Save();
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneChanger.Instance.ChangeScene("Main");
        }
        
        if (Input.GetKeyDown(KeyCode.D))
        {
            Saver.Clear();
            SceneChanger.Instance.ChangeScene("Main");
        }
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            SceneChanger.Instance.ChangeScene("Pick");
        }
        
        if (Input.GetKeyDown(KeyCode.W))
        {
            CreateOptions();
        }
    }

    private void PositionCards()
    {
        var basePos = transform.position + (cards.Count - 1) * 0.5f * Vector3.left;
        var index = 0;
        foreach (var c in cards)
        {
            Tweener.MoveToBounceOut(c.transform, basePos + index * Vector3.right, 0.3f);
            index++;
        }
    }

    public void AddCard()
    {
        if (save.deck.IsEmpty)
        {
            if (!cards.Any())
            {
                Invoke(nameof(CreateOptions), 1.5f);   
            }
            
            return;
        }

        var cardData = save.deck.Draw();
        UpdateDrawPile();
        
        var c = Instantiate(cardPrefab, drawPile.transform.position, Quaternion.identity);
        c.Setup(cardData);
        c.draggable.dropped += () => CardMoved(c);
        c.draggable.preview += pos => ConnectionPreview(c, pos);
        c.draggable.hidePreview += HidePreview;
        
        cards.Insert(0, c);
        PositionCards();
    }

    private void UpdateDrawPile()
    {
        var amount = save.deck.GetCount();
        drawPileNumber.text = amount.ToString();

        if (amount == 0)
        {
            drawPile.SetActive(false);
        }
    }

    private void HidePreview()
    {
        field.HidePreview();
    }

    private void ConnectionPreview(Card card, Vector2 pos)
    {
        field.Preview(card, pos);
    }

    private void CardMoved(Card card)
    {
        card.draggable.dropped = null;
        card.draggable.preview = null;
        card.draggable.hidePreview -= HidePreview;
        field.Place(card);
        cards.Remove(card);
        PositionCards();
    }

    private void CreateOptions()
    {
        cardPicks.Show();

        var amount = 3 + GetPassiveLevel(Passive.CardPicks);
        var luck = Mathf.Pow(0.9f, GetPassiveLevel(Passive.LuckyRolls));
        
        for (var i = 0; i < amount; i++)
        {
            var pos = cardPicks.transform.position + Vector3.left * (amount - 1) * 0.5f + Vector3.right * i;
            var card = Instantiate(cardPrefab, pos, Quaternion.identity);
            var data = CardData.GetRandom(luck);
            card.Setup(data);
            card.draggable.CanDrag = false;
            card.draggable.click += () =>
            {
                card.draggable.click = null;
                save.deck.Add(data);
                save.level++;
                save.Save();
                SceneChanger.Instance.ChangeScene("Pick");
            };
        }
    }

    public int GetScore()
    {
        return save.score;
    }

    public void SetScore(int score)
    {
        save.score = score;
    }
}