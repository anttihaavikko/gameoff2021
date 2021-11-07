using UnityEngine;

namespace AnttiStarterKit.Visuals
{
	public class Shine : MonoBehaviour {
		public float distance = 0.1f;
		public Transform mirrorParent;
		public bool checkRotation = false;
		public Vector3 focus = Vector3.up * 10f;

		private Vector3 originalPos;
		
		private void Start () {
			originalPos = transform.localPosition;
		}

		private void Update () {
			var t = transform;
			var direction = (focus - t.position).normalized;
			direction.z = originalPos.z;
			direction.x = mirrorParent ? mirrorParent.localScale.x * direction.x : direction.x;

			if (checkRotation) {
				var parent = t.parent;
				var angle = parent.rotation.eulerAngles.z;
				var aMod = Mathf.Sign (parent.lossyScale.x);
				direction = Quaternion.Euler(new Vector3(0, 0, -angle * aMod)) * direction;
			}

			transform.localPosition = Vector3.MoveTowards(transform.localPosition, originalPos + direction * distance, 0.1f);
		}
	}
}
