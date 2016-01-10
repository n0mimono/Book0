using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PoolManager : MonoBehaviour {

	private static PoolManager instance;
	public static PoolManager Instance  {
		get {
			if (instance == null) {
				instance = GameObject.FindObjectOfType<PoolManager> ();
				instance.pools = new Dictionary<string, List<GameObject>> ();
			}
			return instance;
		}
	}

	public List<GameObject> prefabs;
	private Dictionary<string, List<GameObject>> pools;

	public GameObject GetInstance(string name) {

		if (!pools.ContainsKey (name)) {
			pools [name] = new List<GameObject> ();
		}
		List<GameObject> pool = pools [name];

		GameObject go = pool.Where (g => !g.activeInHierarchy).FirstOrDefault ();
		if (go == null) {
			GameObject prefab = prefabs.Where (p => p.name == name).FirstOrDefault ();
			go = Instantiate (prefab);
			go.transform.SetParent (transform);
			pool.Add (go);
		}
		go.SetActive (true);

		return go;
	}

}
