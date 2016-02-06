using UnityEngine;
using System.Collections;

public class AngelGearTrigger : MagicProjectile {

	[Header("Gear")]
	public AngelAltar gear;

	public override void Initialize() {
		base.Initialize ();

		gear.OnProcCompleted = () => gameObject.SetActive (false);
	}

	public override void Load() {
		base.Load ();
	}

	public override void Fire() {
		gear.target = target;

		gear.StartProc ();
	}

	public override void Unload() {
		base.Unload ();
	}

}
