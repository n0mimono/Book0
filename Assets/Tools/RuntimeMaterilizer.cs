using UnityEngine;
using System.Collections;

public class RuntimeMaterilizer : MonoBehaviour {
	void Awake () {
		Renderer ren = GetComponent<Renderer> ();

		if (ren != null) {
			ren.sharedMaterial = Instantiate (ren.sharedMaterial);
			ren.sharedMaterial.hideFlags = HideFlags.DontSave;
		}
	}
}
