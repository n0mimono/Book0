using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Custom;

public class FPSCounter : MonoBehaviour {
	public Text text;
	public float fps;

	void Update() {
		fps = Time.timeScale / Time.deltaTime;

		if (text != null) {
			text.text = fps.ToString ("F1");
		}
	}
}
