using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using Custom;

public class FieldWarpTrigger : MonoBehaviour {
	public DamageReceptor receptor;
	public Vector3 toWarp;
	public int loadIndex;

	private bool isReady;

	void Start() {
		isReady = true;

		receptor.OnDamage += OnDamage;
	}

	private void OnDamage(DamageSource src) {
		if (!isReady) return;
		isReady = false;

		src.transform.position = toWarp;

		// todo: we hope more smart solution.
		GameObject.FindObjectOfType<FieldManager>().LoadField(loadIndex);

		DelaydReady ().StartBy (this);

		// wtf: this method shall be operated by last.
		GetComponent<MeshExplosionTrigger>().Explosion();
	}

	private IEnumerator DelaydReady() {
		yield return new WaitForSeconds (3f);
		isReady = true;
	}

}
