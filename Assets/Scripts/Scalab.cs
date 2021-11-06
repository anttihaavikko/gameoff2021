using System;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using UnityEngine;

public class Scalab : MonoBehaviour
{
    [SerializeField] private Animator anim;
    
    private static readonly int Walking = Animator.StringToHash("walking");

    private void Start()
    {
        Invoke(nameof(Move), 2f);
    }

    private void Move()
    {
        var t = transform;
        var p = t.position;
        var target = Vector3.zero.RandomOffset(1.5f) + Vector3.down * 2;
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
}