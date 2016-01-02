﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Custom;

public partial class ElasticCameraOperator : MonoBehaviour {
	public List<Target> targets;

	private IEnumerator curRoutine;

	private float lerpSpeed = 2f;

	[System.Serializable]
	public class Target {
		public Transform trans;
		public bool isPlayer;
		public bool isEnemy;
	}

	private struct CameraProp {
		public Vector3 pos;
		public Vector3 ang;
		public float lerpSpeed;
	}
	private Stack<CameraProp> propStack = new Stack<CameraProp>();

	private void Push() {
		CameraProp prop = new CameraProp() {
			pos = transform.position, ang = transform.eulerAngles, lerpSpeed = lerpSpeed
		};
		propStack.Push(prop);
	}
	private void Pop() {
		CameraProp prop = propStack.Pop ();
		transform.position = prop.pos;
		transform.eulerAngles = prop.ang;
		lerpSpeed = prop.lerpSpeed;
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
		camTrans.eulerAngles = Custom.Utility.AngleLerp (camTrans.eulerAngles, ang, lerpSpeed * Time.deltaTime);
		camTrans.position = Vector3.Lerp(camTrans.position, pos, lerpSpeed * Time.deltaTime);
	}

	void StartLateCoroutine(IEnumerator routine) {
		curRoutine = routine;
	}

	private IEnumerator UpdateRoutine(System.Action update) {
		while (true) {
			update ();
			yield return null;
		}
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

}