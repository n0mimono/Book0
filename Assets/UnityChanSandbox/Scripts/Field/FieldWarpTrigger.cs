using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using Custom;

public class FieldWarpTrigger : MonoBehaviour {
	public DamageReceptor receptor;
	public Vector3 toWarp;
	public int loadIndex;

	public System.Action<int> OnLoad;

	private bool isReady;

	void Start() {
		isReady = true;
		GetComponent<Renderer> ().enabled = true;

		receptor.OnDamage += OnDamage;
	}

	private void OnDamage(DamageSource src) {
		if (!isReady) return;
		isReady = false;

		src.transform.position = toWarp;
		WarpField ().StartBy (this);
	}

	private IEnumerator WarpField() {
		GetComponent<MeshExploder>().Explode();
		yield return null;

		GetComponent<Renderer> ().enabled = false;
		yield return new WaitForSeconds (0.5f);

		yield return null;
		FieldManager.Instance.LoadField (loadIndex, true);
	}

}
