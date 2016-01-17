using UnityEngine;
using System.Collections;

public class DragonStateMachine : StateMachineBehaviour {
	public System.Action<int> OnEnter = (hash) => {};
	public System.Action<int> OnExit = (hash) => {};

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		OnEnter(stateInfo.fullPathHash);
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		OnExit (stateInfo.fullPathHash);
	}

}
