using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Custom;

public partial class EnchantControl : MonoBehaviour {
	public MagicCircle circle;
	public List<EnchantRotater> rotaters;
	public List<MagicMagazine> magazines;

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

		circle.Hold ();
	}

	public void Release() {
		if (!isActive) return;

		circle.Release ();
		Unload ();

		isActive = false;
	}

	public void Load() {
		if (!isActive) return;

		MagicMagazine magazine = magazines
			.Where (m => !m.IsLoaded).FirstOrDefault ();

		if (magazine != null) {
			magazine.Load ();
		}
	}

	public void LoadAll() {
		if (!isActive) return;

		magazines
			.Where (m => !m.IsLoaded)
			.ToList ()
			.ForEach (m => m.Load ());
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

	public void ForceFire() {
		Fire (TargetMode.Single);
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
	[Button("Hold", "Hold")] public float ButtonHold;
	[Button("LoadAll", "LoadAll")] public float ButtonLoadAll;
	[Button("ForceFire", "Fire")] public float ButtonFire;
	[Button("ForceHit", "Hit")] public float ButtonForceHit;

	public void ForceHit() {
		PoolManager.Instance.gameObject
			.GetComponentsInChildren<MagicBullet> (false)
			.ToList ()
			.ForEach (b => b.Hit ());
		Unload ();
	}

}
