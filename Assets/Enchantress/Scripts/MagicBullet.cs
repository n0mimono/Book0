﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Custom;

public partial class MagicBullet : MagicProjectile {
	public TrailRenderer trail;

	[Header("Bullet")]
	public float angleSpeed;
	public Vector3 loadOffset;
	public Vector3 loadNoise;
	public Vector3 targetNoise;
	public bool isTrailOnLoad;

	[Header("Circle")]
	public Vector3 circleRotateSpeed;
	public Transform circle;

	private Vector3 generatedTargetNoise = Vector3.zero;

	public override void Initialize() {
		base.Initialize ();

		trail.material = Material.Instantiate (trail.material);
		trail.enabled = false;

		OnExplosion = (obj) => {
			ParticleSystem particle = obj.GetComponent<ParticleSystem>();
			particle.Play();
		};
	}

	public override void Load() {
		base.Load ();

		trail.Clear ();
		trail.enabled = isTrailOnLoad;
	}

	public override void Fire() {
		base.Fire ();

		generatedTargetNoise = targetNoise.GenerateRandom ();

		transform.forward = (-TargetDirection() + loadNoise.GenerateRandom() + loadOffset).normalized;
		trail.enabled = true;

		ForwardTarget ().While (cur.Is(Mode.Fire)).StartBy (this);
	}

	public override void Unload() {
		base.Unload ();

		trail.enabled = false;
	}

	IEnumerator ForwardTarget() {
		while (true) {
			transform.forward = Vector3.Lerp(transform.forward, TargetDirection(), angleSpeed * Time.deltaTime).normalized;
			yield return null;
		}
	}

	public override Vector3 TargetDirection() {
		return base.TargetDirection() + generatedTargetNoise;
	}

}
