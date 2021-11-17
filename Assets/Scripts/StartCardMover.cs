using System;
using System.Collections.Generic;
using AnttiStarterKit.Animations;
using UnityEngine;
using Random = UnityEngine.Random;

public class StartCardMover : MonoBehaviour
{
    [SerializeField] private List<Transform> cards;
    [SerializeField] private int direction;

    private void Start()
    {
        cards.ForEach(c =>
        {
            var duration = Random.Range(0.2f, 1f);
            var angle = Random.Range(0f, 360f);
            var t = c.transform;
            var p = t.position;
            t.position += Vector3.right * direction * 150f;
            Tweener.MoveToBounceOut(t, p, duration);
            Tweener.RotateToBounceOut(t, Quaternion.Euler(0, 0, angle), duration);
        });
    }
}