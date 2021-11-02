using UnityEngine;
using UnityEngine.Rendering;

namespace AnttiStarterKit.Visuals
{
    public class AutoSpriteOrder : MonoBehaviour
    {
        [SerializeField] private int offset;

        private SortingGroup _group;

        private void Start()
        {
            _group = GetComponent<SortingGroup>();
        }

        private void Update()
        {
            if (_group)
            {
                _group.sortingOrder = -Mathf.RoundToInt(transform.position.y * 10) + offset;
            }
        }
    }
}
