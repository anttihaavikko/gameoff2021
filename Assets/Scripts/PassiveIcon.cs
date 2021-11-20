using System;
using AnttiStarterKit.ScriptableObjects;
using Save;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PassiveIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TMP_Text letter;
    [SerializeField] private SpriteCollection sprites;
    [SerializeField] private Image icon, iconShadow;
    
    private PassiveDetails details;
    private PassiveTooltip tooltip;
    private Passive passive;
    private int level;
    
    public void Setup(Passive passive, PassiveTooltip tooltipComponent, int lvl)
    {
        tooltip = tooltipComponent;
        details = Passives.GetDetails(passive);
        this.passive = passive;
        letter.text = details.name.Substring(0, 1);
        level = lvl;
        icon.sprite = iconShadow.sprite = sprites.Get((int)passive);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.Show(transform.position, passive, details, level);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.Hide();
    }
}