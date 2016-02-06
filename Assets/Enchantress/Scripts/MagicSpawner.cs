using UnityEngine;
using System.Collections;
using System.Linq;
using System;
using Custom;

public class MagicSpawner : MonoBehaviour {
	public MagicProjectile projectile;
	public string instanceName;
	public Vector3 positionNoise;
	public bool getRawTarget;

	public Transform projPosTrans;

	public bool IsLoaded { get { return projectile != null; } }

	protected Action OnHold = () => {};
	protected Action OnRelease = () => {};

	protected Vector3 generatedPositionNoise;

	public virtual void Initilize(string tag) {
		gameObject.SetTag (tag, true);
	}

	public void Load() {
		generatedPositionNoise = positionNoise.GenerateRandom ();

		projectile = PoolManager.Instance.GetInstance (instanceName).GetComponent<MagicProjectile>();
		projectile.gameObject.SetTag(gameObject.tag, true);
		projectile.Initialize ();

		projectile.Load ();

		OnHold ();
	}

	public void Unload() {
		OnRelease ();

		projectile.Unload ();
		projectile = null;
	}

	public void Fire(Transform target) {
		OnRelease ();

		if (projectile == null) return;
		projectile.transform.forward = transform.forward;

		projectile.target = getRawTarget ? target : target.GetTarget();
		projectile.Fire ();
		projectile = null;
	}

	protected virtual void Update() {
		if (projectile != null) {
			if (projPosTrans != null) {
				projectile.transform.position = projPosTrans.position + generatedPositionNoise;
			} else {
				projectile.transform.position = transform.position + generatedPositionNoise;
			}
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
