using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace AnttiStarterKit.Animations
{
    public class ButtonStyle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private bool doScale;
        [SerializeField] private float scaleAmount;
        [SerializeField] private bool doRotation;
        [SerializeField] private float rotationAmount;
    
        [SerializeField] private List<Image> frontImages, bgImages;
        [SerializeField] private List<TMP_Text> texts;

        [SerializeField] private bool doColors;
    
        [SerializeField] private Color backColor, frontColor;

        private Vector3 originalScale;
        private Color originalBackColor, originalFrontColor;

        private Camera cam;

        private void Start()
        {
            originalScale = transform.localScale;
            cam = Camera.main;
        }

        private Vector3 GetSoundPos()
        {
            return cam.ScreenToWorldPoint(transform.position);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            ApplyScaling(scaleAmount, TweenEasings.BounceEaseOut);
            ApplyRotation(Random.Range(-rotationAmount, rotationAmount), TweenEasings.BounceEaseOut);
            ApplyColors(backColor, frontColor);
        
            var pos = GetSoundPos();
            // AudioManager.Instance.PlayEffectAt(53, pos, 0.22f);
            // AudioManager.Instance.PlayEffectAt(49, pos, 0.718f);
        }
    
        private void ApplyScaling(float amount, Func<float, float> easing)
        {
            if (doScale)
            {
                Tweener.Instance.ScaleTo(transform, originalScale * (1f + amount), 0.2f, 0f, easing);
            }
        }

        private void ApplyRotation(float amount, Func<float, float> easing)
        {
            if (doRotation)
            {
                Tweener.Instance.RotateTo(transform, Quaternion.Euler(0, 0, amount), 0.2f, 0f, easing);
            }
        }

        private void ApplyColors(Color back, Color front)
        {
            if (!doColors) return;
            
            bgImages.ForEach(i =>
            {
                originalBackColor = i.color;
                i.color = back;
            });

            frontImages.ForEach(i =>
            {
                originalFrontColor = i.color;
                i.color = front;
            });

            if (!texts.Any()) return;
            originalFrontColor = texts.First().color;
            texts.ForEach(t => t.color = front);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ApplyScaling(0, TweenEasings.BounceEaseOut);
            ApplyRotation(0, TweenEasings.BounceEaseOut);
            ApplyColors(originalBackColor, originalFrontColor);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var pos = GetSoundPos();
            // AudioManager.Instance.PlayEffectAt(36, pos, 0.1f);
            // AudioManager.Instance.PlayEffectAt(25, pos, 4f);
            // AudioManager.Instance.PlayEffectAt(35, pos, 0.629f);
        }
    }
}
