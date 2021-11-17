using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Leaderboards
{
    public class ScoreRow : MonoBehaviour
    {
        public TMP_Text namePart, scorePart, nameShadow, scoreShadow;
        public RawImage flag;
    
        public void Setup(string nam, string sco, string locale)
        {
            namePart.text = nameShadow.text = nam;
            scorePart.text = scoreShadow.text = sco;
            FlagManager.SetFlag(flag, locale);
        }
    }
}
