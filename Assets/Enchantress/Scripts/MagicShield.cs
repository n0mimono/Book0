﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Custom;

public class MagicShield : MagicProjectile {

	[Header("Shield")]
	public DamageReceptor receptor;
	public TransformFollower follower;

	public float fadeTimeScale;
	private List<Material> materials = null;

	public MeshExploder exploder;

	private float curAlpha;
	private float tgtAlpha;

	void Awake() {
		receptor.OnDamage += (src) => {
			if (src.isBreaker) {
				OnBreak();
			}

			OnGuard(src.transform.position);
		};

		if (materials == null) {
			materials = new List<Material> ();
			GetComponentsInChildren<Renderer> ().ToList ().ForEach (r => {
				Material mat = Instantiate(r.material);
				r.material = mat;
				materials.Add(mat);
			});
		}
	}

	void Update() {

		float da = tgtAlpha - curAlpha;
		if (Mathf.Abs (da) > 0.01f) {
			float sign = Mathf.Sign(da);
			curAlpha += sign * Time.deltaTime;
			SetAlpha (curAlpha);
		}
	}

	public override void Initialize() {
		base.Initialize ();
		collide.enabled = false;

		tgtAlpha = 0f;
		curAlpha = 0f;
		SetAlpha (0f);
	}

	public override void Load() {
		base.Load ();
		collide.enabled = true;

		tgtAlpha = 1f;
	}

	public override void Hit(Vector3 pos) {
	}

	public override void Fire() {
		collide.enabled = false;

		tgtAlpha = 0f;
		AfterFire ().StartBy (this);
	}

	private IEnumerator AfterFire() {
		yield return new WaitForSeconds (1.5f);
		gameObject.SetActive (false);
	}

	private void OnGuard(Vector3 srcPos) {
		GameObject obj = PoolManager.Instance.GetInstance (instanceName);
		obj.transform.position = srcPos;
		obj.transform.up = (srcPos - transform.position).normalized;

		MagicCircle circle = obj.GetComponent<MagicCircle> ();
		circle.Release ();
	}

	private void SetAlpha(float alpha) {
		float val = Mathf.Clamp (alpha, 0f, 1f);

		foreach (Material mat in materials) {
			mat.SetColor("_Color", new Color(1f, 1f, 1f, val));
		}

	}

	private void OnBreak() {
		ProcBreak ().StartBy (this);
	}

	private IEnumerator ProcBreak() {
		exploder.Explode();
		yield return null;
		Fire ();
	}

	[Button("OnBreak", "OnBreak")] public int ButtonBreak;
}
