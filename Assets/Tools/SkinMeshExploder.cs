using UnityEngine;
using System.Collections;

public class SkinMeshExploder : MonoBehaviour {
	public float delay;

	public bool useGravity;

	IEnumerator Start() {
		foreach (SkinnedMeshRenderer ren in GetComponentsInChildren<SkinnedMeshRenderer> ()) {
			MeshExploder ex = ren.gameObject.AddComponent<MeshExploder> ();
			ex.useGravity = useGravity;
			yield return null;
		}
	}

	public void Explosion() {
		StartCoroutine (ProcExplosion ());
	}

	private IEnumerator ProcExplosion() {
		yield return new WaitForSeconds (delay);
		BroadcastMessage("Explode");
		yield return null;
		gameObject.SetActive (false);
	}
}
