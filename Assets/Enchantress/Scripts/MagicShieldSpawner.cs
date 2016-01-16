using UnityEngine;
using System.Collections;

public class MagicShieldSpawner : MagicSpawner {
	public override void Initilize(string tag) {
		base.Initilize (tag);

		instanceName = "MagicShield";
	}
}
