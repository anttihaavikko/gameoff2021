using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Extensions;
using Save;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CardPreview : MonoBehaviour
{
    [SerializeField] private List<Image> points;
    [SerializeField] private Sprite starSprite, bombSprite, circleSprite;
    [SerializeField] private GameObject rotateIcon, pushIcon, pullIcon;
    [SerializeField] private List<GameObject> directionIndicators;
    [SerializeField] private bool randomize;
    
    private CardData data;

    private void Start()
    {
        if (randomize)
        {
            Setup(CardData.GetRandom());
        }
    }

    public void Setup(CardData cardData)
    {
        data = cardData.Clone();

        if (data.IsRotator)
        {
            rotateIcon.SetActive(true);
            ActivateDirections();
        }
        
        if (data.type == CardType.RotateLeft)
        {
            rotateIcon.transform.Mirror();
        }

        if (data.type == CardType.Push)
        {
            pushIcon.SetActive(true);
            ActivateDirections();
        }
        
        if (data.type == CardType.Pull)
        {
            pullIcon.SetActive(true);
            ActivateDirections();
        }

        PositionPips();
    }
    
    public void PositionPips()
    {
        points.Where((_, i) => data.pips.Contains(i)).ToList().ForEach(p =>
        {
            var i = points.IndexOf(p);
            p.gameObject.SetActive(true);

            var isStar = data.stars.Contains(i);
            if (isStar)
            {
                p.sprite = starSprite;
                p.transform.Rotate(0, 0, Random.Range(0f, 360f));
            }

            var isBomb = !isStar && data.bombs.Contains(i);
            if (isBomb)
            {
                p.sprite = bombSprite;
                p.transform.Rotate(0, 0, Random.Range(-40f, 40f));
            }
        });
    }
    
    private void ActivateDirections()
    {
        data.directions.ForEach(d => directionIndicators[d].SetActive(true));
    }
}