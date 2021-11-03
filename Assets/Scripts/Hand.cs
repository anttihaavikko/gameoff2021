using System;
using System.Collections.Generic;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Utils;
using Save;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Hand : MonoBehaviour
{
    [SerializeField] private Card cardPrefab;
    [SerializeField] private Field field;
    [SerializeField] private Appearer cardPicks;
    [SerializeField] private GameObject drawPile;
    [SerializeField] private TMP_Text drawPileNumber;

    private Card card;
    private Deck deck;

    private void Start()
    {
        CreateDeck();
        deck.Shuffle();
        UpdateDrawPile();
        
        Invoke(nameof(AddCard), 1.5f);
    }

    private void CreateDeck()
    {
        if (Saver.Exists())
        {
            deck = Saver.Load<Deck>();
            return;
        }
        
        deck = new Deck();
        
        for (var i = 0; i < 5; i++)
        {
            deck.Add(CardData.Starter());
        }
        
        Saver.Save(deck);
    }

    private void Update()
    {
        DebugControls();
    }

    private void DebugControls()
    {
        if (!Application.isEditor) return;

        if (Input.GetKeyDown(KeyCode.A))
        {
            deck.Add(CardData.Random());
            Saver.Save(deck);
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneChanger.Instance.ChangeScene("Main");
        }
        
        if (Input.GetKeyDown(KeyCode.D))
        {
            Saver.Clear();
            SceneChanger.Instance.ChangeScene("Main");
        }
    }

    private void AddCard()
    {
        if (deck.IsEmpty)
        {
            Invoke(nameof(CreateOptions), 1.5f);
            return;
        }

        var cardData = deck.Draw();
        UpdateDrawPile();
        
        card = Instantiate(cardPrefab, drawPile.transform.position, Quaternion.identity);
        card.Setup(cardData);
        card.draggable.dropped += CardMoved;
        card.draggable.preview += ConnectionPreview;
        card.draggable.hidePreview += HidePreview;
        
        Tweener.MoveToBounceOut(card.transform, transform.position, 0.3f);
    }

    private void UpdateDrawPile()
    {
        var amount = deck.GetCount();
        drawPileNumber.text = amount.ToString();

        if (amount == 0)
        {
            drawPile.SetActive(false);
        }
    }

    private void HidePreview()
    {
        field.HidePreview();
    }

    private void ConnectionPreview(Vector2 pos)
    {
        field.Preview(card, pos);
    }

    private void CardMoved()
    {
        card.draggable.dropped -= CardMoved;
        card.draggable.preview -= ConnectionPreview;
        card.draggable.hidePreview -= HidePreview;
        field.Place(card);
        AddCard();
    }

    private void CreateOptions()
    {
        cardPicks.Show();
        
        for (var i = 0; i < 3; i++)
        {
            var pos = cardPicks.transform.position + Vector3.left + Vector3.right * i;
            card = Instantiate(cardPrefab, pos, Quaternion.identity);
            var data = CardData.Random();
            card.Setup(data);
            card.draggable.CanDrag = false;
            card.draggable.click += () =>
            {
                card.draggable.click = null;
                deck.Add(data);
                Saver.Save(deck);
                SceneChanger.Instance.ChangeScene("Main");
            };
        }
    }
}