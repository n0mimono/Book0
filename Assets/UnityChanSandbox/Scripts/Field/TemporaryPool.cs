using UnityEngine;
using System.Collections;
using Custom;

public class TemporaryPool : SingletonMonoBehaviorDestroyable<TemporaryPool> {
	public void Add(Transform trans) {
		trans.SetParent (transform);
	}

}
