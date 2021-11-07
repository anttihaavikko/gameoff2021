using System;
using System.Collections.Generic;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Utils;
using UnityEngine;

public class HandMover : MonoBehaviour
{
    [SerializeField] private List<TransformState> hands;
    [SerializeField] private float amount, timeScale, verticalAmount;

    private void LateUpdate()
    {
        hands.ForEach(Offset);
    }

    private void Offset(TransformState t)
    {
        var x = Mathf.PerlinNoise(t.Position.x, Time.time * timeScale);
        var y = Mathf.PerlinNoise(t.Position.y, Time.time * timeScale);
        t.transform.localPosition = t.LocalPosition + new Vector3(x * amount, y * verticalAmount, 0);
    }
}