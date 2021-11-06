using System;
using Save;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PassiveIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TMP_Text letter;

    private PassiveDetails details;
    private PassiveTooltip tooltip;
    
    public void Setup(Passive passive, PassiveTooltip tooltipComponent)
    {
        tooltip = tooltipComponent;
        details = Passives.GetDetails(passive);
        letter.text = details.name.Substring(0, 1);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.Show(transform.position, details);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.Hide();
    }
}