using System;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

public class Scalab : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private SpeechBubble speechBubble;

    private static readonly int Walking = Animator.StringToHash("walking");

    private Vector3 start;
    private Tutorial<BaseTutorial> tutorial;

    private void Start()
    {
        tutorial = new Tutorial<BaseTutorial>("Tutorial");
        start = transform.position;
        Invoke(nameof(Move), 2f);
        Invoke(nameof(ShowIntro), 1.2f);

        tutorial.onShow += ShowTutorial;
    }

    private void ShowTutorial(BaseTutorial tut)
    {
        var message = GetTutorialText(tut);
        speechBubble.Show(message);
    }

    private string GetTutorialText(BaseTutorial tut)
    {
        return tut switch
        {
            BaseTutorial.Intro => "I'm (Scalab) and I'll help you get started with playing!",
            BaseTutorial.PlaceHelp => "Place cards on the field to (connect) as many (pips) as possible.",
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
    }
}

public enum BaseTutorial
{
    Intro,
    PlaceHelp
}