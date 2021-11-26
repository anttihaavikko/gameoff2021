using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Actions;
using AnttiStarterKit.Animations;
using TMPro;
using UnityEngine;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Managers;
using AnttiStarterKit.ScriptableObjects;
using AnttiStarterKit.Utils;
using AnttiStarterKit.Visuals;
using Curses;
using Save;
using Tasks;
using Random = UnityEngine.Random;

public class Field : MonoBehaviour
{
    [SerializeField] private TMP_Text output, totalScoreField, levelField, parField;
    [SerializeField] private TMP_Text totalScoreFieldShadow, levelFieldShadow, parFieldShadow;
    [SerializeField] private TextWithBackground scorePopPrefab;
    [SerializeField] private ConnectionLines connectionLines;
    [SerializeField] private LayerMask cardLayer, fieldLayer;
    [SerializeField] private Hand hand;
    [SerializeField] private Appearer spinner;
    [SerializeField] private Color markColor;
    [SerializeField] private EffectCamera cam;
    [SerializeField] private Meanie meanie;
    [SerializeField] private Cloud cloudPrefab;
    [SerializeField] private SoundCollection noteSounds;

    private TileGrid<Pip> grid;
    private int totalScore, shownScore;
    private ActionQueue actionQueue;
    private List<Card> cards;
    private List<Card> turnActivatedCards;
    private StageTask stageTask;
    private Curse curse;
    private bool processing;
    private int scoreScroll;
    private int turnScore;
    private int notePos;

    public int PreviousTouched { get; private set; }

    public bool HasCurse => curse != null;

    private void Start()
    {
        cards = new List<Card>();
        turnActivatedCards = new List<Card>();
        cam.BaseEffect(0.1f);
        actionQueue = new ActionQueue();
        grid = new TileGrid<Pip>(15, 15);
        totalScore = hand.GetScore();
        shownScore = totalScore;
        UpdateScore();
        
        PlaceLevelCards();
        
        levelField.text = $"STAGE {hand.Level}";
        levelFieldShadow.text = levelField.text;
        
        ShowTaskOrPar();
        Invoke(nameof(ShowTaskAndCurseTutorial), 1.1f);
        
        AudioManager.Instance.ChangeMusic(1);
    }

    private void Update()
    {
        UpdateScore();
        Time.timeScale = processing && Input.GetKey(KeyCode.Space) ? 5 : 1;
    }

    public void BaseEffect(float amount)
    {
        cam.BaseEffect(amount);
    }

    public Cloud CreateCloud()
    {
        return Instantiate(cloudPrefab, Vector3.zero, Quaternion.identity);
    }

    public void ShowTaskAndCurseTutorial()
    {
        if (HasCurse)
        {
            hand.ShowMessage(curse.GetTutorial(), false);
        }
        
        if (stageTask == null) return;
        Invoke(nameof(ShowTaskLabel), 0.3f);
        hand.ShowMessage(stageTask.GetTutorial(), false);
    }

    private void ShowTaskLabel()
    {
        ShowTextAt("SPECIAL TASK", cam.cameraRig.position.WhereZ(0), 2.25f, 3, 3f);
    }

    private void ShowTaskOrPar()
    {
        var text = stageTask != null ? stageTask.GetText() : $"PAR {GetPar(hand.Level)}";
        parField.text = text;
        parFieldShadow.text = text;
    }

    public bool HasCurseOfType<T>()
    {
        return HasCurse && curse.GetType() == typeof(T);
    }

