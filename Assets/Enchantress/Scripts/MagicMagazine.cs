using UnityEngine;
using System.Collections;
using Custom;

public class MagicMagazine : MagicSpawner {
	public MagicCircle circle;

	public override void Initilize(string tag) {
		base.Initilize (tag);

		OnHold = () => circle.Hold ();
		OnRelease = () => circle.Release ();
	}
}
		
