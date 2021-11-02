using System;
using TMPro;
using UnityEngine;

namespace AnttiStarterKit.Animations
{
    public class Appearer : MonoBehaviour
    {
        public float appearAfter = -1f;
        public float hideDelay;
        public bool silent;
        public GameObject visuals;
        public bool inScreenSpace;
        public Camera cam;
        
        public TMP_Text text;
        private Vector3 size;

        private void Start()
        {
            var t = transform;
            size = t.localScale;
            t.localScale = Vector3.zero;
            if(visuals) visuals.SetActive(false);

            if (appearAfter >= 0)
                Invoke(nameof(Show), appearAfter);
        }

        public void ShowAfter(float delay)
        {
            Invoke(nameof(Show), delay);
        }

        public void Show()
        {
            CancelInvoke(nameof(Hide));
            CancelInvoke(nameof(MakeInactive));
            DoSound();

            if(visuals) visuals.SetActive(true);
            Tweener.Instance.ScaleTo(transform, size, 0.3f, 0f, TweenEasings.BounceEaseOut);
        }

        public void Hide()
        {
            CancelInvoke(nameof(Show));
            DoSound();

            Tweener.Instance.ScaleTo(transform, Vector3.zero, 0.2f, 0f, TweenEasings.QuadraticEaseOut);
        
            if(visuals)
                Invoke(nameof(MakeInactive), 0.2f);
        }

        private void MakeInactive()
        {
            visuals.SetActive(false);
        }

        private void DoSound()
        {
            if (silent) return;

            var p = transform.position;
            var pos = inScreenSpace && cam ? cam.ScreenToWorldPoint(p) : p;

            // TODO SOUND
        }

        public void HideWithDelay()
        {
            Invoke(nameof(Hide), hideDelay);
        }
        
        public void HideWithDelay(float delay)
        {
            Invoke(nameof(Hide), delay);
        }

        public void ShowWithText(string t, float delay)
        {
            if (text)
                text.text = t;

            Invoke(nameof(Show), delay);
        }
    }
}