    private void PlaceLevelCards()
    {
        if (hand.IsDaily)
        {
            CreateDailyLevel();
            return;
        }

        switch (hand.Level)
        {
            case 1:
            {
                PlaceCard(hand.CreateCard(Vector3.zero, CardData.Loop()));
                break;
            }
            case 3:
            {
                PlaceCard(hand.CreateCard(new Vector3(-1, -1, 0), CardData.Empty()));
                PlaceCard(hand.CreateCard(new Vector3(-1, 1, 0), CardData.Empty()));
                PlaceCard(hand.CreateCard(new Vector3(1, -1, 0), CardData.Empty()));
                PlaceCard(hand.CreateCard(new Vector3(1, 1, 0), CardData.Empty()));
                break;
            }
            case 5:
            {
                PlaceCard(hand.CreateCard(new Vector3(-2, -2, 0), CardData.Empty()));
                PlaceCard(hand.CreateCard(new Vector3(-2, 2, 0), CardData.Empty()));
                PlaceCard(hand.CreateCard(new Vector3(2, -2, 0), CardData.Empty()));
                PlaceCard(hand.CreateCard(new Vector3(2, 2, 0), CardData.Empty()));
                break;
            }
            case 6:
            {
                stageTask = GetTask(0);
                break;
            }
            case 8:
            {
                curse = GetCurse(0);
                break;
            }
            case 10:
            {
                stageTask = GetTask(1);
                break;
            }
            case 12:
            {
                curse = GetCurse(1);
                stageTask = GetTask(2);
                break;
            }
            case 14:
            {
                curse = GetCurse(2);
                stageTask = GetTask(3);
                break;
            }
            case 16:
            {
                stageTask = GetTask(4);
                break;
            }
            case 18:
            {
                curse = GetCurse(3);
                stageTask = GetTask(4);
                break;
            }
        }

        if (hand.Level > 20)
        {
            curse = GetCurse(4);

            if (hand.Level % 3 == 0)
            {
                stageTask = GetTask(4);
            }
        }
    }

    private void CreateDailyLevel()
    {
        hand.ApplySeed();
        
        if (hand.Level == 1)
        {
            if (Random.value < 0.8f)
            {
                PlaceCard(hand.CreateCard(Vector3.zero, GetDailyStarter()));                
            }
            
            return;
        }
        
        if (Random.value < 0.3f)
        {
            stageTask = GetTask(GetDailyDifficulty());
        }

        if (Random.value < 0.3f)
        {
            curse = GetCurse(GetDailyDifficulty());
        }
    }

    private int GetDailyDifficulty()
    {
        return Mathf.Max(hand.Level, Random.Range(0, 5));
    }

    private CardData GetDailyStarter()
    {
        var createFunc = GetDailyStarterFunc();
        return createFunc();
    }

    private Func<CardData> GetDailyStarterFunc()
    {
        return new List<Func<CardData>>
        {
            CardData.Loop,
            () => CardData.GetRandom(0.9f),
            () => CardData.GetRandom(1f),
            () => CardData.GetRandom(1.1f),
            () => CardData.GetRandom(1.2f),
            () => CardData.GetRandom(1.3f),
            CardData.Empty,
            CardData.Starter,
            CardData.StubDown,
            CardData.StubRight,
            CardData.StubLeft,
            CardData.StubUp,
            CardData.StarterWithBomb
        }.Random();
    }

    private Curse GetCurse(int difficulty)
    {
        var curseCreator = GetCurses(difficulty).Random();
        return curseCreator();
    }
    
    private StageTask GetTask(int difficulty)
    {
        var taskCreator = GetTasks(difficulty).Random();
        return taskCreator();
    }

