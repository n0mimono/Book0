using UnityEngine;
using System.Collections;
using Custom;

public class RimEffect : MonoBehaviour {
	public Material orgMaterial;

	public bool isActiveOnStart;

	void Start() {
		if (isActiveOnStart) {
			Initialize ();
		}
	}

	public void Initialize() {
		Renderer[] renderers = GetComponentsInChildren<Renderer> ();

		foreach (Renderer renderer in renderers) {
			Material[] materials = renderer.materials;
			for (int i = 0; i < materials.Length; i++) {
				Material mat = Material.Instantiate (orgMaterial);
				mat.CopyTextureFromMaterial (materials [i], "_MainTex");
				materials [i] = mat;
			}
			renderer.materials = materials;
		}

	}

}
