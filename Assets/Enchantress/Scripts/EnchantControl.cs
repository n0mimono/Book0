using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Custom;
using System;

public partial class EnchantControl : MonoBehaviour {
	public List<MagicCircle> circles;
	public List<MagicSpawner> magazines;

	public enum TargetMode {
		Single,
		Multi
	}

	public Transform primaryTarget;
	public List<Transform> targetList;

	private bool isActive;

	public void Initilize(string tag) {
		gameObject.tag = tag;
		magazines.ForEach (m => m.Initilize (gameObject.tag));
	}

	public void Hold() {
		isActive = true;

		circles.ForEach (c => c.Hold ());
	}

	public void Release() {
		if (!isActive) return;

		circles.ForEach (c => c.Release ());
		Unload ();

		isActive = false;
	}

	public void Load() {
		if (!isActive) return;

		MagicSpawner magazine = magazines
			.Where (m => !m.IsLoaded).FirstOrDefault ();

		if (magazine != null) {
			magazine.Load ();
		}
	}

	public void LoadAll() {
		if (!isActive)
			return;

		magazines
			.Where (m => !m.IsLoaded)
			.ToList ()
			.ForEach (m => m.Load ());
	}

	public IEnumerator LoadSequential(float interval) {
		while (magazines.Where (m => !m.IsLoaded).Any ()) {
			yield return new WaitForSeconds (interval);
			Load ();
		}
	}

	public void Unload() {
		if (!isActive) return;

		magazines
			.Where (m => m.IsLoaded)
			.ToList ()
			.ForEach (m => m.Unload ());
	}

	public void Fire(TargetMode mode) {
		if (!isActive) return;

		magazines
			.Where (m => m.IsLoaded)
			.ToList ()
			.ForEach (m => m.Fire (GetTarget(mode)));
	}

	public void RandomLoadAndFire(TargetMode mode) {
		if (!isActive) return;

		MagicSpawner magazine = magazines
			.Where (m => !m.IsLoaded).RandomOrDefault ();
		if (magazine == null) {
			return;
		}

		magazine.Load ();
		magazine.Fire (GetTarget (mode));
	}

	public void SetTarget (Transform primaryTarget) {
		this.primaryTarget = primaryTarget;
	}
	public void SetTarget(List<Transform> targetList) {
		this.targetList = targetList;
	}

	public Transform GetTarget(TargetMode mode) {
		if (mode == TargetMode.Single) {
			return primaryTarget;
		} else {
			return targetList.RandomOrDefault ();
		}
	}

}

public partial class EnchantControl {
	[Button("Initilize", "Init As Player", "Player")] public float ButtonInitPlayer;
	[Button("Initilize", "Init As Enemy", "Enemy")] public float ButtonInitEnemy;

	[Button("Hold", "Hold")] public float ButtonHold;
	[Button("LoadAll", "LoadAll")] public float ButtonLoadAll;

	[Button("Fire", "Fire Single", TargetMode.Single)] public float ButtonFireSingle;
	[Button("Fire", "Fire Multi", TargetMode.Multi)] public float ButtonFireMulti;

	public void ForceFire() {
		Fire (TargetMode.Single);
	}

	public void ForceHit() {
		PoolManager.Instance.gameObject
			.GetComponentsInChildren<MagicBullet> (false)
			.ToList ()
			.ForEach (b => b.Hit (b.transform.position));
		Unload ();
	}

}
