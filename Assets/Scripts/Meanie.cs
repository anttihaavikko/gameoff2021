using System;
using AnttiStarterKit.Animations;
using UnityEngine;
using AnttiStarterKit.Extensions;

public class Meanie : MonoBehaviour
{
    [SerializeField] private Animator anim;
    
    private static readonly int Walking = Animator.StringToHash("walking");
    private static readonly int Pushing = Animator.StringToHash("push");

    public void MoveTo(Vector3 target, float duration = 1f)
    {
        var t = transform;
        var p = t.position;
        SetWalking(true, Vector3.Distance(p, target) * 0.9f);
        Tweener.MoveTo(t, target, duration, TweenEasings.SineEaseInOut);
        this.StartCoroutine(() => SetWalking(false), 0.8f * duration);
    }
    
    private void SetWalking(bool state, float speed = 1f)
    {
        anim.speed = speed;
        anim.SetBool(Walking, state);
    }

    public void Push()
    {
        anim.SetTrigger(Pushing);
    }
}