    private List<Func<StageTask>> GetTasks(int difficulty)
    {
        return difficulty switch
        {
            0 => new List<Func<StageTask>>
            {
                () => new TouchTask(2, "two"),
                () => new TouchTask(3, "three"),
                () => new TimedTask(hand.Level, 5),
                () => new ConnectTask(this, new[]
                {
                    new Vector3(-1, 0, 0),
                    new Vector3(1, 0, 0)
                }),
                () => new ConnectTask(this, new[]
                {
                    new Vector3(0, -1, 0),
                    new Vector3(0, 1, 0)
                }),
                () => new ConnectTask(this, new[]
                {
                    new Vector3(-1, -1, 0),
                    new Vector3(1, 1, 0)
                }),
                () => new ConnectTask(this, new[]
                {
                    new Vector3(1, -1, 0),
                    new Vector3(-1, 1, 0)
                })
            },
            1 => new List<Func<StageTask>>
            {
                () => new TouchTask(2, "two"),
                () => new TouchTask(3, "three"),
                () => new TimedTask(hand.Level, 4),
                () => new ConnectTask(this, new[]
                {
                    new Vector3(-2, 0, 0),
                    new Vector3(2, 0, 0)
                })
            },
            2 => new List<Func<StageTask>>
            {
                () => new TouchTask(3, "three"),
                () => new TouchTask(4, "four"),
                () => new TimedTask(hand.Level, 3),
                () => new ConnectTask(this, new[]
                {
                    new Vector3(-2, 0, 0),
                    new Vector3(2, 0, 0),
                    new Vector3(0, 0, 0)
                }),
                () => new ConnectTask(this, new[]
                {
                    new CardPlacement(CardData.StubLeft(), new Vector3(-1, 0, 0)),
                    new CardPlacement(CardData.StubRight(), new Vector3(1, 0, 0))
                }),
                () => new ConnectTask(this, new[]
                {
                    new CardPlacement(CardData.StubUp(), new Vector3(0, 1, 0)),
                    new CardPlacement(CardData.StubDown(), new Vector3(0, -1, 0))
                })
            },
            3 => new List<Func<StageTask>>
            {
                () => new TouchTask(4, "four"),
                () => new TouchTask(5, "five"),
                () => new TimedTask(hand.Level, 3),
                () => new ConnectTask(this, new[]
                {
                    new Vector3(0, -2, 0),
                    new Vector3(0, 0, 0),
                    new Vector3(0, 2, 0)
                }),
                () => new ConnectTask(this, new[]
                {
                    new CardPlacement(CardData.StubUp(), new Vector3(-1, 1, 0)),
                    new CardPlacement(CardData.StubDown(), new Vector3(1, -1, 0))
                }),
                () => new ConnectTask(this, new[]
                {
                    new CardPlacement(CardData.StubUp(), new Vector3(1, 1, 0)),
                    new CardPlacement(CardData.StubDown(), new Vector3(-1, -1, 0))
                })
            },
            _ => new List<Func<StageTask>>
            {
                () => new TouchTask(5, "five"),
                () => new TouchTask(6, "six"),
                () => new TouchTask(7, "seven"),
                () => new TimedTask(hand.Level, 2),
                () => new ConnectTask(this, new[]
                {
                    new Vector3(-1, -1, 0),
                    new Vector3(1, 1, 0),
                    new Vector3(-1, 1, 0),
                    new Vector3(1, -1, 0)
                }),
                () => new ConnectTask(this, new[]
                {
                    new CardPlacement(CardData.StubDown(), new Vector3(2, 2, 0)),
                    new CardPlacement(CardData.StubUp(), new Vector3(-2, -2, 0))
                }),
                () => new ConnectTask(this, new[]
                {
                    new CardPlacement(CardData.StubDown(), new Vector3(-2, 2, 0)),
                    new CardPlacement(CardData.StubUp(), new Vector3(2, -2, 0))
                })
            }
        };
    }

