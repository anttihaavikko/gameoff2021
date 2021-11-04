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
    private SaveData save;
    
    public bool HasPassive(Passive passive) => save.HasPassive(passive);
    public int GetPassiveLevel(Passive passive) => save.GetPassiveLevel(passive);

    private void Start()
    {
        save = SaveData.LoadOrCreate();
        save.deck.Shuffle();
        UpdateDrawPile();
        
        Invoke(nameof(AddCard), 1.5f);
        
        print($"Current passives: {string.Join(", ", save.passives)}");
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
            save.deck.Add(CardData.GetRandom());
            save.Save();
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
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            SceneChanger.Instance.ChangeScene("Pick");
        }
    }

    private void AddCard()
    {
        if (save.deck.IsEmpty)
        {
            Invoke(nameof(CreateOptions), 1.5f);
            return;
        }

        var cardData = save.deck.Draw();
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
        var amount = save.deck.GetCount();
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
            var data = CardData.GetRandom();
            card.Setup(data);
            card.draggable.CanDrag = false;
            card.draggable.click += () =>
            {
                card.draggable.click = null;
                save.deck.Add(data);
                save.Save();
                SceneChanger.Instance.ChangeScene("Pick");
            };
        }
    }
}