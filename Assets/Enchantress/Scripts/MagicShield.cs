using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Custom;

public class MagicShield : MagicProjectile {
	public DamageReceptor receptor;
	public TransformFollower follower;

	public float fadeTimeScale;
	private List<Material> materials = null;

	void Awake() {
		receptor.OnDamage += (src) => {
			OnGuard(src.transform.position);
		};
	}

	public override void Initialize() {
		base.Initialize ();
		collide.enabled = false;

		if (materials == null) {
			materials = new List<Material> ();
			GetComponentsInChildren<Renderer> ().ToList ().ForEach (r => {
				Material mat = Instantiate(r.material);
				r.material = mat;
				materials.Add(mat);
			});
		}
		SetAlpha (0f);
	}

	public override void Load() {
		base.Load ();
		collide.enabled = true;

		Fade (t => t, () => {});
	}

	public override void Hit(Vector3 pos) {
	}

	public override void Fire() {
		collide.enabled = false;

		Fade (t => 1 - t, () => gameObject.SetActive (false));
	}

	private void OnGuard(Vector3 srcPos) {
		GameObject obj = PoolManager.Instance.GetInstance ("ShieldGuard");
		obj.transform.position = srcPos;
		obj.transform.up = (srcPos - transform.position).normalized;

		MagicCircle circle = obj.GetComponent<MagicCircle> ();
		circle.Release ();
	}

	private void Fade(System.Func<float,float> timeToValue, System.Action onCompleted) {
		float time = 0f;
		new Noop ()
			.OnUpdate (() => time += Time.deltaTime * fadeTimeScale)
			.OnUpdate (() => SetAlpha (timeToValue (time)))
			.While (() => 0f <= time && time <= 1f)
			.OnCompleted (() => SetAlpha (timeToValue (time)))
			.OnCompleted (() => onCompleted ())
			.StartBy (this);
	}

	private void SetAlpha(float alpha) {
		float val = Mathf.Clamp (alpha, 0f, 1f);

		foreach (Material mat in materials) {
			mat.SetColor("_Color", new Color(1f, 1f, 1f, val));
		}

	}
}