    private List<Func<Curse>> GetCurses(int difficulty)
    {
        return difficulty switch
        {
            0 => new List<Func<Curse>>
            {
                () => new PusherCurse(meanie),
                () => new BlindnessCurse(this, 1),
                () => new DoublerCurse(CardData.Starter(), "cards"),
                () => new RotationCurse(),
                () => new NoPreviewCurse()
            },
            1 => new List<Func<Curse>>
            {
                () => new PusherCurse(meanie),
                () => new BlindnessCurse(this, 2),
                () => new DoublerCurse(CardData.Empty(), "blank cards"),
                () => new DoublerCurse(CardData.Starter(), "cards"),
                () => new DoublerCurse(CardData.StarterWithBomb(), "bombs"),
                () => new RotationCurse(),
                () => new NoPreviewCurse()
                
            },
            2 => new List<Func<Curse>>
            {
                () => new PusherCurse(meanie),
                () => new BlindnessCurse(this, 3),
                () => new DoublerCurse(CardData.Empty(), "blank cards"),
                () => new DoublerCurse(CardData.StarterWithBomb(), "bombs"),
                () => new RotationCurse(),
                () => new NoPreviewCurse()
            },
            3 => new List<Func<Curse>>
            {
                () => new PusherCurse(meanie),
                () => new BlindnessCurse(this, 4),
                () => new DoublerCurse(CardData.Empty(), "blank cards"),
                () => new DoublerCurse(CardData.StarterWithBomb(), "bombs"),
                () => new RotationCurse(),
                () => new NoPreviewCurse()
            },
            _ => new List<Func<Curse>>
            {
                () => new PusherCurse(meanie),
                () => new BlindnessCurse(this, 5),
                () => new DoublerCurse(CardData.Empty(), "blank cards"),
                () => new DoublerCurse(CardData.StarterWithBomb(), "bombs"),
                () => new RotationCurse(),
                () => new NoPreviewCurse()
            }
        };
    }

    public int GetPar(int level)
    {
        if (level == 1) return 30;
        var mod = Mathf.Pow(0.8f, hand.GetPassiveLevel(Passive.SmallerPars));
        var mod5 = Mathf.FloorToInt(level / 5f) + 1;
        var mod10 = Mathf.FloorToInt(level / 10f) * 2 + 1;
        var mod15 = Mathf.FloorToInt(level / 15f) * 8 + 1;
        var mod20 = Mathf.FloorToInt(level / 20f) * 5 + 1;
        return Mathf.RoundToInt(GetPar(level - 1) + level * 30 * mod5 * mod10 * mod15 * mod20 * mod);
    }

    private void UpdateScore()
    {
        shownScore = Mathf.RoundToInt(Mathf.MoveTowards(shownScore, totalScore, scoreScroll));
        totalScoreField.text = shownScore.ToString();
        totalScoreFieldShadow.text = totalScoreField.text;
    }

    public void Place(Card card)
    {
        cards.Add(card);
        card.PlaceEffect();
        
        spinner.Show();
        connectionLines.Hide();

        PlacePipsToGrid(card);

        output.text = grid.DataAsString();
        
        hand.LockCards(true);

        if (HasCurse && curse.GetType() == typeof(DoublerCurse))
        {
            var neighbours = GetFreeNeighboursFor(card).ToList();
            if (neighbours.Any())
            {
                var p = card.transform.position + neighbours.Random();
                var c = hand.CreateCard(p, ((DoublerCurse)curse).CardType, hand.HasPassive(Passive.StarOnGenerated));
                PlaceCard(c, false, true);   
            }
        }

        actionQueue.Add(new ActivateAction(card, 1));

        StartCoroutine(ProcessQueue());
    }

    private IEnumerator ProcessQueue()
    {
        processing = true;
        
        yield return actionQueue.Process(this);

        if (HasCurse)
        {
            yield return curse.Apply(this);
        }
        
        hand.AddTurnScore(turnScore);
        turnScore = 0;

        hand.NextTurn();
        spinner.Hide();

        processing = false;
    }

