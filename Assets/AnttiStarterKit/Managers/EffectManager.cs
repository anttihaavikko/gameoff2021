using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AnttiStarterKit.Managers
{
	public class EffectManager : MonoBehaviour {

		public AutoEnd[] effects;

		[SerializeField]
		private Queue<AutoEnd>[] effectPool;

		// ==================

		private static EffectManager instance = null;

		public static EffectManager Instance {
			get { return instance; }
		}

		// ==================

		void Awake() {
			if (instance != null && instance != this) {
				Destroy (this.gameObject);
				return;
			} else {
				instance = this;
			}

			effectPool = new Queue<AutoEnd>[effects.Length];

			for(var i = 0; i < effectPool.Length; i++)
			{
				effectPool[i] = new Queue<AutoEnd>();
			}
		}

		public GameObject AddEffect(int effect, Vector3 position, float angle = 0f) {
			var e = Get(effect);
			e.transform.parent = transform;
			e.transform.position = position;
			e.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
			return e.gameObject;
		}

		public GameObject AddEffectToParent(int effect, Vector3 position, Transform parent) {
			var e = Get(effect);
			e.transform.parent = parent;
			e.transform.position = position;
			return e.gameObject;
		}

		private AutoEnd Get(int index)
		{
			if (!effectPool[index].Any())
				AddObjects(index, 1);

			var obj = effectPool[index].Dequeue();
			obj.gameObject.SetActive(true);
			obj.Start();
			obj.GetParticleSystem().Play();

			return obj;
		}

		private void AddObjects(int index, int count)
		{
			for (var i = 0; i < count; i++)
			{
				var obj = Instantiate(effects[index]);
				obj.Pool = index;
				effectPool[index].Enqueue(obj);
			}
		}

		public void ReturnToPool(AutoEnd obj)
		{
			obj.gameObject.SetActive(false);
			effectPool[obj.Pool].Enqueue(obj);
		}

		public static void AddEffects(IEnumerable<int> ids, Vector3 position)
		{
			foreach (var id in ids)
			{
				Instance.AddEffect(id, position);
			}
		}

		public static GameObject AddEffect(int id, Vector3 position)
		{
			var eff = Instance.AddEffect(id, position);
			return eff.gameObject;
		}
	}
}