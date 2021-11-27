using AnttiStarterKit.Managers;
using AnttiStarterKit.Utils;
using Save;
using TMPro;
using UnityEngine;

public class StartView : MonoBehaviour
{
    [SerializeField] private GameObject restartButton;
    [SerializeField] private TMP_Text startText;

    private void Start()
    {
        AudioManager.Instance.ChangeMusic(0);
        
        if (!Saver.Exists() || HasDailySave()) return;
        restartButton.SetActive(true);
        startText.text = "Continue";
    }

    private static bool HasDailySave()
    {
        return Saver.Exists() && Saver.Load<SaveData>().IsDaily;
    }

    public void Continue()
    {
        if (HasDailySave())
        {
            Saver.Clear();
        }
        
        PlayerPrefs.SetString("NextScene", "Main");
        var scene = PlayerPrefs.HasKey("PlayerName") ? "Main" : "Name";
        SceneChanger.Instance.ChangeScene(scene);
    }

    public void Restart()
    {
        Saver.Clear();
        Continue();
    }

    public void Quit()
    {
        Application.Quit();
    }
}