    public void Activate(Card card, int multi = 1)
    {
        if (!card) return;
        var pos = card.GetCoordinates();

        MarkCardActivated(card);
        
        if (pos.x == 2 && pos.y == 2)
        {
            multi += hand.GetPassiveLevel(Passive.MultiOnCenter) * 5;
        }

        if (pos.y == 4)
        {
            multi += hand.GetPassiveLevel(Passive.MultiOnBottom);
        }
        
        if (pos.y == 0)
        {
            multi += hand.GetPassiveLevel(Passive.MultiOnTop);
        }
        
        if (pos.x == 0)
        {
            multi += hand.GetPassiveLevel(Passive.MultiOnLeft);
        }
        
        if (pos.x == 4)
        {
            multi += hand.GetPassiveLevel(Passive.MultiOnRight);
        }

        if (multi > 1)
        {
            ShowTextAt($"x{multi}", card.transform.position + Vector3.right * 0.3f);
        }
        
        actionQueue.Add(new ScoreAction(card, multi));

        if (card.IsRotator)
        {
            var neighbours = GetNeighboursFor(card).ToList();
            var rotations = hand.HasPassive(Passive.DoubleRotations) ? 2 : 1;
            for (var i = 0; i < rotations; i++)
            {
                actionQueue.Add(new RotateAction(neighbours, card.RotatesClockwise, card));
                neighbours.ForEach(n => actionQueue.Add(new ActivateAction(n, multi + 1)));   
            }
        }

        if (card.IsPusher)
        {
            var p = card.transform.position;
            var megaMode = hand.HasPassive(Passive.MegaPush);
            var neighbours = megaMode ? GetAxisCards(card).ToList() : GetNeighboursFor(card).ToList();
            actionQueue.Add(new PushAction(neighbours, p, card, megaMode));
            neighbours.ForEach(n => actionQueue.Add(new ActivateAction(n, multi + 1)));
        }
        
        if (card.IsPuller)
        {
            var megaMode = hand.HasPassive(Passive.MegaPull);
            var neighbours = megaMode ? GetAxisCards(card).ToList() : GetNeighboursFor(card, 2).ToList();
            actionQueue.Add(new PullAction(neighbours, card, true));
            neighbours.ForEach(n => actionQueue.Add(new ActivateAction(n, multi + 1)));
        }
    }

    public void TurnStart()
    {
        if (HasTask() && !TaskComplete())
        {
            var done = stageTask.Update(this);
            if (done)
            {
                hand.ShowMessage("Nice! You just (completed) the (special task) for this (stage).", true);
            }
        }
        
        turnActivatedCards.Clear();
        PreviousTouched = 0;
    }

    private void MarkCardActivated(Card card)
    {
        if (!turnActivatedCards.Contains(card))
        {
            turnActivatedCards.Add(card);
        }
    }

    public bool WasCardActivated(Card card)
    {
        return turnActivatedCards.Contains(card);
    } 

    public IEnumerator ScoreCard(Card card, int multi)
    {
        if (!card) yield break;

        OnCardActivation(card);
        
        var allVisited = new List<Pip>();
        var pips = card.GetPoints().ToList();
        foreach (var pip in pips)
        {
            if (allVisited.Contains(pip)) continue;

            var visited = new List<Pip>();
            Fill(pip, visited, 0);
            visited = visited.Distinct().OrderBy(p => p.pathIndex).ToList();
            yield return MarkCoroutine(visited, multi);
            allVisited.AddRange(visited);
        }
    }

    public void OnCardActivation(Card card)
    {
        if (!card) return;
        
        card.SetBorderColorTo(Color.black);
        
        if (!hand.IsFirstTurn && hand.HasPassive(Passive.Orphanizer) && !GetNeighboursFor(card, 1, true).Any())
        {
            actionQueue.Add(new DestroyAction(card, 1));
        }
    }

    public void RemoveCard(Card card, int multiplier)
    {
        if (!card) return;
        EffectManager.AddEffects(new []{ 0, 1, 3 }, card.transform.position);
        EffectManager.AddEffect(2, card.GetExplosionPosition());
        cam.BaseEffect(0.4f);
        ClearPipsFromGrid(card);
        cards.Remove(card);
        
        AudioManager.Instance.PlayEffectAt(4, transform.position, 1.5f);

        if (hand.HasPassive(Passive.Detonator))
        {
            var neighbours = GetNeighboursFor(card, 1, true).ToList();
            neighbours.ForEach(n => actionQueue.Add(new ActivateAction(n, multiplier)));
        }

        if (hand.HasPassive(Passive.Replacement))
        {
            var next = hand.GetRandomCard(card.GetPositionBeforeShaking());
            PlaceCard(next, false, true);
            actionQueue.Add(new ActivateAction(next, 1));
        }
        
        Destroy(card.gameObject);
    }

