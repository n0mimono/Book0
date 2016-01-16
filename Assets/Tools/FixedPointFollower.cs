using UnityEngine;
using System.Collections;

public class FixedPointFollower : MonoBehaviour {
	public Transform target;
	public Transform actualTarget;

	void Start() {
		actualTarget = target.GetTarget ();
	}

	void Update() {
		if (actualTarget != null) {
			transform.LookAt (actualTarget);
		}
	}

}
