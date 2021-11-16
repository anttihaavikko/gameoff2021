using System;
using System.Linq;
using Save;
using UnityEngine;

public class PickView : MonoBehaviour
{
    [SerializeField] private PassivePanel panelPrefab;
    [SerializeField] private Transform container;
    [SerializeField] private RectTransform scrollContent;

    private SaveData save; 

    private void Start()
    {
        save = SaveData.LoadOrCreate();
        var amount = 2 + save.GetPassiveLevel(Passive.Options);
        
        Passives.GetRandom(save.passives, amount).ToList().ForEach(AddPassive);
        scrollContent.sizeDelta = new Vector2 (265 * amount, 300);
    }

    private void AddPassive(Passive passive)
    {
        var details = Passives.GetDetails(passive);
        var panel = Instantiate(panelPrefab, container);
        panel.SetDetails(details, save.GetPassiveLevel(passive) + 1);
        panel.pickButton.onClick.AddListener(() =>
        {
            save.passives.Add(passive);
            save.Save();
            panel.pickButton.onClick.RemoveAllListeners();
            SceneChanger.Instance.ChangeScene("Main");
        });
    }

    private void Update()
    {
        DevKeys();
    }

    private void DevKeys()
    {
        if (!Application.isEditor) return;
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneChanger.Instance.ChangeScene("Pick");
        }
        
        if (Input.GetKeyDown(KeyCode.A))
        {
            foreach (Transform child in container)
            {
                Destroy(child.gameObject);
            }

            var all = Passives.GetAll().ToList();
            all.ForEach(AddPassive);
            scrollContent.sizeDelta = new Vector2 (265 * all.Count, 300);
        }
    }
}