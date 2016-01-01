using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
		None   = 0,
		Salute = 1,
		Jump   = 2,
		Dive   = 3,
	}

	void Start() {
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
		if (isWalkable) {
			animator.SetSpeed(curVelocity.magnitude * animSpeedScale);
		}
	}

	public void SetTargetVelocity(Vector3 tgtVelocity) {
		tgtVelocity = tgtVelocity.normalized * Mathf.Min (maxSpeed, tgtVelocity.magnitude);;
		tgtVelocity = tgtVelocity.magnitude < 0.2f ?  Vector3.zero : tgtVelocity;
		this.tgtVelocity = tgtVelocity;
	}

	public void SetDiveVelocity() {
		tgtVelocity = transform.forward * diveSpeed;
	}

	public void ForceStop() {
		tgtVelocity = Vector3.zero;
		curVelocity = Vector3.zero;
		UpdateVelocity ();
	}

	public void StartAnimeAction(AnimeAction act, System.Action onCompleted, bool isForceStop = false) {
		if (isForceStop) {
			ForceStop ();
		}

		stateMachines.ForEach (m => m.OnExit = (hash) => onCompleted());
		animator.SetAnimeAction ((int)act);
	}

	public void SetWalkable(bool isWalkable) {
		this.isWalkable = isWalkable;
	}
}

