using System;
using UnityEngine;

public class DeckPreviewer : MonoBehaviour
{
    [SerializeField] private Hand hand;
    
    private void OnMouseUp()
    {
        hand.ToggleDeckPreview();
    }
}