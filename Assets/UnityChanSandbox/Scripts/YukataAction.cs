﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Custom;

public class YukataAction : MonoBehaviour {
	public CharacterController characterControl;
	public Animator animator;

	public float maxSpeed;
	public float diveSpeed;
	public float acceleration;
	public float charSpeedScale;
	public float animSpeedScale;

	private List<UnityChanLocoSD> stateMachines;

	private bool isWalkable;
	private Vector3 tgtVelocity;
	private Vector3 curVelocity;

	public enum AnimeAction {
		None    = 0,
		Salute  = 1,
		Jump    = 2,
		Dive    = 3,
		Damaged = 4,
	}
	private AnimeAction inAction;
	public AnimeAction InAction { get { return inAction; } }

	public delegate void LockHandler(AnimeAction act);
	public event LockHandler InLockAction;
	public event LockHandler OutLockAction;

	void Start() {
		InLockAction += (act) => {
			SetWalkable (false);
			inAction = act;
		};
		OutLockAction += (act) => {
			SetWalkable (true);
			inAction = AnimeAction.None;
		};

		stateMachines = animator.GetBehaviours<UnityChanLocoSD> ().ToList();

		SetWalkable (true);
	}

	void Update() {
		UpdateVelocity ();
	}

	private void UpdateVelocity() {
		// move control
		curVelocity = Vector3.Lerp (curVelocity, tgtVelocity, acceleration * Time.deltaTime);

		// position
		characterControl.Move (charSpeedScale * curVelocity * Time.deltaTime);

		// angle
		if (curVelocity.magnitude > 0.1f) {
			Transform trans = transform;
			trans.LookAt (trans.position + tgtVelocity);
		}

		// animation control
		float animSpeed = isWalkable ? curVelocity.magnitude * animSpeedScale : 0f;
		animator.SetSpeed (animSpeed);
	}

	public void SetTargetVelocity(Vector3 tgtVelocity) {
		tgtVelocity = tgtVelocity.normalized * Mathf.Min (maxSpeed, tgtVelocity.magnitude);;
		tgtVelocity = tgtVelocity.magnitude < 0.2f ?  Vector3.zero : tgtVelocity;
		this.tgtVelocity = tgtVelocity;
	}

	public void SetDiveVelocity(Vector3 dir) {
		transform.forward = dir;
		tgtVelocity = dir * diveSpeed;
	}

	private void ForceStop() {
		tgtVelocity = Vector3.zero;
		curVelocity = Vector3.zero;
		UpdateVelocity ();
	}

	public void StartLockedAction(AnimeAction act, LockHandler onCompleted, bool isForceStop = false) {
		if (isForceStop) {
			ForceStop ();
		}

		// anime
		InLockAction(act);
		stateMachines.ForEach (m => m.OnExit = (hash) => {
			OutLockAction(act);
			onCompleted(act);
		});
		animator.SetAnimeAction ((int)act);
	}

	private void SetWalkable(bool isWalkable) {
		this.isWalkable = isWalkable;
	}

	void OnControllerColliderHit(ControllerColliderHit hit) {
		if (!hit.collider.gameObject.IsLayer (Common.Layer.Character)) {
			return;
		}

		YukataAction opponent = hit.collider.GetComponent<YukataAction> ();
		AttackTo (opponent);
	}

	private void AttackTo(YukataAction receiver) {
		if (InAction == AnimeAction.Dive && receiver.InAction != AnimeAction.Damaged) {
			receiver.OnAttackedBy (this);
		}
	}

	private void OnAttackedBy(YukataAction server) {
		YukataAction.LockHandler onCompleted = (act) => {};
		StartLockedAction (AnimeAction.Damaged, onCompleted, true);
		transform.LookAt (server.transform);
	}

}

