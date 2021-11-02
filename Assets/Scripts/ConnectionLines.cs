using System.Collections.Generic;
using UnityEngine;

public class ConnectionLines : MonoBehaviour
{
    [SerializeField] private List<LineRenderer> lines;

    public void ShowLine(int index, Vector3 from, Vector3 to)
    {
        lines[index].gameObject.SetActive(true);
        lines[index].SetPosition(0, from);
        lines[index].SetPosition(1, to);
    }

    public void Hide()
    {
        lines.ForEach(l => l.gameObject.SetActive(false));
    }
}