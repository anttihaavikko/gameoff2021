using UnityEngine;
using UnityEngine.Rendering;

namespace AnttiStarterKit.Visuals
{
    public class AutoSpriteOrderOnStart : MonoBehaviour
    {
        [SerializeField] private int offset;

        private void Start()
        {
            var group = GetComponent<SortingGroup>();
            
            if (group)
            {
                group.sortingOrder = -Mathf.RoundToInt(transform.position.y * 10) + offset;
            }
        }
    }
}