using Save;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PassivePanel : MonoBehaviour
{
    [SerializeField] private TMP_Text title, description;
    
    public Button pickButton;

    public void SetDetails(PassiveDetails details, int level)
    {
        title.text = details.name;
        description.text = TextFormatter.DoColors(details.description.Replace("[LEVEL]", level.ToString()));
    }
}