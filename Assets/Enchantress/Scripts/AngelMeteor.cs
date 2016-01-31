using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Custom;

public class AngelMeteor : MonoBehaviour {
	public GameObject photographer;

	public Transform target;
	public System.Action OnProcCompleted; 

	public List<MagicCircle> circles;
	public List<MagicMagazine> magazines;

	void Start() {
	}

	public void StartProc() {
		Proc ().StartBy (this);
	}

	private IEnumerator Proc() {
		photographer.SetActive (true);

		circles.ForEach (c => c.Hold ());
		yield return new WaitForSeconds (2f);;

		bool keep = true;
		ProcMeteor ().While (() => keep).StartBy (this);
		yield return new WaitForSeconds (10f);
		keep = false;

		circles.ForEach (c => c.Release ());
		yield return new WaitForSeconds(4);

		OnProcCompleted ();
	}

	private IEnumerator ProcMeteor() {
		magazines.ForEach (m => m.Load ());

		while (true) {
			MagicSpawner magazine = magazines.RandomOrDefault ();
			yield return null;

			magazine.Fire (null);
			yield return null;

			magazine.Load ();
			yield return new WaitForSeconds (0.1f);
		}
	}

}
