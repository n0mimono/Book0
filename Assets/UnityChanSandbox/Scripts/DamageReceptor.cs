using UnityEngine;
using System.Collections;

public class DamageReceptor : MonoBehaviour {
	public event System.Action<DamageSource> OnDamage;
	public System.Func<bool> IsDamageable;

	void Awake() {
		OnDamage += (src) => {};
	}

	public void InvokeDamage(DamageSource source) {
		OnDamage (source);
	}

}
