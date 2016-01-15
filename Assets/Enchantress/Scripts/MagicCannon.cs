using UnityEngine;
using System.Collections;
using System.Linq;
using Custom;
using DigitalRuby.PyroParticles;

public class MagicCannon : MagicSpawner {
	public ParticleSystem gatherBubbles;

	public override void Initilize(string tag) {
		base.Initilize (tag);

		instanceName = "Fireball2";
		OnHold = () => gatherBubbles.Play ();
		OnRelease = () => gatherBubbles.Stop ();
	}

}
