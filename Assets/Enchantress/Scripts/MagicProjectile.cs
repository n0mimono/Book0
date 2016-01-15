﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Custom;
using System;

public class MagicProjectile : MonoBehaviour {
	public Collider collide;

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

	[Header("Projectile")]
	public float speed;

	[Header("Targert")]
	public Transform target;
	public Vector3 TargetDirection { get {
			if (target == null) {
				return transform.forward;
			} else {
				return (target.position - transform.position).normalized;
			}
		} }

	[Header("Damage Source")]
	public DamageSource damageSource;

	protected string instanceName;
	protected Action<GameObject> OnExplotion = (obj) => {};

	protected void InitiilzeDamageSource() {
		damageSource.OnHit += (src) => {
			Hit();
		};
	}

	public virtual void Initialize() {
		collide.enabled = false;
		InitiilzeDamageSource ();
	}

	public virtual void Load() {
		gameObject.SetActive (true);
		collide.enabled = false;

		cur.mode = Mode.Load;
	}

	public virtual void Fire() {
		transform.forward = TargetDirection;
		collide.enabled = true;

		cur.mode = Mode.Fire;
		Move ().While (cur.Is(Mode.Fire)).StartBy (this);
	}

	public virtual void Hit() {
		if (!cur.Is (Mode.None)()) {
			Explotion ().StartBy (this);
		}
		cur.mode = Mode.None;
	}

	IEnumerator Explotion() {
		GameObject obj = PoolManager.Instance.GetInstance (instanceName);
		obj.transform.position = transform.position;

		OnExplotion (obj);
		yield return null;
		gameObject.SetActive (false);
	}

	public virtual void Unload() {
		cur.mode = Mode.None;

		gameObject.SetActive (false);
		collide.enabled = false;
	}

	IEnumerator Move() {
		while (true) {
			transform.position += transform.forward * speed * Time.deltaTime;
			yield return null;
		}
	}
}