    private void PlaceCard(Card card, bool mark = false, bool doEffect = false)
    {
        card.draggable.enabled = false;
        card.hoverer.enabled = false;
        card.SetMarking(mark);
        PlacePipsToGrid(card);
        cards.Add(card);
        if (doEffect)
        {
            card.PlaceEffect();
        }
        output.text = grid.DataAsString();
    }

    private void PlacePipsToGrid(Card card)
    { 
        var pips = card.GetPoints(true).ToList();
        pips.ForEach(pip => grid.Set(pip, pip.x, pip.y));
    }

    private void ClearPipsFromGrid(Card card)
    {
        var pips = card.GetPoints(false).ToList();
        pips.ForEach(pip =>
        {
            grid.Set(null, pip.x, pip.y); 
        });
    }

    private IEnumerable<Card> GetAxisCards(Card card)
    {
        var pos = card.GetMirroredCoordinates();
        var dirs = card.GetDirections();
        return cards.Where(c => c != card && c.IsOnSameAxisAs(pos, dirs));
    }

    public IEnumerable<Card> GetCardsOnAxisY(int y)
    {
        return cards.Where(c => c.IsOnSameAxisY(y));
    }
    
    public IEnumerable<Card> GetCardsOnAxisX(int x)
    {
        return cards.Where(c => c.IsOnSameAxisX(x));
    }

    private IEnumerable<Card> GetNeighboursFor(Card card, int distance = 1, bool all = false)
    {
        var pos = card.transform.position;
        return card.GetDirections(all).Select(dir => GetNeighbourFor(pos, dir, distance)).Where(c => c != null);
    }

    private IEnumerable<Vector3> GetFreeNeighboursFor(Card card)
    {
        var pos = card.transform.position;
        return card.GetDirections(true).Where(dir => IsOnArea(pos + dir) && !GetNeighbourFor(pos, dir, 1));
    }
    
    private Card GetNeighbourFor(Vector3 pos, Vector3 dir, int maxSteps = 1)
    {
        for (var i = 1; i <= maxSteps; i++)
        {
            var diff = (i - 0.25f) * dir;
            var target = Physics2D.OverlapCircle(pos + diff, 0.1f, cardLayer);
            if (target)
            {
                return target.GetComponent<Card>();
            }
        }

        return null;
    }

    private bool IsOnArea(Vector3 pos)
    {
        return Physics2D.OverlapCircle(pos, 0.1f, fieldLayer);
    }

    public void Rotate(Card card, bool clockwise)
    {
        if (!card) return;
        ClearPipsFromGrid(card);
        card.Rotate(clockwise);
        card.ResetBombs();
        PlacePipsToGrid(card);

        if (hand.HasPassive(Passive.StarOnRotate))
        {
            card.MakeRandomStar();
        }
    }

    private IEnumerator MarkCoroutine(List<Pip> pips, int multi)
    {
        yield return MarkPip(pips, true, multi);
        yield return new WaitForSeconds(0.5f);
        yield return MarkPip(pips, false, 0);
    }

