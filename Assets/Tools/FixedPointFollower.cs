using UnityEngine;
using System.Collections;
using Custom;

public class FixedPointFollower : MonoBehaviour {
	public Transform target;
	public Transform actualTarget;

	public float lerpSpeed;

	void Start() {
		actualTarget = target.GetTarget ();
	}

	void Update() {
		if (actualTarget != null) {
			transform.LookAtLerp (actualTarget.position, lerpSpeed * Time.deltaTime);
		}
	}

}
