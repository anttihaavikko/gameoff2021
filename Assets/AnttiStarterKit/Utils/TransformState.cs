using System;
using UnityEngine;

namespace AnttiStarterKit.Utils
{
    public class TransformState : MonoBehaviour
    {
        public Vector3 Position { get; private set; }
        public Vector3 LocalPosition { get; private set; }
        public Vector3 Scale { get; private set; }

        private void Start()
        {
            SaveState();
        }

        public void SaveState()
        {
            var t = transform;
            Position = t.position;
            LocalPosition = t.localPosition;
            Scale = t.localScale;
        }
    }
}