using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Custom;

public partial class MagicFireball : MagicProjectile {
	public List<ParticleSystem> particleSystems;

	public override void Initialize() {
		base.Initialize ();
		instanceName = "FireballExplosion";
	}

	public override void Fire() {
		base.Fire ();

		particleSystems.ForEach (p => {
			p.startSpeed = speed;
			p.Play();
		});	

		AutoHit ().StartBy (this);
	}

	IEnumerator AutoHit() {
		yield return new WaitForSeconds (10f);
		Hit ();
	}

}
