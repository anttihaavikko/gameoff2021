using System;
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
        var amount = 2 + save.GetPassiveLevel(Passive.Options);
        
        Passives.GetRandom(save.passives, amount).ToList().ForEach(passive =>
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

    private void Update()
    {
        DevKeys();
    }

    private static void DevKeys()
    {
        if (!Application.isEditor) return;
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneChanger.Instance.ChangeScene("Pick");
        }
    }
}