using UnityEngine;
using System.Collections;
using System.Linq;
using Custom;

public class MagicCannon : MagicSpawner {
	public ParticleSystem gatherBubbles;
	public MagicCircle circle;

	public override void Initilize(string tag) {
		base.Initilize (tag);

		OnHold = () => {
			gatherBubbles.Play ();
			circle.Hold();
		};
		OnRelease = () => {
			gatherBubbles.Stop ();
			circle.Release ();
		};

	}

}
