using System.Collections.Generic;
using AnttiStarterKit.Animations;
using UnityEngine;

public class ConnectionLines : MonoBehaviour
{
    [SerializeField] private List<LineRenderer> lines;
    [SerializeField] private Transform preview;

    private Vector3 previousPos;

    public void ShowLine(int index, Vector3 from, Vector3 to)
    {
        lines[index].gameObject.SetActive(true);
        lines[index].SetPosition(0, from);
        lines[index].SetPosition(1, to);
    }

    public void Hide()
    {
        preview.gameObject.SetActive(false);
        lines.ForEach(l => l.gameObject.SetActive(false));
    }

    public void MovePreview(Vector2 to)
    {
        preview.gameObject.SetActive(true);

        if (Vector3.Distance(to, previousPos) < 0.5f) return;
        
        previousPos = to;
        Tweener.MoveToBounceOut(preview, to, 0.15f);
    }
}