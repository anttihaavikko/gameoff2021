using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Utils;
using Leaderboards;
using Save;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

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
    [SerializeField] private Transform deckPreviewContainer;
    [SerializeField] private TransformState deckPreview, confirmDialog;
    [SerializeField] private CardPreview cardPreviewPrefab;
    [SerializeField] private BoxCollider2D drawPileCollider;
    [SerializeField] private Appearer gameOver, tryAgain, backToMenu;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private TMP_Text pickText, pickTextShadow;
    

    private List<Card> cards;
    private SaveData save;
    private Stack<GameObject> stuffers;
    private int turnNumber = 1;
    private int previousScore;
    private bool previewShown, confirmShown;
    private bool firstPicked;
    private bool alreadyFailed;

    private readonly string[] badIntros =
    {
        "",
        "Too bad!",
        "Oh no!",
        "Oh noes!",
        "Uh oh!",
        "Darn!",
        "Not quite!",
        "Bummer!",
        "Oh dang!",
        "Alas!"
    };

    public bool HasPassive(Passive passive) => save.HasPassive(passive);
    public int GetPassiveLevel(Passive passive) => save.GetPassiveLevel(passive);
    public int Level => save.level + 1;
    public bool IsEvenTurn => turnNumber % 2 == 0;
    public bool IsOddTurn => !IsEvenTurn;
    public bool IsFirstTurn => turnNumber == 1;
    public int GetDeckSize => save.deck.GetCount();

    private void Awake()
    {
        save = SaveData.LoadOrCreate();
        Random.InitState(save.seed + Level);
    }

    private void Start()
    {
        cards = new List<Card>();
        
        deckPreview.transform.position += Vector3.down * Screen.height;
        confirmDialog.transform.position += Vector3.down * Screen.height;
        
        save.deck.Shuffle();
        CreateStuffers();
        UpdateDrawPile();

        for (var i = 0; i < GetPassiveLevel(Passive.BiggerHand) + 1; i++)
        {
            Invoke(nameof(AddCard), 1.5f + i * 0.3f);
        }
        
        ShowPassives();
        Invoke(nameof(ShowPassiveTutorial), 1.2f);
        
        field.StartTimer();
    }

    private void ShowPassiveTutorial()
    {
        if (!save.passives.Any()) return;
        scalab.TriggerTutorial(save.passives.Last());
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleQuitConfirm();
        }
        
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

        if (Input.GetKeyDown(KeyCode.L))
        {
            ToggleDeckPreview();
        }

        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            save.level++;
            save.Save();
            SceneChanger.Instance.ChangeScene("Main");
        }
    }

    public void ToggleDeckPreview()
    {
        previewShown = !previewShown;
        drawPileCollider.enabled = !previewShown;
        var target = deckPreview.Position +  Vector3.down * (previewShown ? 0 : Screen.height);
        var t = deckPreview.transform;
        Tweener.MoveToBounceOut(t, target.WhereX(t.position.x), 0.5f);
        UpdateDeckPreviewContents();
    }

    private void UpdateDeckPreviewContents()
    {
        if (!previewShown) return;
        
        foreach (Transform child in deckPreviewContainer)
        {
            Destroy(child.gameObject);
        }

        var pile = HasPassive(Passive.PreviewInOrder) ? save.deck.PreviewInOrder : save.deck.Preview;
        pile.ForEach(c =>
        {
            var preview = Instantiate(cardPreviewPrefab, deckPreviewContainer);
            preview.Setup(c);
        });
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
        field.TurnStart();
        
        if (turnNumber >= 2)
        {
            SecondTurnMessages();
        }

        previousScore = GetScore();

        if (field.IsFull())
        {
            StageDone();
            return;
        }
        
        if (HasPassive(Passive.Chaos) && turnNumber % 5 == 0)
        {
            var luck = GetLuck();
            AddCard(CardData.GetRandom(luck), Vector3.down * 5, true);
            return;
        }
        
        if (save.deck.IsEmpty)
        {
            if (!cards.Any())
            {
                StageDone();
            }
            
            return;
        }

        stuffers.Pop().SetActive(false);

        var cardData = save.deck.Draw();
        AddCard(cardData, deckTop.position);
        UpdateDeckPreviewContents();
    }

    private void StageDone()
    {
        cards.ForEach(MoveCardDown);
        field.StopTimer();
        Invoke(nameof(CreateOptions), 1.5f);   
    }

    private void SecondTurnMessages()
    {
        if (Level >= 2 && turnNumber == 3)
        {
            TriggerTutorial(BaseTutorial.DeckPreview);
        }
        
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
        var card = CreateCard(startPosition, data, HasPassive(Passive.StarOnGenerated));
        return card;
    }

    public Card CreateCard(Vector3 startPosition, CardData data, bool addStar = false)
    {
        var card = Instantiate(cardPrefab, startPosition, Quaternion.identity);
        card.Setup(data);

        if (addStar)
        {
            card.AddRandomStar();
        }
        
        return card;
    }

    private void AddCard(CardData cardData, Vector3 from, bool addStar = false)
    {
        UpdateDrawPile();

        var c = Instantiate(cardPrefab, from, Quaternion.identity);
        c.Setup(cardData);
        c.draggable.dropped += CardMoved;
        c.draggable.preview += ConnectionPreview;
        c.draggable.hidePreview += HidePreview;
        c.draggable.dropCancelled += PositionCards;

        if (addStar)
        {
            c.AddRandomStar();
        }

        cards.Insert(0, c);
        PositionCards();

        field.ApplyCurseToDrawnCard(c);
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

        var addition = (amount - 1) * deckCardHeight;
        drawPileCollider.size = new Vector2(1, 1 + addition);
        drawPileCollider.offset = new Vector2(0, addition * 0.5f);
    }

    public void NextTurn()
    {
        turnNumber++;
        turnField.text = $"TURN {turnNumber}";
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

    public void FailStage()
    {
        CreateOptions();
        alreadyFailed = true;
    }

    private void CreateOptions()
    {
        if (alreadyFailed) return;
        
        if (previewShown)
        {
            ToggleDeckPreview();    
        }
        
        if (LevelFailed())
        {
            scoreManager.SubmitScore(save.score, Level);
            
            gameOver.Show();
            tryAgain.ShowAfter(0.3f);
            backToMenu.ShowAfter(0.6f);
            
            field.BaseEffect(0.3f);

            Saver.Clear();
            
            return;
        }
        
        TriggerTutorial(BaseTutorial.NewCard);
        
        cardPicks.Show();

        var amount = 3 + GetPassiveLevel(Passive.CardPicks);
        var luck = GetLuck();
        
        for (var i = 0; i < amount; i++)
        {
            var pos = cardPicks.transform.position + Vector3.left * (amount - 1) * 0.5f + Vector3.right * i;
            var card = Instantiate(cardPrefab, new Vector3(0, -10f, 0), Quaternion.identity);
            Tweener.MoveToBounceOut(card.transform, pos, 0.2f + 0.05f * i);
            var data = CardData.GetRandom(luck);
            card.Setup(data);
            card.draggable.CanDrag = false;
            card.hoverer.onHover += () =>
            {
                var message = card.GetInfo();
                if (message != null)
                {
                    scalab.ShowMessage(message, true);   
                }
            };
            card.draggable.click += () =>
            {
                card.draggable.click = null;
                save.deck.Add(data);
                MoveCardDown(card);

                if (firstPicked || !HasPassive(Passive.DoublePicks))
                {
                    save.level++;
                    save.Save();
                    SceneChanger.Instance.ChangeScene("Pick");   
                }

                if (HasPassive(Passive.DoublePicks))
                {
                    pickText.text = pickTextShadow.text = "Pick another new card!";   
                }

                firstPicked = true;
            };
        }
    }

    private static void MoveCardDown(Card card)
    {
        Tweener.MoveToBounceOut(card.transform, Vector3.down * 10f, 0.3f);
    }

    private bool LevelFailed()
    {
        if (field.HasTask())
        {
            var failure = !field.TaskComplete();
            if(failure) scalab.ShowMessage($"{badIntros.Random()} You did (not) manage to complete the (special task)!", true);
            return failure;
        }

        var par = field.GetPar(Level);
        var noPar = GetScore() < par;
        if(noPar) scalab.ShowMessage($"{badIntros.Random()} You did (not) reach the stage (par) of ({par}) points.", true);
        return noPar;
    }

    private bool CardsLeft()
    {
        var cardsLeft = cards.Any() || !save.deck.IsEmpty;
        if(cardsLeft) scalab.ShowMessage($"{badIntros.Random()} You did (not) manage to (play all) your (cards).", true);
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

    public void ShowMessage(string message, bool force)
    {
        scalab.ShowMessage(message, force);
    }

    public void BackToMenu()
    {
        ToggleQuitConfirm();
        SceneChanger.Instance.ChangeScene("Start");
    }

    public void CancelQuit()
    {
        ToggleQuitConfirm();
    }

    private void ToggleQuitConfirm()
    {
        confirmShown = !confirmShown;
        drawPileCollider.enabled = !confirmShown;
        var target = confirmDialog.Position +  Vector3.down * (confirmShown ? 0 : Screen.height);
        var t = confirmDialog.transform;
        Tweener.MoveToBounceOut(t, target.WhereX(t.position.x), 0.5f);
    }
}