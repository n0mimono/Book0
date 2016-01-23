using UnityEngine;
using System.Collections;

public class TemporaryPool : MonoBehaviour {
	private static TemporaryPool instance;
	public static TemporaryPool Instance {
		get {
			if (instance == null) {
				instance = GameObject.FindObjectOfType<TemporaryPool> ();
				if (instance == null) {
					GameObject obj = new GameObject (typeof(TemporaryPool).ToString ());
					instance = obj.AddComponent<TemporaryPool> ();
				}
			}
			return instance;
		}
	}

	public void Add(Transform trans) {
		trans.SetParent (transform);
	}

}
