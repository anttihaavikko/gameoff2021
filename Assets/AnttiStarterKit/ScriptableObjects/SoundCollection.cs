using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Extensions;
using UnityEngine;

namespace AnttiStarterKit.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Sound Collection", menuName = "Sound collection", order = 0)]
    public class SoundCollection : ScriptableObject
    {
        [SerializeField] private List<AudioClip> clips;
        
        public int Count => clips.Count;

        public AudioClip Random()
        {
            return !clips.Any() ? null : clips.Random();
        }

        public AudioClip At(int i)
        {
            return clips[i];
        }
    }
}