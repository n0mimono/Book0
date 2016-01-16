using UnityEngine;
using System.Collections;
using Custom;

public class Shaker : MonoBehaviour {

	public float   amplitude;
	public float   damp;

	public Vector3 offset;

	[Button("StartShake", "Start Shake")] public float ButtonStartShake;

	void Update() {
		transform.position = offset;
	}

	public void StartShake() {
		ProcShake ().StartBy (this);
	}

	private IEnumerator ProcShake() {
		float tmpAmplitude = amplitude;

		while (tmpAmplitude > 0f) {
			offset = Random.insideUnitSphere * tmpAmplitude;
			tmpAmplitude -= damp * Time.deltaTime;
			yield return null;
		}

		offset = Vector3.zero;
	}
}
