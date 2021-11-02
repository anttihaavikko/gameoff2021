using System;
using AnttiStarterKit.Extensions;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    public Action dropped;
    
    [SerializeField] private LayerMask dropMask, blockMask;
    [SerializeField] private bool lockAfterDrop = true;

    private Camera cam;
    private bool dragging;
    private Vector3 offset;
    private Vector3 start;
    private int layerId;

    private void Start()
    {
        cam = Camera.main;
    }

    private void OnMouseDown()
    {
        var go = gameObject;
        dragging = true;
        start = transform.position;
        offset = start - GetMousePos();
        layerId = go.layer;
        go.layer = 0;
    }

    private void Update()
    {
        if (dragging)
        {
            transform.position = GetMousePos() + offset;
        }

        if (dragging && Input.GetMouseButtonUp(0))
        {
            StopDrag();
        }
    }

    private void StopDrag()
    {
        var p = transform.position;
        var rounded = new Vector2(Mathf.Round(p.x), Mathf.Round(p.y));
        DropOn(rounded);
    }

    private void DropOn(Vector2 pos)
    {
        dragging = false;

        var allowed = Physics2D.OverlapCircle(pos, 0.1f, dropMask);
        var blocked = Physics2D.OverlapCircle(pos, 0.1f, blockMask);

        gameObject.layer = layerId;

        if (allowed && !blocked)
        {
            transform.position = pos;
            dropped?.Invoke();
            enabled = !lockAfterDrop;
            
            return;
        }

        transform.position = start;
    }

    private Vector3 GetMousePos()
    {
        return cam.ScreenToWorldPoint(Input.mousePosition).WhereZ(0);
    }
}