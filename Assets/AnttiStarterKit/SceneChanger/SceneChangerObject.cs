using AnttiStarterKit.Extensions;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class SceneChangerObject : MonoBehaviour
{
    public void ChangeScene(string scene)
    {
        SceneChanger.Instance.ChangeScene(scene);
    }

    public void ChangeSceneOrName(string scene)
    {
        PlayerPrefs.SetString("NextScene", scene);
        var sceneName = PlayerPrefs.HasKey("PlayerName") ? scene : "Name";
        SceneChanger.Instance.ChangeScene(sceneName);
    }

    public void Quit()
    {
        SceneChanger.Instance.blinders.Close();
        this.StartCoroutine(() => Application.Quit(), 1f);
    }
}
