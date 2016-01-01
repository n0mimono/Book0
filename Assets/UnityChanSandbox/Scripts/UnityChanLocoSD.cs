using UnityEngine;
using System.Collections;

public class UnityChanLocoSD : StateMachineBehaviour {
	public System.Action<int> OnExit = (hash) => {};

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		animator.InitAnimeAction ();
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (layerIndex == 0) {
			OnExit (stateInfo.shortNameHash);
		}
	}

}
