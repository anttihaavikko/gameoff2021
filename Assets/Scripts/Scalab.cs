using System;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using UnityEngine;

public class Scalab : MonoBehaviour
{
    private void Start()
    {
        Invoke(nameof(Move), 2f);
    }

    private void Move()
    {
        var t = transform;
        var p = t.position;
        Tweener.MoveTo(t, Vector3.zero.RandomOffset(2f), 0.7f, TweenEasings.SineEaseInOut);
        Invoke(nameof(Move), 3f);
    }
}