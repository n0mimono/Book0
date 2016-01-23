using UnityEngine;
using System.Collections;

public class DemoCamera : MonoBehaviour {
	public Camera myCamera;

	IEnumerator Start() {
		yield return null;

		if (Camera.main == null) {
			myCamera.enabled = true;
		} else {
			myCamera.enabled = false;
		}
	}
}
