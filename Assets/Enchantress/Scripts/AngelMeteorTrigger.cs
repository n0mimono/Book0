using UnityEngine;
using System.Collections;

public class AngelMeteorTrigger : MagicProjectile {

	[Header("Angel")]
	public AngelAltar meteor;

	public override void Initialize() {
		base.Initialize ();

		meteor.OnProcCompleted = () => gameObject.SetActive (false);
	}

	public override void Load() {
		base.Load ();
	}

	public override void Fire() {
		transform.position = Vector3.zero;
		transform.eulerAngles = Vector3.zero;

		meteor.StartProc ();
	}

	public override void Unload() {
		base.Unload ();
	}

}
