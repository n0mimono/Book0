using UnityEngine;
using System.Collections;

public class UnityChanLocoSD : StateMachineBehaviour {
	public System.Action<int> OnExit = (hash) => {};

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		animator.SetInteger ("Action", 0);
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (layerIndex == 0) {
			OnExit (stateInfo.shortNameHash);
		}
	}

	public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash) {
	}

	public override void OnStateMachineExit(Animator animator, int stateMachinePathHash) {
	}

}
