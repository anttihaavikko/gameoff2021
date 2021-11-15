using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Cloud : MonoBehaviour
{
    [SerializeField] private List<Transform> balls;

    private List<float> offsets;

    private void Start()
    {
        offsets = new List<float>();
        balls.ForEach(b =>
        {
            b.localScale *= Random.Range(0.1f, 2f);
            offsets.Add(Random.Range(0f, 1000f));
        });
    }

    private void Update()
    {
        var i = 0;
        balls.ForEach(b =>
        {
            const float mod = 0.75f;
            const float timeScale = 0.5f;
            var x = Mathf.PerlinNoise(offsets[i], Time.time * timeScale) * mod;
            var y = Mathf.PerlinNoise(Time.time * timeScale, offsets[i]) * mod;
            b.localPosition = new Vector3(x, y, 0);
            i++;
        });
    }
}