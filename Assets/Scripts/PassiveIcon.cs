using Save;
using TMPro;
using UnityEngine;

public class PassiveIcon : MonoBehaviour
{
    [SerializeField] private TMP_Text letter;
    
    public void Setup(Passive passive)
    {
        var details = Passives.GetDetails(passive);
        letter.text = details.name.Substring(0, 1);
    }
}