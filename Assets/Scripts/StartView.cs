using AnttiStarterKit.Utils;
using UnityEngine;

public class StartView : MonoBehaviour
{
    public void OnStart()
    {
        var scene = PlayerPrefs.HasKey("PlayerName") ? "Main" : "Name";
        SceneChanger.Instance.ChangeScene(scene);
    }
}