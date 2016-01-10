using UnityEngine;
using System.Collections;

public class DamageSource : MonoBehaviour {
	public Common.Layer damageableLayer;

	public event System.Action<DamageSource> OnHit;
	public System.Func<bool> IsDamageable;

	void Awake() {
		OnHit += (src) => {};
		IsDamageable = () => true;
	}

	void OnControllerColliderHit(ControllerColliderHit hit) {
		OnCollision (hit.collider.gameObject);
	}

	void OnCollisionEnter(Collision collision) {
		OnCollision (collision.gameObject);
	}

	void OnTriggerEnter(Collider collider) {
		OnCollision (collider.gameObject);
	}

	public void OnCollision(GameObject other) {
		DamageReceptor receptor = other.GetComponent<DamageReceptor> ();

		OnHit (this);
		if (receptor != null
			&& gameObject.IsOppositeTo (other)
			&& other.IsLayer(damageableLayer)
			&& IsDamageable()
			&& receptor.IsDamageable()
		) {
			receptor.InvokeDamage (this);
		}

	}

}
