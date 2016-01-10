using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Custom;

public class EnchantControl : MonoBehaviour {
	public MagicCircle circle;
	public List<EnchantRotater> rotaters;
	public List<MagicMagazine> magazines;

	public Transform target;

	private List<MagicBullet> bullets;
	private bool isActive;

	[Button("Hold", "Hold")] public float ButtonHold;
	[Button("LoadAll", "LoadAll")] public float ButtonLoadAll;
	[Button("Fire", "Fire")] public float ButtonFire;
	[Button("ForceHit", "Hit")] public float ButtonForceHit;

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

	public void Fire() {
		if (!isActive) return;

		magazines
			.Where (m => m.IsLoaded)
			.ToList ()
			.ForEach (m => m.Fire (target));
	}

	public void ForceHit() {
		PoolManager.Instance.gameObject
			.GetComponentsInChildren<MagicBullet> (false)
			.ToList ()
			.ForEach (b => b.Hit ());
	}

}
