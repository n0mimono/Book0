using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Custom;

public class MagicBullet : MonoBehaviour {
	public TrailRenderer trail;

	public enum Mode {
		None,
		Load,
		Fire,
		Hit
	}
	[System.Serializable]
	public class Status {
		public Mode mode;

		public System.Func<bool> Is(Mode mode) {
			return () => this.mode == mode;
		}
	}
	public Status cur = new Status();

	[Header("Bullet")]
	public float speed;
	public float angleSpeed;
	public Vector3 loadOffset;
	public Vector3 loadNoise;

	[Header("Circle")]
	public Vector3 circleRotateSpeed;
	public Transform circle;

	[Header("Targert")]
	public Transform target;
	public Vector3 TargetDirection { get { return (target.position - transform.position).normalized; } }
	public bool TargetIsForward { get { return Vector3.Dot (transform.forward, TargetDirection) > 0f; } }

	public void Initialize() {
		trail.material = Material.Instantiate (trail.material);
	}

	public void Load() {
		gameObject.SetActive (true);
		trail.enabled = false;

		cur.mode = Mode.Load;
	}

	public void Fire() {
		transform.forward = (-TargetDirection + loadNoise.GenerateRandom() + loadOffset).normalized;
		trail.enabled = true;

		cur.mode = Mode.Fire;
		Move ().While (cur.Is(Mode.Fire)).StartBy (this);
		ForwardTarget ().While (cur.Is(Mode.Fire)).StartBy (this);
	}

	public void Hit() {
		cur.mode = Mode.None;

		Explotion ().StartBy (this);

		gameObject.SetActive (false);
	}

	IEnumerator Explotion() {
		GameObject obj = PoolManager.Instance.GetInstance ("Detonator-Tiny");
		obj.transform.position = transform.position;
		Detonator detonator = obj.GetComponent<Detonator> ();
		detonator.explodeOnStart = false;

		detonator.UpdateComponents ();
		detonator.Explode ();
		yield return null;
	}

	public void Unload() {
		cur.mode = Mode.None;

		gameObject.SetActive (false);
		trail.enabled = false;
	}

	IEnumerator ForwardTarget() {
		while (true) {
			transform.forward = Vector3.Lerp(transform.forward, TargetDirection, angleSpeed * Time.deltaTime).normalized;
			yield return null;
		}
	}

	IEnumerator Move() {
		while (true) {
			transform.position += transform.forward * speed * Time.deltaTime;
			yield return null;
		}
	}

	IEnumerator Ripple() {

		while (true) {
			
			yield return new WaitForSeconds (1f);
		}
	}

}
