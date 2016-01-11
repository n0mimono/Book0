using UnityEngine;
using System.Collections;

public class FixedPointFollower : MonoBehaviour {
	public Transform target;

	void Update() {
		if (target != null) {
			transform.LookAt (target);
		}
	}

}
