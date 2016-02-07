using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using Custom;

public class FieldWarpTrigger : Creature {
	public Vector3 toWarp;
	public int loadIndex;

	public System.Action<int> OnLoad;
	private bool isReady;

	protected override void Initialize() {
		base.Initialize ();

		isReady = true;
		GetComponent<Renderer> ().enabled = true;

		damageReceptor.OnDamage += DamageBreak;
	}

	private void DamageBreak(DamageSource src) {
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
