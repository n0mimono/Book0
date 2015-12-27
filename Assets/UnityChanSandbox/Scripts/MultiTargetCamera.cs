﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Custom;

public class MultiTargetCamera : MonoBehaviour {
	public List<Target> targets;

	private IEnumerator curRoutine;

	[System.Serializable]
	public class Target {
		public Transform trans;
		public bool isPlayer;
		public bool isEnemy;
	}

	void Start() {
		StartRegularUpdate ();
	}

	void LateUpdate() {
		if (curRoutine != null) {
			curRoutine.MoveNext ();
		}
		CommonUpdate ();
	}

	private void CommonUpdate() {
		Vector3 pos = transform.position;
		Vector3 ang = transform.eulerAngles;

		Transform camTrans = Camera.main.transform;
		camTrans.eulerAngles = Custom.Utility.AngleLerp (camTrans.eulerAngles, ang, 2f * Time.deltaTime);
		camTrans.position = Vector3.Lerp(camTrans.position, pos, 2f * Time.deltaTime);
	}

	void StartLateCoroutine(IEnumerator routine) {
		curRoutine = routine;
	}

	private void StartRegularUpdate() {
		StartLateCoroutine (UpdateRoutine (RegularUpdate));
	}

	private void RegularUpdate() {
		int n = targets.Count;
		Vector3 center = targets.Select (t => t.trans.position).Aggregate (Vector3.zero, (m, p) => m += p * (1f / n));
		float range = targets.Select (t => t.trans.position).Aggregate (0f, (m, p) => m += Vector3.Distance(p, center));

		float dist = Mathf.Max(Mathf.Pow(range, 0.5f) * 2f, 5f);

		transform.LookAt (center);
		Vector3 pos = center - transform.forward * dist;
		pos.y = 2f;
		transform.position = pos;
	}

	private IEnumerator UpdateRoutine(System.Action update) {
		while (true) {
			update ();
			yield return null;
		}
	}

	public void StartMagi() {
		StartLateCoroutine (Magi ());
	}

	private IEnumerator Magi() {
		Transform player = targets.Where (t => t.isPlayer).Select (t => t.trans).FirstOrDefault ();
		Transform enemy = targets.Where (t => t.isEnemy).Select (t => t.trans).FirstOrDefault ();

		transform.LookAt (player.position + Vector3.up * 1f);
		transform.position = player.position - transform.forward * 2f;
		transform.SetPositionY (1.5f);

		for (float time = 0f; time < 2f; time += Time.deltaTime) yield return null;
		yield return null;

		transform.LookAt (enemy.position + Vector3.up * 1f);
		for (float time = 0f; time < 5f; time += Time.deltaTime) {
			transform.AddEulerAngleY (Time.deltaTime * 15f);
			transform.position = enemy.position - transform.forward * (4f + time * 1.5f);
			yield return null;
		}

		transform.LookAt (enemy.position + Vector3.up * 7f);
		for (float time = 0f; time < 5f; time += Time.deltaTime) yield return null;

		StartRegularUpdate ();
	}
}