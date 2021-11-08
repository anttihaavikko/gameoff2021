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
    private int level;
    
    public void Setup(Passive passive, PassiveTooltip tooltipComponent, int lvl)
    {
        tooltip = tooltipComponent;
        details = Passives.GetDetails(passive);
        letter.text = details.name.Substring(0, 1);
        level = lvl;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.Show(transform.position, details, level);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.Hide();
    }
}