using UnityEngine;
using System.Collections;

public class MagicShieldSpawner : MagicSpawner {
	public ParticleSystem particle;

	private MagicShield shield;

	public override void Initilize(string tag) {
		base.Initilize (tag);

		instanceName = "MagicShield";

		OnHold = () => {
			particle.Play ();
		};
		OnRelease = () => {
			particle.Stop ();
		};
	}

}
