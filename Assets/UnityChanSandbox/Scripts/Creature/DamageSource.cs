using UnityEngine;
using System.Collections;

public class DamageSource : MonoBehaviour {
	public event System.Action<DamageSource> OnHit;
	public System.Func<bool> IsDamageable;

	public int basePoint;
	public bool isBreaker;

	void Awake() {
		OnHit += (src) => {};
		IsDamageable = () => true;
	}

	void OnControllerColliderHit(ControllerColliderHit hit) {
		OnCollision (hit.collider.gameObject);
	}

	void OnCollisionEnter(Collision collision) {
		//Debug.Log (gameObject + " > " + collision);
		OnCollision (collision.gameObject);
	}

	void OnTriggerEnter(Collider collider) {
		//Debug.Log (gameObject + " > " + collider.gameObject.name);
		OnCollision (collider.gameObject);
	}

	public void OnCollision(GameObject other) {
		DamageReceptor receptor = other.GetComponent<DamageReceptor> ();

		OnHit (this);
		if (receptor != null
			&& gameObject.IsOppositeTo (other)
			&& IsDamageable()
			&& receptor.IsDamageable()
		) {
			receptor.InvokeDamage (this);
		}

	}

}
