using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Custom;

public class ShakePoint : MonoBehaviour {
	public List<Transform> targetList;
	public Transform target;
	public Transform actualTarget;

	public Shaker shaker;

	void Start() {
		Next ();
	}

	void Update() {
		if (actualTarget != null) {
			transform.position = actualTarget.position + shaker.offset;
		}
	}

	public void Next() {
		target = targetList.NextOrDefault (t => t == target);
		actualTarget = target.GetTarget ();
	}

	[Button("Next", "Next")] public int ButtonNext;
}
