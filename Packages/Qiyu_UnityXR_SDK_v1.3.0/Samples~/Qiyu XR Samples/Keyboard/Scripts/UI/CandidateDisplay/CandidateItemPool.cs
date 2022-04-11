using UnityEngine;
using System.Collections.Generic;

namespace Qiyi.InputMethod.Keyboard
{
	public class CandidateItemPool
	{
		private GameObject pooledItem;

		List<GameObject> pool = new List<GameObject> ();

		public CandidateItemPool (GameObject pooledItem)
		{
			this.pooledItem = pooledItem;
		}

		public GameObject GetPooledObject (Transform parent)
		{
			foreach (var obj in pool) {
				if (!obj.activeInHierarchy) {
					obj.transform.SetParent (parent);
                    obj.transform.localPosition = Vector3.zero;
                    obj.transform.localScale = Vector3.one;
                    obj.transform.localRotation = Quaternion.identity;
                    obj.SetActive (true);
					return obj;
				}
			}

			GameObject newObj = Object.Instantiate (pooledItem, parent, false) as GameObject;
			pool.Add (newObj);
			newObj.SetActive (true);
			return newObj;
		}

		public void RecycleObject (GameObject obj)
		{
			obj.SetActive (false);
			obj.transform.SetParent (null);
		}

		public void Clear ()
		{
			if (pool.Count > 0) {
				for (int i = pool.Count - 1; i >= 0; i--) {
					Object.Destroy (pool [i]);
				}
			}
		}

	}
}

