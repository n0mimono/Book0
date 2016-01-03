using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Custom;

public class YukataChan : MonoBehaviour {

	public YukataAction yukataAction;
	public float walkingSpeed;
	public float runningSpeed;

	private Transform myTrans;

	public enum State {
		Search,
		Chase,
		Attack
	}

	private class Status {
		public State state;
		public IEnumerator routine;
		public float walkSpeed;
		public float turnSpeed;

		public Transform tgtTrans;
		public Transform aimTrans;

		public bool isUpdateCommon;

		public bool Is(State state) {
			return this.state == state;
		}
	}
	private Status cur = new Status();

	void Start() {
		myTrans = yukataAction.transform;
		yukataAction.InLockAction += InLockInterrupt;

		CreateAim ();

		StartCoroutine (UpdateCommon ());
		NextState (Search);
	}

	private void InLockInterrupt(YukataAction.AnimeAction act) {

	}

	private void CreateAim() {
		GameObject obj = new GameObject ("_Aim_" + myTrans.name);
		cur.aimTrans = obj.transform;
		cur.aimTrans.SetParent (GameObject.Find ("Misc").transform);
	}

	private void NextState(System.Func<IEnumerator> routiner) {
		if (cur.routine != null) {
			StopCoroutine (cur.routine);
		}

		cur.routine = routiner ();
		StartCoroutine (cur.routine);
	}

	private IEnumerator UpdateCommon() {
		cur.isUpdateCommon = true;

		while (true) {
			if (!cur.isUpdateCommon) {
				yield return null;
				continue;
			}

			Vector3 tgtDir = (cur.aimTrans.position - myTrans.position).normalized;

			// direction
			if (!IsStraightToTarget(0.99f)) {
				myTrans.forward = Vector3.Slerp (myTrans.forward, tgtDir, cur.turnSpeed * Time.deltaTime);
			}

			// position
			yukataAction.SetTargetVelocity (myTrans.forward * cur.walkSpeed);

			yield return null;
		}
	}

	public bool IsStraightToTarget(float threshold) {
		Vector3 tgtDir = (cur.aimTrans.position - myTrans.position).normalized;
		return myTrans.forward.Similarity (tgtDir) > threshold;
	}

	public bool IsNearTarget(float distance) {
		return Vector3.Distance(myTrans.position, cur.aimTrans.position) < distance;
	}

	private IEnumerator Search() {
		cur.state = State.Search;
		cur.turnSpeed = 1f;

		while (true) {
			yield return null;
			if (IsStraightToTarget(0.8f) || IsNearTarget(10f)) {
				NextState (Chase);
			}
			yield return null;

			float r = Random.value * 10f + 10f;
			float t = (2f * Random.value - 1f) * 90f;
			Vector3 m = Quaternion.AngleAxis (t, Vector3.up) * myTrans.forward;
			cur.aimTrans.position = myTrans.position + m * r;
			yield return new WaitForSeconds (2f);

			cur.walkSpeed = walkingSpeed;
			yield return new WaitForSeconds (5f);

			cur.walkSpeed = 0.1f;
			yield return new WaitForSeconds (2f);
		}

	}

	private IEnumerator Chase() {
		cur.state = State.Chase;
		cur.turnSpeed = 10f;

		// temp solution
		cur.tgtTrans = GameObject.FindGameObjectWithTag ("Player").transform;

		cur.walkSpeed = runningSpeed;
		while (true) {
			cur.aimTrans.position = cur.tgtTrans.position;

			if (IsStraightToTarget (0.95f) && IsNearTarget (5f)) {
				NextState (Attack);
			} else if (!IsStraightToTarget (0f) || !IsNearTarget (10f)) {
				NextState (Search);
			}

			yield return null;
		}
	}

	private IEnumerator Attack() {
		cur.state = State.Attack;
		cur.turnSpeed = 0.1f;

		yield return null;
		cur.isUpdateCommon = false;
		YukataAction.LockHandler onCompleted = (act) => cur.isUpdateCommon = true;

		yukataAction.StartLockedAction (YukataAction.AnimeAction.Dive, onCompleted, false);
		yukataAction.SetDiveVelocity (myTrans.forward);

		while (!cur.isUpdateCommon) {
			yield return null;
		}

		NextState (Chase);
	}

}
