using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Custom;

public class FixedPointFollower : MonoBehaviour {
	public List<Transform> targetList;
	public Transform target;
	public Transform actualTarget;

	public float lerpSpeed;

	void Start() {
		Next ();
	}

	void Update() {
		if (actualTarget != null) {
			transform.LookAtLerp (actualTarget.position, lerpSpeed * Time.deltaTime);
		}
	}

	public void Next() {
		target = targetList.NextOrDefault (t => t == target);
		actualTarget = target.GetTarget ();
	}

	[Button("Next", "Next")] public int ButtonNext;
}
