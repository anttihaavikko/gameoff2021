using Save;
using UnityEngine;

public class PassiveTooltip : MonoBehaviour
{
    [SerializeField] private PassivePanel content;
    [SerializeField] private Vector3 offset;
    
    public void Show(Vector3 pos, Passive passive, PassiveDetails details, int level)
    {
        content.gameObject.SetActive(true);
        transform.position = pos + offset;
        content.SetDetails(passive, details, level);
    }

    public void Hide()
    {
        content.gameObject.SetActive(false);
    }
}