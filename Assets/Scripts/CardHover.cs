using System;
using System.Collections;
using System.Collections.Generic;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Managers;
using UnityEngine;

public class CardHover : MonoBehaviour
{
    public Action onHover;
    
    private Vector3 originalScale;
    private float scaleAmount = 0.1f;
    
    private void Start()
    {
        originalScale = transform.localScale;
    }
    
    private void OnMouseEnter()
    {
        ApplyScaling(scaleAmount, TweenEasings.BounceEaseOut);
        onHover?.Invoke();
    }
    
    private void ApplyScaling(float amount, Func<float, float> easing)
    {
        if (!enabled) return;
        Tweener.Instance.ScaleTo(transform, originalScale * (1f + amount), 0.2f, 0f, easing);
        AudioManager.Instance.PlayEffectAt(2, transform.position, 1f);
    }

    private void OnMouseExit()
    {
        ApplyScaling(0, TweenEasings.BounceEaseOut);
    }

    public void Disable()
    {
        ApplyScaling(0, TweenEasings.BounceEaseOut);
        enabled = false;
    }
}
