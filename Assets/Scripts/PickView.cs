using System.Linq;
using Save;
using UnityEngine;

public class PickView : MonoBehaviour
{
    [SerializeField] private PassivePanel panelPrefab;
    [SerializeField] private Transform container;

    private SaveData save; 

    private void Start()
    {
        save = SaveData.LoadOrCreate();

        Passives.GetRandom(save.passives, 2).ToList().ForEach(passive =>
        {
            var details = Passives.GetDetails(passive);
            var panel = Instantiate(panelPrefab, container);
            panel.SetDetails(details);
            panel.pickButton.onClick.AddListener(() =>
            {
                save.passives.Add(passive);
                save.Save();
                panel.pickButton.onClick.RemoveAllListeners();
                SceneChanger.Instance.ChangeScene("Main");
            });
        });
        
        
    }
}