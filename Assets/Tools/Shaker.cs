using UnityEngine;
using System.Collections;
using Custom;

public class Shaker : MonoBehaviour {

	public float amplitude;
	public float damp;
	public float keepTime;

	public Vector3 offset;

	[Button("StartShake", "Start Shake")] public float ButtonStartShake;

	public void StartShake() {
		ProcShake ().StartBy (this);
	}

	private IEnumerator ProcShake() {
		float tmpAmplitude = amplitude;

		float time = 0f;
		while (tmpAmplitude > 0f) {
			time += Time.deltaTime;

			offset = Random.insideUnitSphere * tmpAmplitude;
			if (time > keepTime) {
				tmpAmplitude -= damp * Time.deltaTime;
			}
			yield return null;
		}

		offset = Vector3.zero;
	}
}
