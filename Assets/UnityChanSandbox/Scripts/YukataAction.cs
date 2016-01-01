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

public static class AnimatorProxy {

	private static Dictionary<string, float> valueCaches = new Dictionary<string, float>();

	public static void SetFloatWithStartTrigger(this Animator animator, string name, float value, float threshold) {
		string key = animator.GetInstanceID ().ToString () + "_" + name;

		if (!valueCaches.ContainsKey (key)) {
			valueCaches [key] = 0f;
		}
		float prevValue = valueCaches [key];

		valueCaches [key] = value;
		animator.SetFloat (name, value);

		if (prevValue < threshold && value >= threshold) {
			animator.SetTrigger ("Start" + name);
		}

	}

	public static void SetSpeed(this Animator animator, float value) {
		animator.SetFloat ("Speed", value);

		AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo (0);
		bool isMoving = state.IsName ("Walking") || state.IsName ("Running");
		animator.SetBool ("Rest", !isMoving);
	}

	public static void SetAnimeAction(this Animator animator, int id) {
		animator.SetInteger ("Action", id);
	}

	public static void InitAnimeAction(this Animator animator) {
		animator.SetAnimeAction (0);
	}
}
