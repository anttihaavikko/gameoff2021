using System;
using AnttiStarterKit.Extensions;
using UnityEngine;
using UnityEngine.Rendering;

public class Draggable : MonoBehaviour
{
    public Action dropped, hidePreview, click;
    public Action<Vector2> preview;
    
    [SerializeField] private LayerMask dropMask, blockMask;
    [SerializeField] private bool lockAfterDrop = true;
    [SerializeField] private int normalSortOrder, dragSortOrder;

    public bool CanDrag { get; set; } = true;
    public bool DropLocked { get; set; }

    private Camera cam;
    private bool dragging;
    private Vector3 offset;
    private Vector3 start;
    private int layerId;
    private SortingGroup sortingGroup;

    public bool IsDragging => dragging;

    private void Start()
    {
        sortingGroup = GetComponent<SortingGroup>();
        cam = Camera.main;
    }

    private void OnMouseDown()
    {
        if (!CanDrag) return;
        
        var go = gameObject;
        dragging = true;
        start = transform.position;
        offset = start - GetMousePos();
        layerId = go.layer;
        go.layer = 0;

        SetSortOrder(dragSortOrder);
    }

    private void SetSortOrder(int order)
    {
        if (sortingGroup)
        {
            sortingGroup.sortingOrder = order;
        }
    }

    private void OnMouseUp()
    {
        click?.Invoke();
    }

    private void Update()
    {
        if (dragging)
        {
            transform.position = GetMousePos() + offset;
            InvokePreview();
        }

        if (dragging && Input.GetMouseButtonUp(0))
        {
            StopDrag();
        }
    }

    private void InvokePreview()
    {
        var rounded = GetRoundedPos();
        if (CanDrop(rounded))
        {
            preview?.Invoke(rounded);
            return;
        }
        
        hidePreview?.Invoke();
    }

    private void StopDrag()
    {
        var rounded = GetRoundedPos();
        DropOn(rounded);
        SetSortOrder(normalSortOrder - Mathf.RoundToInt(transform.position.y));
    }

    private Vector2 GetRoundedPos()
    {
        var p = transform.position;
        return new Vector2(Mathf.Round(p.x), Mathf.Round(p.y));
    }

    private void DropOn(Vector2 pos)
    {
        dragging = false;

        if (CanDrop(pos))
        {
            transform.position = pos;
            dropped?.Invoke();
            enabled = !lockAfterDrop;
            gameObject.layer = layerId;
            
            return;
        }

        transform.position = start;
        gameObject.layer = layerId;
    }

    private bool CanDrop(Vector2 pos)
    {
        if (DropLocked) return false;
        
        var allowed = Physics2D.OverlapCircle(pos, 0.1f, dropMask);
        var blocked = Physics2D.OverlapCircle(pos, 0.1f, blockMask);

        return allowed && !blocked;
    }

    private Vector3 GetMousePos()
    {
        return cam.ScreenToWorldPoint(Input.mousePosition).WhereZ(0);
    }
}