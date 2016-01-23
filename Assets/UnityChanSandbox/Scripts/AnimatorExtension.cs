using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class AnimatorExtension {

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

	public static void SetFace(this Animator animator, int id) {
		//Debug.Log ("id: " + id);
		animator.SetInteger ("FaceType", id);
	}

	public static void SetAlive(this Animator animator, bool isAlive) {
		animator.SetBool ("IsAlive", isAlive);
	}

	public static bool IsFullName(this AnimatorStateInfo stateInfo, string fullpath) {
		return stateInfo.fullPathHash == Animator.StringToHash (fullpath);
	}

	public static bool IsUnlockState(this AnimatorStateInfo stateInfo) {
		return stateInfo.IsTag ("Unlock");
	}

}
