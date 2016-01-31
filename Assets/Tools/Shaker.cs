using UnityEngine;
using System.Collections;
using Custom;
using System.Collections.Generic;
using System.Linq;

public class Shaker : SingletonMonoBehaviorWithoutCreate<Shaker> {

	public enum Type {
		None,
		Low,
		Middle,
		High,
	}

	[System.Serializable]
	public class ShakeParam {
		public Type type;

		public float amplitude;
		public float damp;
		public float keepTime;
	}
	public List<ShakeParam> shakeParamList;
	public float distDamp;

	public Vector3 offset;

	public List<Shake> shakes;

	public void StartShake(Type type, Vector3 position) {
		ShakeParam param = shakeParamList.Where (s => s.type == type).FirstOrDefault ();
		if (param == null) return;

		float amp = param.amplitude * AmplifyFromDistance (position);
		Shake shake = new Shake (amp, param.damp, param.keepTime);

		shake.StartShake (this);
		shakes.Add (shake);
	}

	void Awake() {
		shakes = new List<Shake> ();
	}

	void Update() {
		shakes.RemoveAll (s => s.isFinished);
		offset = shakes.Select (s => s.offset).Sum ();
	}

	public static void Shake(Type type, Vector3 position) {
		Instance.Do (s => s.StartShake (type, position));
	}

	private float AmplifyFromDistance(Vector3 position) {
		float dist = Vector3.Distance (position, Camera.main.transform.position);

		float amp = Mathf.Clamp(- dist/distDamp + 1f, 0f, 1f);
		return amp;
	}

}

public class Shake {
	private float amplitude;
	private float damp;
	private float keepTime;

	public Vector3 offset { get; private set; }
	public bool isFinished { get; private set; }

	private float tmpAmplitude;

	public Shake(float amplitude, float damp, float keepTime) {
		this.amplitude = amplitude;
		this.damp = damp;
		this.keepTime = keepTime;
	}

	public void StartShake(Shaker parent) {
		ProcShake ().StartBy (parent);
	}

	private IEnumerator ProcShake() {
		tmpAmplitude = amplitude;
		offset = Vector3.zero;
		isFinished = false;

		float time = 0f;
		while (tmpAmplitude > 0f) {
			time += Time.deltaTime;

			offset = UnityEngine.Random.insideUnitSphere * tmpAmplitude;
			if (time > keepTime) {
				tmpAmplitude -= damp * Time.deltaTime;
			}

			yield return null;
		}

		tmpAmplitude = 0f;
		offset = Vector3.zero;
		isFinished = true;
	}

}