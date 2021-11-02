using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Card : MonoBehaviour
{
    public Draggable draggable;
    
    [SerializeField] private List<SpriteRenderer> points;

    private void Start()
    {
        points.ForEach(p => p.gameObject.SetActive(Random.value < 0.5f));
    }
}
