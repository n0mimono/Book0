using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UIPoolManager : MonoBehaviour {

	public PooledPrefab[] objects;
	public Dictionary<Type, List<GameObject>> pooledObjects;

	private static UIPoolManager instance;
	public static UIPoolManager Instance {
		get {
			if (instance == null) {
				instance = GameObject.FindObjectOfType<UIPoolManager> ();
				instance.Initilize ();
			}
			return instance;
		}
	}

	public enum Type {
		Ripple = 0,
		All,
	}

	[System.Serializable]
	public class PooledPrefab {
		public Type       type;
		public GameObject go;
	}

	public void Initilize() {
		pooledObjects = new Dictionary<Type, List<GameObject>> ();
		for (int i = 0; i < (int)Type.All; i++) {
			pooledObjects[(Type)i] = new List<GameObject>();
		}
	}

	public GameObject GetObject(Type type) {
		GameObject obj = pooledObjects [type].Where (o => !o.activeInHierarchy).FirstOrDefault ();

		if (obj == null) {
			GameObject go = objects.Where (o => o.type == type).FirstOrDefault ().go;
			obj = Instantiate (go);
			obj.name = go.name + "_Cloned_" + pooledObjects [type].Count;
			obj.transform.SetParent (transform);
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localEulerAngles = Vector3.zero;
			obj.transform.localScale = Vector3.one;
			pooledObjects [type].Add (obj);
		}

		obj.SetActive (true);
		return obj;
	}

}
