using UnityEngine;
using System.Collections;
using Custom;

public class MagicMagazine : MagicSpawner {
	[Header("Magazine")]
	public MagicCircle circle;

	public override void Initilize(string tag) {
		base.Initilize (tag);

		if (circle != null) {
			OnHold = () => circle.Hold ();
			OnRelease = () => circle.Release ();
		}
	}

}
