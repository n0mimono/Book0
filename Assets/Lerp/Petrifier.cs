using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Custom;

public class Petrifier : MonoBehaviour {
	public Material material;
	public List<Behaviour> exceptions;
	public float time;
	public bool petrifyOnStart;

	private List<Material> materials;
	private bool isPetrified;
	private List<Behaviour> behavs;

	void Start() {
		materials = new List<Material> ();

		foreach (Renderer ren in GetComponentsInChildren<Renderer> ()) {
			Material[] mats = ren.materials;
			for (int i = 0; i < mats.Length; i++) {
				Material mat = Instantiate (material);
				mat.hideFlags = HideFlags.DontSave;
				mat.SetTexture("_MainTex", mats[i].GetTexture("_MainTex"));
				mats [i] = mat;

				if (!materials.Where (m => m == mat).Any ()) {
					materials.Add (mat);
				}
			}
			ren.materials = mats;
		}

		foreach (SkinnedMeshRenderer ren in GetComponentsInChildren<SkinnedMeshRenderer> ()) {
			ren.gameObject.AddComponent<MeshExploder> ();
		}

		if (petrifyOnStart) {
			Petrify ();
		}
	}

	public void Petrify() {
		if (isPetrified) return;
		isPetrified = true;

		LerpMaterial (time).StartBy (this);
	}

	private IEnumerator LerpMaterial(float time) {
		float t = 0f;
		while (t < time) {
			t += Time.deltaTime / time;
			materials.ForEach (m => m.SetFloat ("_LerpRate", t));
			yield return null;
		}

		behavs = GetComponentsInChildren<Behaviour> ()
			.Where (b => b.enabled)
			.Where (b => !exceptions.Where (e => e == b).Any ())
			.ToList ();

		behavs.ForEach (b => b.enabled = false);
		yield return null;
	}

	public void Restore() {
		if (!isPetrified) return;
		isPetrified = false;

		LerpMaterialRestore (time).StartBy (this);
	}

	private IEnumerator LerpMaterialRestore(float time) {
		float t = 0f;
		while (t < time) {
			t += Time.deltaTime / time;
			materials.ForEach (m => m.SetFloat ("_LerpRate", 1 - t));
			yield return null;
		}

		behavs.ForEach (b => b.enabled = true);
		yield return null;
	}

	public void Explosion() {
		BroadcastMessage("Explode");
		gameObject.SetActive (false);
	}

}
