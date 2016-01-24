using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Custom;

public class YukataSanSpecter : MonoBehaviour {
	[Header("Yukata Android")]
	public YukataAction yukataAction;
	protected Transform myTrans;

	[Header("Yukata-Chan")]
	public float turnSpeed;

	private bool isChecking;

	void Start() {
		myTrans = yukataAction.transform;

		ProcDaruma ().StartBy (this);
	}

	private IEnumerator ProcDaruma() {
		while (true) {
			yield return StartCoroutine (Daruma ());
			yield return null;
		}
	}

	private IEnumerator Daruma() {
		yield return new WaitForSeconds (0.5f);
		yield return new WaitForSeconds (0.5f);
		yield return new WaitForSeconds (0.5f);
		yield return new WaitForSeconds (0.5f);
		yield return new WaitForSeconds (0.5f);
		yield return new WaitForSeconds (0.5f);
		yield return new WaitForSeconds (0.5f);
		yield return new WaitForSeconds (0.5f);
		yield return new WaitForSeconds (0.5f);
		yield return new WaitForSeconds (0.5f);

		yield return new WaitForSeconds (3f);

		yield return null;
	}

	private IEnumerator CheckTarget() {
		Transform target = gameObject.FindOppositeCharacters ().RandomOrDefault().transform;

		Vector3 curPos = target.position;

		while (true) {
			Vector3 prevPos = curPos;
			curPos = target.position;
			myTrans.LookAt (curPos);

			float spd = Vector3.Distance (prevPos, curPos) / Time.deltaTime;

			yield return null;
		}
	}

	private void OnPlayerFound(Transform target) {
		//Debug.Log (target);
	}

}
