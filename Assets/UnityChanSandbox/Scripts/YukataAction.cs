using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class YukataAction : MonoBehaviour {
	public CharacterController characterControl;
	public Animator animator;

	public float maxSpeed;
	public float charSpeedScale;
	public float animSpeedScale;

	private UnityChanLocoSD stateMachine;

	public enum AnimeAction {
		None   = 0,
		Salute = 1,
	}

	void Start() {
		stateMachine = animator.GetBehaviour<UnityChanLocoSD> ();
	}

	public void SetTargetVelocity(Vector3 tgtVelocity) {
		tgtVelocity = tgtVelocity.normalized * Mathf.Min (maxSpeed, tgtVelocity.magnitude);

		// move control
		tgtVelocity = tgtVelocity.magnitude < 0.2f ?  Vector3.zero : tgtVelocity;

		// position
		characterControl.Move (charSpeedScale * tgtVelocity * Time.deltaTime);

		// angle
		if (tgtVelocity.magnitude > 0.1f) {
			Transform trans = transform;
			trans.LookAt (trans.position + tgtVelocity);
		}

		// animation control
		animator.SetSpeed(tgtVelocity.magnitude * animSpeedScale);
	}

	public void StartAnimeAction(AnimeAction act, System.Action onCompleted) {
		stateMachine.OnExit = (hash) => onCompleted();
		animator.SetAnimeAction ((int)act);
	}

}

