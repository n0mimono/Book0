using UnityEngine;
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

	[Header("Circle")]
	public Vector3 circleRotateSpeed;
	public Transform circle;

	public override void Initialize() {
		base.Initialize ();
		instanceName = "MagicBulletExplosion";

		trail.material = Material.Instantiate (trail.material);
		trail.enabled = false;

		OnExplotion = (obj) => {
			ParticleSystem particle = obj.GetComponent<ParticleSystem>();
			particle.Play();
		};
	}

	public override void Load() {
		base.Load ();

		trail.enabled = false;
	}

	public override void Fire() {
		base.Fire ();

		transform.forward = (-TargetDirection + loadNoise.GenerateRandom() + loadOffset).normalized;
		trail.enabled = true;

		ForwardTarget ().While (cur.Is(Mode.Fire)).StartBy (this);
	}

	public override void Unload() {
		base.Unload ();

		trail.enabled = false;
	}

	IEnumerator ForwardTarget() {
		while (true) {
			transform.forward = Vector3.Lerp(transform.forward, TargetDirection, angleSpeed * Time.deltaTime).normalized;
			yield return null;
		}
	}

}
