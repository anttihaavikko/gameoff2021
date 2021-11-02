using UnityEngine;

namespace AnttiStarterKit.Randomizers
{
    public class RandomRotation : MonoBehaviour
    {
        private void Start()
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360f)));
        }
    }
}
