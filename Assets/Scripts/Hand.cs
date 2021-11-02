using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Hand : MonoBehaviour
{
    [SerializeField] private Card cardPrefab;
    [SerializeField] private Field field;

    private Card card;

    private void Start()
    {
        AddCard();
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
            AddCard();
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadSceneAsync("Main");
        }
    }

    private void AddCard()
    {
        card = Instantiate(cardPrefab, transform.position, Quaternion.identity);
        card.draggable.dropped += CardMoved;
    }

    private void CardMoved()
    {
        card.draggable.dropped -= CardMoved;
        field.Place(card);
        AddCard();
    }
}