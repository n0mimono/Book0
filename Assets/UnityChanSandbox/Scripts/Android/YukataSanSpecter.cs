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
		Vector3 fwd = myTrans.forward;

		yield return new ActionWithTime ((t, dt) =>
			myTrans.AddEulerAngleY(180f * turnSpeed * dt), turnSpeed);
		myTrans.forward = -1f * fwd;

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

		yield return new ActionWithTime ((t, dt) =>
			myTrans.AddEulerAngleY(180f * turnSpeed * dt), turnSpeed);
		myTrans.forward = fwd;

		yield return CheckTarget ().WhileInCount (3f);

		yield return null;
	}

	private IEnumerator CheckTarget() {
		Transform target = gameObject.FindOppositeCharacters ().RandomOrDefault().transform;

		while (true) {
			Vector3 src = myTrans.position + 3f * myTrans.forward;
			Vector3 dst = target.position;
			myTrans.LookAt (dst.Ground (src.y));

			RaycastHit hit;
			if (Physics.Linecast (src, dst, out hit)) {
				GameObject obj = hit.collider.gameObject;
				//Debug.Log (obj);

				if (obj.IsLayer(Common.Layer.Character) && obj.IsPlayerTag()) {
					OnPlayerFound(target);
				}
			}

			yield return null;
		}
	}

	private void OnPlayerFound(Transform target) {
		//Debug.Log (target);
	}

}
