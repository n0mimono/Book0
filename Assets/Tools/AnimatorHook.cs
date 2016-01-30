using UnityEngine;
using System.Collections;

public class AnimatorHook : StateMachineBehaviour {

	public bool isDisableGameObjectOnExit;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (isDisableGameObjectOnExit) {
			animator.gameObject.SetActive (false);
		}
	}

}
