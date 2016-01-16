using UnityEngine;
using System.Collections;

public class MagicShieldSpawner : MagicSpawner {
	public ParticleSystem particle;

	private MagicShield shield;

	public override void Initilize(string tag) {
		base.Initilize (tag);

		instanceName = "MagicShield";

		OnHold = () => {
			//projectile.GetComponent<TransformFollower>().parent = transform;
			particle.Play ();
		};
		OnRelease = () => {
			//projectile.GetComponent<TransformFollower>().parent = null;
			particle.Stop ();
		};
	}

}
