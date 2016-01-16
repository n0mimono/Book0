using UnityEngine;
using System.Collections;

public class DamageReceptor : MonoBehaviour {
	public event System.Action<DamageSource> OnDamage;
	public System.Func<bool> IsDamageable;

	void Awake() {
		OnDamage += (src) => {};
		IsDamageable = () => true;
	}

	public void InvokeDamage(DamageSource source) {
		OnDamage (source);
	}

}
