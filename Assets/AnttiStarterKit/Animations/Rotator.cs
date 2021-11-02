using UnityEngine;

namespace AnttiStarterKit.Animations
{
	public class Rotator : MonoBehaviour {

		public float speed = 1f;

		private float angle;

		private void Start()
		{
			angle = Random.value * 360f;
		}

		// Update is called once per frame
		void Update () {
			angle += speed * Time.deltaTime * 60f;
			transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, angle));
		}

		public void ChangeSpeed(float s) {
			speed = s;
		}
	}
}
