using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Utils;
using Save;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class Hand : MonoBehaviour
{
    [SerializeField] private Card cardPrefab;
    [SerializeField] private Field field;
    [SerializeField] private Appearer cardPicks;
    [SerializeField] private TMP_Text drawPileNumber;
    [SerializeField] private PassiveIcon passiveIconPrefab;
    [SerializeField] private Transform passiveIconContainer;
    [SerializeField] private PassiveTooltip passiveTooltip;
    [SerializeField] private Transform deckContainer, deckTop;
    [SerializeField] private SortingGroup deckStufferPrefab;
    [SerializeField] private float deckCardHeight = 0.2f;
    [SerializeField] private TMP_Text turnField, turnFieldShadow;
    [SerializeField] private Scalab scalab;

    private List<Card> cards;
    private SaveData save;
    private Stack<GameObject> stuffers;
    private int turnNumber = 1;
    private int previousScore;

    public bool HasPassive(Passive passive) => save.HasPassive(passive);
    public int GetPassiveLevel(Passive passive) => save.GetPassiveLevel(passive);
    public int Level => save.level + 1;
    public bool IsEvenTurn => turnNumber % 2 == 0;
    public bool IsOddTurn => !IsEvenTurn;

    private void Awake()
    {
        save = SaveData.LoadOrCreate();
    }

    private void Start()
    {
        cards = new List<Card>();
        
        save.deck.Shuffle();
        CreateStuffers();
        UpdateDrawPile();

        for (var i = 0; i < GetPassiveLevel(Passive.BiggerHand) + 1; i++)
        {
            Invoke(nameof(AddCard), 1.5f + i * 0.3f);
        }
        
        ShowPassives();
    }

    private void CreateStuffers()
    {
        stuffers = new Stack<GameObject>();
        var count = 0;
        var amount = save.deck.cards.Count;
        save.deck.cards.ForEach(c =>
        {
            var stuffer = Instantiate(deckStufferPrefab, deckContainer);
            stuffer.sortingOrder = -amount - 2 + count;
            stuffer.transform.position += Vector3.up * count * deckCardHeight;
            count++;
            stuffers.Push(stuffer.gameObject);
        });
    }

    private void ShowPassives()
    {
        save.passives.ForEach(p =>
        {
            var icon = Instantiate(passiveIconPrefab, passiveIconContainer);
            icon.Setup(p, passiveTooltip, save.GetPassiveLevel(p));
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
            scalab.ResetTutorials();
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

        Time.timeScale = Input.GetKey(KeyCode.Tab) ? 5 : 1;
    }
    
    public void TriggerTutorial(BaseTutorial tut)
    {
        scalab.TriggerTutorial(tut);
    }

    public void MarkTutorial(BaseTutorial tut)
    {
        scalab.MarkTutorial(tut);
    }

    private void PositionCards()
    {
        var basePos = transform.position + (cards.Count - 1) * 0.5f * Vector3.left;
        var index = 0;
        foreach (var c in cards)
        {
            if (!c.draggable.IsDragging)
            {
                Tweener.MoveToBounceOut(c.transform, basePos + index * Vector3.right, 0.3f);    
            }
            
            index++;
        }
    }

    public void AddCard()
    {
        if (turnNumber >= 2)
        {
            SecondTurnMessages();
        }

        previousScore = GetScore();
        
        if (HasPassive(Passive.Chaos) && turnNumber % 5 == 0)
        {
            var luck = GetLuck();
            AddCard(CardData.GetRandom(luck), Vector3.down * 5);
            return;
        }

        if (field.IsFull())
        {
            Invoke(nameof(CreateOptions), 1.5f);
            return;
        }
        
        if (save.deck.IsEmpty)
        {
            if (!cards.Any())
            {
                Invoke(nameof(CreateOptions), 1.5f);   
            }
            
            return;
        }

        stuffers.Pop().SetActive(false);

        var cardData = save.deck.Draw();
        AddCard(cardData, deckTop.position);
    }

    private void SecondTurnMessages()
    {
        if (GetScore() - previousScore >= 8)
        {
            TriggerTutorial(BaseTutorial.NiceKeepGoing);
            MarkTutorial(BaseTutorial.NotQuite);
            return;
        }
        
        TriggerTutorial(BaseTutorial.NotQuite);
    }

    public Card GetRandomCard(Vector3 startPosition)
    {
        var luck = GetLuck();
        var data = CardData.GetRandom(luck);
        var card = CreateCard(startPosition, data);
        return card;
    }

    public Card CreateCard(Vector3 startPosition, CardData data)
    {
        var card = Instantiate(cardPrefab, startPosition, Quaternion.identity);
        card.Setup(data);
        return card;
    }

    private void AddCard(CardData cardData, Vector3 from)
    {
        UpdateDrawPile();

        var c = Instantiate(cardPrefab, from, Quaternion.identity);
        c.Setup(cardData);
        c.draggable.dropped += CardMoved;
        c.draggable.preview += ConnectionPreview;
        c.draggable.hidePreview += HidePreview;
        c.draggable.dropCancelled += PositionCards;

        cards.Insert(0, c);
        PositionCards();
    }

    private void UpdateDrawPile()
    {
        var amount = save.deck.GetCount();
        drawPileNumber.text = amount.ToString();
        
        Tweener.MoveToBounceOut(deckTop, deckContainer.position + (amount - 1) * Vector3.up * deckCardHeight, 0.1f);

        if (amount == 0)
        {
            deckTop.gameObject.SetActive(false);
        }
    }

    public void NextTurn()
    {
        turnNumber++;
        turnField.text = $"Turn {turnNumber}";
        turnFieldShadow.text = turnField.text;
        
        AddCard();
        LockCards(false);
    }

    private void HidePreview()
    {
        field.HidePreview();
    }

    private void ConnectionPreview(Draggable draggable)
    {
        var card = cards.First(c => c.draggable == draggable);
        field.Preview(card, draggable.GetRoundedPos());
    }

    private void CardMoved(Draggable draggable)
    {
        var card = cards.First(c => c.draggable == draggable);
        card.hoverer.Disable();
        card.draggable.dropped -= CardMoved;
        card.draggable.preview -= ConnectionPreview;
        card.draggable.hidePreview -= HidePreview;
        card.draggable.dropCancelled -= PositionCards;
        field.Place(card);
        cards.Remove(card);
        PositionCards();
    }

    private void CreateOptions()
    {
        if (LevelFailed())
        {
            // TODO: restart etc
            return;
        }
        
        cardPicks.Show();

        var amount = 3 + GetPassiveLevel(Passive.CardPicks);
        var luck = GetLuck();
        
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

    private bool LevelFailed()
    {
        var noPar = GetScore() < Field.GetPar(Level);
        if(noPar) scalab.ShowMessage($"Too bad! You didn't reach the stage (par) of ({Field.GetPar(Level)}).", true);
        return noPar || CardsLeft();
    }

    private bool CardsLeft()
    {
        var cardsLeft = cards.Any() || !save.deck.IsEmpty;
        if(cardsLeft) scalab.ShowMessage("Too bad! You didn't manage to (play all) your (cards).", true);
        return cardsLeft;
    }

    private float GetLuck()
    {
        return Mathf.Pow(0.9f, GetPassiveLevel(Passive.LuckyRolls));
    }

    public int GetScore()
    {
        return save.score;
    }

    public void SetScore(int score)
    {
        save.score = score;
    }

    public void LockCards(bool state)
    {
        // print($"Locking all {cards.Count} cards");
        cards.ForEach(c => c.draggable.DropLocked = state);
    }
}