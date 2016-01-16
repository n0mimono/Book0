using UnityEngine;
using System.Collections;
using System.Linq;
using System;
using Custom;

public class MagicSpawner : MonoBehaviour {
	public MagicProjectile projectile;

	public bool IsLoaded { get { return projectile != null; } }

	protected string instanceName;
	protected Action OnHold = () => {};
	protected Action OnRelease = () => {};

	public virtual void Initilize(string tag) {
		gameObject.tag = tag;
	}

	public void Load() {
		OnHold ();

		projectile = PoolManager.Instance.GetInstance (instanceName).GetComponent<MagicProjectile>();
		projectile.gameObject.tag = gameObject.tag;
		projectile.Initialize ();

		projectile.Load ();
	}

	public void Unload() {
		OnRelease ();

		projectile.Unload ();
		projectile = null;
	}

	public void Fire(Transform target) {
		OnRelease ();

		if (target != null) {
			projectile.transform.forward = transform.forward;
		}

		projectile.target = target;
		projectile.Fire ();
		projectile = null;
	}

	void Update() {
		if (projectile != null) {
			projectile.transform.position = transform.position;
		}
	}

	[Button("Initilize", "Initilize", "Player")] public float ButtonInitilize;
	[Button("Load", "Load")] public float ButtonLoad;
	[Button("ForceFireTarget", "Fire")] public float ButtonFire;
	[Button("ForceFireNonTarget", "Fire (None)")] public float ButtonFireNone;
	[Button("Unload", "Unload")] public float ButtonUnload;

	public void ForceFireTarget() {
		GameObject enemy = gameObject.FindOppositeCharacters ().FirstOrDefault ();
		Fire (enemy.transform);
	}

	public void ForceFireNonTarget() {
		Fire (null);
	}

}
