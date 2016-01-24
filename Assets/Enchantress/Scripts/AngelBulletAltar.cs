using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Custom;

public class AngelBulletAltar : MonoBehaviour {
	public Transform target;
	public System.Action OnProcCompleted; 

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

		public IEnumerator Update() {
			while (true) {
				main.localPosition += moveSpeed * Time.deltaTime;
				main.localEulerAngles += angleSpeed * Time.deltaTime;
				subs.ForEach (s => s.localPosition += subMoveSpeed * Time.deltaTime);
				yield return null;
			}
		}
		public IEnumerator UpdateOnce() {
			return Update().WhileInCount(time);
		}
	}
	public Bulletive bulletive;
	public Bulletive bulletive2;

	private enum CircleType {
		UnderOuterCircle = 0,
		UnderInnerCircle = 1,
		MidUnderRing = 2,
		MidInnerRing = 3,
		MidOuterRing = 4,
		MidUpperRing = 5,
		UpperInnerCircle = 6,
		UpperOuterCircle = 7,
		UpperUpperCircle = 8,
		Sky = 9,
	}

	void Start() {
		particles.ForEach (p => p.SetEmission (false));
		bulletive.main.gameObject.SetActive (false);
		bulletive2.main.gameObject.SetActive (false);
	}

	public void StartProc() {
		TraceTarget ().StartBy (this);
		Proc ().StartBy (this);
	}

	private IEnumerator Proc() {
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

		yield return new WaitForSeconds(5f);
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
