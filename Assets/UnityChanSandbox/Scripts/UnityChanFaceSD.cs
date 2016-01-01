using UnityEngine;
using System.Collections;

public class UnityChanFaceSD : StateMachineBehaviour {

	public enum FaceType {
		Loop    = 00,
		Salute  = 01,
		Running = 02,
		Idle    = 10,
	}

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (stateInfo.IsFullName("Base.Salute")) {
			animator.SetFace ((int)FaceType.Salute);
		} else if (stateInfo.IsFullName("Face.Salute")) {
			animator.SetFace ((int)FaceType.Loop);
		} else if (stateInfo.IsFullName("Face.Random")) {
			int id = (int)FaceType.Idle + (int)(Random.value * 4f) + 1;
			animator.SetFace (id);
		} else {
			animator.SetFace ((int)FaceType.Idle);
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		animator.SetFace ((int)FaceType.Idle);
	}

}
