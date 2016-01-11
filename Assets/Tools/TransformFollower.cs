using UnityEngine;
using System.Collections;

public class TransformFollower : MonoBehaviour {
	public Transform parent;

	public bool followPosition;
	public bool followAngles;
	public bool followScales;
	public bool followActive;

	void Update() {
		if (followPosition) {
			transform.position = parent.position;
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
