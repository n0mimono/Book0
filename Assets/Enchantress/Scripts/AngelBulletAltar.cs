﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Custom;

public class AngelBulletAltar : AngelAltar {
	[Header("Bullet")]
	public List<MagicCircle> circles;
	public List<ParticleSystem> particles;
	public List<MagicMagazine> magazines;

	[System.Serializable]
	public class Bulletive {
		public Transform main;
		public List<Transform> subs;
		public Vector3 moveSpeed;
		public Vector3 angleSpeed;
		public Vector3 subMoveSpeed;
		public float time;

		public void Initilize() {
		}

		public IEnumerator Update() {
			while (true) {
				main.localPosition += moveSpeed * Time.deltaTime;
				main.localEulerAngles += angleSpeed * Time.deltaTime;
				subs.ForEach (s => s.localPosition += subMoveSpeed * Time.deltaTime);
				yield return null;
			}
		}
		public IEnumerator UpdateOnce() {
			main.Tell<MemorableTransform> (m => m.InitilizeLocal ());
			subs.ForEach(s => s.Tell<MemorableTransform>(m => m.InitilizeLocal ()));
			subs.ForEach(s => s.Tell<TrailRenderer> (t => t.Clear ()));

			return Update().WhileInCount(time);
		}
	}
	public Bulletive bulletive;
	public Bulletive bulletive2;

	void Start() {
		particles.ForEach (p => p.SetEmission (false));
		bulletive.main.gameObject.SetActive (false);
		bulletive2.main.gameObject.SetActive (false);
	}

	public override void StartProc() {
		TraceTarget ().StartBy (this);
		Proc ().StartBy (this);
	}

	private IEnumerator Proc() {
		photographer.SetActive (true);

		particles.ForEach (p => p.SetEmission (true));
		yield return null;

		circles [0].Hold ();
		yield return new WaitForSeconds(1f);
		circles [1].Hold ();
		yield return new WaitForSeconds(0.5f);
		circles [2].Hold ();
		yield return new WaitForSeconds(0.5f);
		circles [3].Hold ();
		bulletive.main.gameObject.SetActive (true);
		bulletive.UpdateOnce ().StartBy (this);
		yield return new WaitForSeconds(0.5f);
		circles [4].Hold ();
		circles [5].Hold ();
		yield return new WaitForSeconds(0.5f);
		circles [6].Hold ();
		bulletive2.main.gameObject.SetActive (true);
		bulletive2.UpdateOnce ().StartBy (this);
		yield return new WaitForSeconds(1f);
		circles [7].Hold ();
		StartCoroutine (LoadMagazines ());
		yield return new WaitForSeconds(1f);
		circles [8].Hold ();
		circles [9].Hold ();

		yield return new WaitForSeconds(2);
		magazines.ForEach (m => m.Fire (target));
		circles.ForEach (c => c.Release ());
		particles.ForEach (p => p.SetEmission (false));

		yield return new WaitForSeconds(0.5f);
		for (int i = 0; i < 15; i++) { // 1 sec
			MagicMagazine magazine = magazines.RandomOrDefault();
			magazine.Load ();
			yield return null;
			magazine.Fire (target);
			yield return null;
		}

		yield return new WaitForSeconds(4);

		OnProcCompleted ();
	}

	private IEnumerator LoadMagazines() {
		foreach (MagicMagazine magazine in magazines) {
			magazine.Initilize (gameObject.tag);
			magazine.Load ();
			yield return null;
		}
	}

	private IEnumerator TraceTarget() {
		while (true) {
			transform.position = target.position;
			yield return null;
		}
	}

	public void Release() {
		circles.ForEach (c => c.Release ());
		particles.ForEach (p => p.SetEmission (false));
	}

	[Button("StartProc", "StartProc")] public int ButtonStartProc;
	[Button("Release", "Release")] public int ButtonRelease;

}