    private IEnumerator MarkPip(List<Pip> pips, bool state, int startMulti)
    {
        if (!pips.Any()) yield break;
        
        var total = 0;
        var sum = Vector3.zero;
        var amount = 0;
        var multi = startMulti;
        
        var pos = pips.First().sprite.transform.position;
        var pop = state ? Instantiate(scorePopPrefab, pos, Quaternion.identity) : null;
        
        var postMulti = 1;
        
        if (hand.HasPassive(Passive.Tactician))
        {
            postMulti = hand.IsEvenTurn ? 3 : 0;
        }

        foreach (var pip in pips)
        {
            if (pip == null) continue;

            var targetColor = state ? markColor : Color.black;
            var targetSize = Vector3.one * (state ? 0.18f : 0.15f);
            const float duration = 0.15f;
            Tweener.ColorToBounceOut(pip.sprite, targetColor, duration);
            Tweener.ScaleToBounceOut(pip.sprite.transform, targetSize, duration);
            
            if (amount % 2 == 0)
            {
                PlayRandomNote(pos);
            }

            if (state)
            {
                var skipBombActivation = false;
                
                MarkCardActivated(pip.GetCard());
                
                if (pip.isStar)
                {
                    multi += 1;
                    ShowTextAt($"x{multi}", pip.sprite.transform.position + Vector3.right * 0.3f);
                    
                    PlayRandomNote(pos);
                    AudioManager.Instance.PlayEffectAt(5, pos, 1f);

                    if (hand.HasPassive(Passive.BombTransformer))
                    {
                        pip.MakeBomb();
                        skipBombActivation = true;
                    }
                }

                if (pip.isBomb && !skipBombActivation)
                {
                    PlayRandomNote(pos);
                    
                    if(pip.isShaking)
                    {
                        actionQueue.Add(new DestroyAction(pip.GetCard(), multi));
                        pip.GetCard().Shake();
                    }
                    else
                    {
                        pip.StartShaking();
                        pip.isShaking = true;
                    }
                    
                    if (hand.HasPassive(Passive.StarTransformer))
                    {
                        pip.GetCard().ResetBombs();
                        pip.MakeStar();
                    }
                }

                total++;
                pop.SetText((total * multi * postMulti).ToString(CultureInfo.InvariantCulture));
                var pt = pop.transform;
                sum += pip.sprite.transform.position;
                amount++;
                pt.position = sum / amount;
            }

            CheckPipTransforms(total, pip);

            yield return new WaitForSeconds(0.02f);
        }
        
        AddScore(total * multi * postMulti);

        if (pop)
        {
            StartCoroutine(DestroyAfter(pop.gameObject));
        }
    }

    private void PlayRandomNote(Vector3 pos)
    {
        notePos += Random.Range(-1, 2);
        var note = notePos.LoopAround(0, noteSounds.Count);
        AudioManager.Instance.PlayEffectAt(noteSounds.At(note), pos, 0.2f, false);
    }

    private void CheckPipTransforms(int total, Pip pip)
    {
        if (hand.HasPassive(Passive.Bomberman))
        {
            if ((total + 1) % 20 == 0)
            {
                PlayRandomNote(pip.GetCard().transform.position);
                pip.MakeBomb();
            }
        }

        if (hand.HasPassive(Passive.Starchild))
        {
            if ((total + 1) % 30 == 0)
            {
                PlayRandomNote(pip.GetCard().transform.position);
                pip.MakeStar();
            }
        }
    }

    private void AddScore(int amount)
    {
        scoreScroll = Mathf.CeilToInt(amount * 0.05f);
        totalScore += amount;
        hand.SetScore(totalScore);
        turnScore += amount;
    }

    private void ShowTextAt(string text, Vector3 pos, float scale = 1f, int sortOrder = 1, float hideAfter = 1f)
    {
        var pop = Instantiate(scorePopPrefab, pos, Quaternion.identity);
        pop.transform.localScale *= scale;
        pop.SetText(text);
        pop.SetSortOrder(sortOrder);
        StartCoroutine(DestroyAfter(pop.gameObject, hideAfter));
    }

    private static IEnumerator DestroyAfter(GameObject pop, float delay = 0.5f)
    {
        yield return new WaitForSeconds(delay);
        Destroy(pop);
    }

