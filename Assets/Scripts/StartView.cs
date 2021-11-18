using System;
using AnttiStarterKit.Utils;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class StartView : MonoBehaviour
{
    [SerializeField] private GameObject restartButton;
    [SerializeField] private TMP_Text startText;

    private void Start()
    {
        if (!Saver.Exists()) return;
        restartButton.SetActive(true);
        startText.text = "Continue";
    }

    public void Continue()
    {
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