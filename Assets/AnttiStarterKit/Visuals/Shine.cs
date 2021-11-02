using UnityEngine;

namespace AnttiStarterKit.Visuals
{
	public class Shine : MonoBehaviour {

		Vector3 originalPos;
		public float distance = 0.1f;
		public Transform mirrorParent;
		public bool checkRotation = false;
		public Vector3 focus = Vector3.up * 10f;

		// Use this for initialization
		void Start () {
			originalPos = transform.localPosition;
		}
	
		// Update is called once per frame
		void Update () {
			Vector3 direction = (focus - transform.position).normalized;
			direction.z = originalPos.z;
			direction.x = mirrorParent ? mirrorParent.localScale.x * direction.x : direction.x;

			if (checkRotation) {
				float angle = transform.parent.rotation.eulerAngles.z;
				float aMod = Mathf.Sign (transform.parent.lossyScale.x);
				direction = Quaternion.Euler(new Vector3(0, 0, -angle * aMod)) * direction;
			}

			transform.localPosition = Vector3.MoveTowards(transform.localPosition, originalPos + direction * distance, 0.1f);
		}
	}
}
