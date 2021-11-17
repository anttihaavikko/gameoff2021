﻿using UnityEngine;

namespace AnttiStarterKit.Animations
{
	public class Rotator : MonoBehaviour {

		[SerializeField] private bool randomizeSpeed;

		public float speed = 1f;
		public float pulsingSpeed, pulsingMin;

		private float angle;

		private void Start()
		{
			if (randomizeSpeed)
			{
				speed = Random.Range(-2f, 2f);
			}
			
			angle = Random.value * 360f;
		}
		
		private void Update ()
		{
			var mod = pulsingSpeed > 0 ? Mathf.Abs(Mathf.Sin(Time.time * pulsingSpeed)) + pulsingMin : 1f;
			angle -= speed * Time.deltaTime * 60f * mod;
			transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, angle));
		}

		public void ChangeSpeed(float s) {
			speed = s;
		}
	}
}
