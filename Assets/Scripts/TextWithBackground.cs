using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class TextWithBackground : MonoBehaviour
{
    [SerializeField] private SortingGroup sortingGroup;
    
    public TMP_Text textActual, textBg, textShadow;

    public void SetText(string text)
    {
        textBg.text = "<mark=#000000 padding='20, 20, 10, 10'>" + text + "</mark>";
        textActual.text = text;
        textShadow.text = text;
    }

    public void SetSortOrder(int order)
    {
        sortingGroup.sortingOrder = order;
    }
}
