using UnityEngine;
using System.Collections;

public class UICameraFinder : MonoBehaviour {
	public Canvas canvas;
	private Camera uiCamera = null;

	void Update() {
		if (uiCamera == null) {
			GameObject obj = GameObject.FindGameObjectWithTag ("UICamera");
			Camera camera = obj.GetComponent<Camera> ();
			if (camera != null) {
				uiCamera = camera;
				canvas.worldCamera = uiCamera;
			}
		}
	}

}