    private void Fill(Pip pip, List<Pip> visited, int index)
    {
        visited.Add(pip);

        if (pip.isBomb && pip.isShaking) return;

        pip.pathIndex = index;
        
        grid.GetNeighbours(pip.x, pip.y)
            .Where(n => n != null && !visited.Contains(n))
            .OrderBy(_ => Random.value)
            .ToList()
            .ForEach(n =>
        {
            Fill(n, visited, index + 1); 
        });
    }

    public void Preview(Card card, Vector3 pos)
    {
        connectionLines.Hide();
        connectionLines.MovePreview(pos);
        
        var pips = card.GetAllPips().ToList();
        PreviousTouched = 0;
        
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
            PreviousTouched++;
        }
    }

    public void HidePreview()
    {
        connectionLines.Hide();
    }

    public void Move(Card card, Vector3 direction, Passive makeStarIf = Passive.None, bool ignoreBlocks = false)
    {
        if (!card) return;
        var p = card.transform.position;
        AudioManager.Instance.PlayEffectAt(3, p, 1f);
        var blocked = !ignoreBlocks && GetNeighbourFor(p, direction);
        if (!blocked && IsOnArea(p + direction))
        {
            StartCoroutine(MoveCoroutine(card, direction, makeStarIf));
        }
    }

    private IEnumerator MoveCoroutine(Card card, Vector3 direction, Passive makeStarIf = Passive.None)
    {
        card.ResetBombs();
        ClearPipsFromGrid(card);
        var t = card.transform;
        Tweener.MoveToBounceOut(t, t.position + direction, 0.3f);
        yield return new WaitForSeconds(0.35f);
        card.PositionPips();
        PlacePipsToGrid(card);

        if (hand.HasPassive(makeStarIf))
        {
            card.MakeRandomStar();
        }
        
        output.text = grid.DataAsString();
    }

    public void Tilt()
    {
        hand.TriggerTutorial(BaseTutorial.Tilt);
        ShowTextAt("TILT", cam.cameraRig.position.WhereZ(0), 3f, 2);
        actionQueue.Clear();
        cam.BaseEffect(0.4f);
        AudioManager.Instance.PlayEffectAt(6, Vector3.zero, 2f);
    }

    public int GetStackSize()
    {
        return 40 + 40 * hand.GetPassiveLevel(Passive.StackSize);
    }

    public bool IsFull()
    {
        return cards.Count >= 25;
    }

    public Card CreateAndPlaceCard(Vector3 pos, CardData data)
    {
        var card = hand.CreateCard(pos, data);
        PlaceCard(card);
        card.SetMarking(true);
        return card;
    }

    public bool HasTask()
    {
        return stageTask != null;
    }

    public bool TaskComplete()
    {
        return stageTask.IsCompleted;
    }

    public void ApplyCurseToDrawnCard(Card card)
    {
        if (HasCurse && curse.GetType() == typeof(RotationCurse))
        {
            card.Rotate(Random.value < 0.5f);   
        }
    }

    public void StartTimer()
    {
        if (HasTask() && stageTask.GetType() == typeof(TimedTask))
        {
            StartCoroutine(UpdateTimer());   
        }
    }

    public void StopTimer()
    {
        if (!HasTask() || stageTask.GetType() != typeof(TimedTask)) return;
        var task = (TimedTask)stageTask;
        if (!task.HasTime()) return;
        task.MarkDone();
        hand.ShowMessage($"Nice! You (finished) the stage with ({task.GetTimeLeft()}) left to spare.", true);
    }
    
    private IEnumerator UpdateTimer()
    {
        var task = (TimedTask)stageTask;
        while (task.HasTime())
        {
            if (!processing && !hand.IsFirstTurn && !task.IsCompleted)
            {
                task.TickDown();
                ShowTaskOrPar();
            }

            yield return new WaitForSeconds(1f);
        }
        
        hand.FailStage();
    }
}