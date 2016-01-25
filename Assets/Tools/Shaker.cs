using UnityEngine;
using System.Collections;
using Custom;

public class Shaker : SingletonMonoBehaviorWithoutCreate<Shaker> {

	public float amplitude;
	public float damp;
	public float keepTime;

	public Vector3 offset;

	private IEnumerator shake;
	private float tmpAmplitude;

	public void StartShake() {
		shake.Do (s => StopCoroutine (s));

		shake = ProcShake ();
		StartCoroutine (shake);
	}

	private IEnumerator ProcShake() {
		tmpAmplitude = amplitude;

		float time = 0f;
		while (tmpAmplitude > 0f) {
			time += Time.deltaTime;

			offset = Random.insideUnitSphere * tmpAmplitude;
			if (time > keepTime) {
				tmpAmplitude -= damp * Time.deltaTime;
			}
			yield return null;
		}

		tmpAmplitude = 0f;
		offset = Vector3.zero;
	}
}
