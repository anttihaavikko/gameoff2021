using System;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Managers;
using AnttiStarterKit.ScriptableObjects;
using AnttiStarterKit.Utils;
using Save;
using UnityEngine;
using Random = UnityEngine.Random;

public class Scalab : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private SpeechBubble speechBubble;
    [SerializeField] private SoundCollection speechSounds;

    private static readonly int Walking = Animator.StringToHash("walking");

    private Vector3 start;
    private Tutorial<BaseTutorial> tutorial;
    private Tutorial<Passive> skillTutorials;
    
    private readonly string[] skillStarters =
    {
        "The (code) you just picked up",
        "That piece of (code)",
        "That there (code)",
        "That new (code)",
        "That (code) is interesting, it"
    };

    private void Start()
    {
        tutorial = new Tutorial<BaseTutorial>("Tutorial");
        skillTutorials = new Tutorial<Passive>("SkillTutorial");
        start = transform.position;
        Invoke(nameof(Move), 2f);
        Invoke(nameof(ShowIntro), 1.2f);

        tutorial.onShow += ShowTutorial;
        skillTutorials.onShow += ShowTutorial;

        speechBubble.onWord += PlaySpeechSound;
    }

    public void TriggerTutorial(BaseTutorial tut)
    {
        tutorial.Show(tut);
    }
    
    public void TriggerTutorial(Passive passive)
    {
        skillTutorials.Show(passive);
    }

    private void ShowTutorial(BaseTutorial tut)
    {
        var message = GetTutorialText(tut);
        var force = tut == BaseTutorial.NiceKeepGoing || tut == BaseTutorial.NotQuite;
        speechBubble.Show(message, force);
        AudioManager.Instance.PlayEffectAt(3, speechBubble.transform.position, 1f);
    }
    
    private void ShowTutorial(Passive passive)
    {
        var details = Passives.GetDetails(passive);
        var start = skillStarters.Random();
        speechBubble.Show($"{start} {details.tutorial}", false);
        AudioManager.Instance.PlayEffectAt(3, speechBubble.transform.position, 1f);
    }

    public void ShowMessage(string message, bool forced)
    {
        speechBubble.Show(message, forced);
        AudioManager.Instance.PlayEffectAt(3, speechBubble.transform.position, 1f);
    }

    private string GetTutorialText(BaseTutorial tut)
    {
        return tut switch
        {
            BaseTutorial.Intro => "I'm (Scalab) and I'll help you get started with playing!",
            BaseTutorial.PlaceHelp => "Place cards on the field to (connect) as many (pips) as possible.",
            BaseTutorial.NiceKeepGoing => "Nicely done! Keep going, the (par) for this stage is (30) points.",
            BaseTutorial.NotQuite => "Not quite! Place the card so that the (pips connect) to others.",
            BaseTutorial.Tilt => "Wow! There was a (buffer overflow) and it caused the system to (tilt).",
            BaseTutorial.DeckPreview => "You can (preview) the (draw pile) contents by clicking on it.",
            BaseTutorial.NewCard => "Cool cool! Now you can pick a (new card) to add to your (deck).",
            _ => throw new ArgumentOutOfRangeException(nameof(tut), tut, null)
        };
    }

    private void ShowIntro()
    {
        tutorial.Show(BaseTutorial.Intro);
        tutorial.Show(BaseTutorial.PlaceHelp);
    }

    private void Update()
    {
        if (!Application.isEditor) return;

        if (Input.GetKeyDown(KeyCode.T))
        {
            start = start.WhereX(-start.x);
        }
    }

    private void Move()
    {
        var t = transform;
        var p = t.position;
        var target = start.RandomOffset(1f);
        SetWalking(true, Vector3.Distance(p, target) * 0.9f);
        Tweener.MoveTo(t, target, 0.7f, TweenEasings.SineEaseInOut);
        this.StartCoroutine(() => SetWalking(false), 0.6f);
        Invoke(nameof(Move), 3f);
    }

    private void SetWalking(bool state, float speed = 1f)
    {
        anim.speed = speed;
        anim.SetBool(Walking, state);
    }

    public void ResetTutorials()
    {
        tutorial.Clear();
        skillTutorials.Clear();
    }

    public void MarkTutorial(BaseTutorial tut)
    {
        tutorial.Mark(tut);
    }

    private void PlaySpeechSound()
    {
        AudioManager.Instance.PlayEffectAt(speechSounds.Random(), transform.position, 2f);
    }
}

public enum BaseTutorial
{
    Intro,
    PlaceHelp,
    NiceKeepGoing,
    NotQuite,
    Tilt,
    DeckPreview,
    NewCard
}