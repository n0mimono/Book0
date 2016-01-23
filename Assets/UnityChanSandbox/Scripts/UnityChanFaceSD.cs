using UnityEngine;
using System.Collections;

public class UnityChanFaceSD : StateMachineBehaviour {

	public enum FaceType {
		Loop    = 00,
		Salute  = 01,
		Running = 02,
		Damage  = 03,
		Idle    = 10,
	}

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

		if (stateInfo.IsFullName ("Base.Lock Action.Salute")) { // salute in
			animator.SetFace ((int)FaceType.Salute);
		} else if (stateInfo.IsFullName ("Face.Salute")) { // salute out
			animator.SetFace ((int)FaceType.Loop);
		} else if (stateInfo.IsFullName ("Base.Lock Action.GoDown")) { // damage in
			animator.SetFace ((int)FaceType.Damage);
		} else if (stateInfo.IsFullName ("Face.Damage")) { // damage out
			animator.SetFace ((int)FaceType.Loop);
		} if (stateInfo.IsFullName ("Face.Random")) { // idle
			int id = (int)FaceType.Idle + (int)(Random.value * 4f) + 1;
			animator.SetFace (id);
		} else {
			//animator.SetFace ((int)FaceType.Idle);
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		animator.SetFace ((int)FaceType.Idle);
	}

}
