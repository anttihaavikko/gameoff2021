using AnttiStarterKit.ScriptableObjects;
using Save;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PassivePanel : MonoBehaviour
{
    [SerializeField] private TMP_Text title, description;
    [SerializeField] private Image icon;
    [SerializeField] private SpriteCollection sprites;

    public Button pickButton;

    public void SetDetails(Passive passive, PassiveDetails details, int level)
    {
        title.text = details.name;
        description.text = TextFormatter.DoColors(details.description.Replace("[LEVEL]", level.ToString()));
        icon.sprite = sprites.Get((int)passive);
    }
}