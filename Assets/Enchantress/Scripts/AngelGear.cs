using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Custom;

public class AngelGear : AngelAltar {
	[Header("Gear")]

	public List<MagicCircle> circles;
	public List<ParticleSystem> particles;

	public string instanceName;
	public int magazineNums;
	public Transform magazinePoint;
	public List<MagicMagazine> magazines;

	void Start() {
		particles.ForEach (p => p.SetEmission (false));
	}

	public override void StartProc() {
		Proc ().StartBy (this);
	}

	private IEnumerator Proc() {
		photographer.SetActive (true);

		yield return null;
		circles [0].Hold ();
		yield return new WaitForSeconds(0.6f);
		circles [1].Hold ();
		yield return new WaitForSeconds(0.5f);
		circles [2].Hold ();
		yield return new WaitForSeconds(0.4f);
		circles [3].Hold ();
		yield return new WaitForSeconds(0.3f);
		circles [4].Hold ();
		yield return new WaitForSeconds(0.3f);
		circles [5].Hold ();
		yield return new WaitForSeconds(0.2f);
		circles [6].Hold ();
		yield return new WaitForSeconds(0.2f);
		circles [7].Hold ();
		yield return new WaitForSeconds(0.2f);
		circles [8].Hold ();
		yield return new WaitForSeconds(0.1f);
		circles [9].Hold ();
		yield return new WaitForSeconds(0.1f);
		circles [10].Hold ();
		yield return new WaitForSeconds(0.1f);
		circles [11].Hold ();
		yield return new WaitForSeconds(3f);
		particles.ForEach (p => p.SetEmission (true));
		StartCoroutine (LoadMagazines ());

		yield return new WaitForSeconds(2f);
		magazines.ForEach (m => m.Fire (target));

		yield return new WaitForSeconds(5f);
		circles.ForEach (c => c.Release ());

		yield return new WaitForSeconds(5f);
		particles.ForEach (p => p.SetEmission (false));

		yield return new WaitForSeconds(5f);
		OnProcCompleted ();
	}

	private IEnumerator LoadMagazines() {
		
		magazines.Clear ();
		for (int i = 0; i < magazineNums; i++) {
			GameObject obj = PoolManager.Instance.GetInstance (instanceName);
			Transform trans = obj.transform;

			trans.position = magazinePoint.position;
			trans.forward = Random.insideUnitSphere.normalized;

			MagicMagazine magazine = obj.GetComponent<MagicMagazine>();
			magazine.Initilize (gameObject.tag);
			magazine.Load ();

			magazines.Add (magazine);
			yield return null;
		}

	}

}
