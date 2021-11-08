using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Extensions;
using UnityEngine;

public class GroundDecorations : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> sprites;
    [SerializeField] private List<Sprite> options;
    [SerializeField] private List<int> nonAnimated;
    [SerializeField] private Material staticMaterial;

    private void Start()
    {
        var ratio = 0.8f;
        
        sprites.ForEach(s =>
        {
            s.sprite = options.Random();
            ApplyShader(s);
            var t = s.transform;
            s.gameObject.SetActive(Random.value < ratio);
            s.transform.Mirror(Random.value < 0.5f);
            s.transform.localScale *= Random.Range(0.5f, 1f);
            t.position = t.position.RandomOffset(0.2f);
            s.color = s.color.RandomTint(0.01f);
        });
    }

    private void ApplyShader(SpriteRenderer s)
    {
        if (nonAnimated.Contains(options.IndexOf(s.sprite)))
        {
            s.material = staticMaterial;
        }
    }

    public void ClearDecorationsNearPike(Vector3 position)
    {
        sprites.Where(s => IsNear(position, s.transform.position)).ToList()
            .ForEach(s => s.gameObject.SetActive(false));
    }

    private static bool IsNear(Vector3 position, Vector3 point)
    {
        var yDiff = Mathf.Abs(position.y - point.y);
        var tooClose = position.x < 0 && point.x < position.x || position.x > 0 && point.x > position.x;
        return yDiff < 1f && tooClose;
    }
}
