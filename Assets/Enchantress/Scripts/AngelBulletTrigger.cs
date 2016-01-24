using UnityEngine;
using System.Collections;
using Custom;

public class AngelBulletTrigger : MagicProjectile {
	
	[Header("Angel")]
	public AngelBulletAltar altar;

	public override void Initialize() {
		base.Initialize ();

		altar.OnProcCompleted = () => gameObject.SetActive (false);
	}

	public override void Load() {
		base.Load ();
	}

	public override void Fire() {
		altar.target = target;

		altar.StartProc ();
	}

	public override void Unload() {
		base.Unload ();
	}

}
