using UnityEngine;
using System.Collections;
using Custom;

public class TransformFollower : MonoBehaviour {
	public Transform parent;

	public bool followPosition;
	public bool followAngles;
	public bool followScales;
	public bool followActive;

	[Header("Options")]
	public bool positionGroundOnly;

	void Update() {
		if (parent == null) {
			return;
		}

		if (followPosition) {
			if (positionGroundOnly) {
				transform.position = parent.position.Ground(transform.position.y);
			} else {
				transform.position = parent.position;
			}
		}
		if (followAngles) {
			transform.eulerAngles = parent.eulerAngles;
		}
		if (followScales) {
			transform.localScale = parent.localScale;
		}
		if (followActive) {
			gameObject.SetActive (parent.gameObject.activeInHierarchy);
		}

	}

}
