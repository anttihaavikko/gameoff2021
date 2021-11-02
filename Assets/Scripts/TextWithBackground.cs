using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextWithBackground : MonoBehaviour
{
    public TMP_Text textActual, textBg;

    public void SetText(string text)
    {
        textBg.text = "<mark=#000000 padding='20, 20, 10, 10'>" + text + "</mark>";
        textActual.text = text;
    }
}
