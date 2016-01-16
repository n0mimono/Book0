using UnityEngine;
using System.Collections;

public class MagicShield : MagicProjectile {
	public DamageReceptor receptor;

	void Awake() {
		receptor.OnDamage += (src) => {
			OnGuard(src.transform.position);
		};
	}

	public override void Hit() {
	}

	public override void Fire() {
	}

	private void OnGuard(Vector3 srcPos) {
		GameObject obj = PoolManager.Instance.GetInstance ("ShieldGuard");
		obj.transform.position = srcPos;
		obj.transform.up = (srcPos - transform.position).normalized;

		MagicCircle circle = obj.GetComponent<MagicCircle> ();
		circle.Release ();
	}

}
