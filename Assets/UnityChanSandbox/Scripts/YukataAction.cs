using UnityEngine;
using System.Collections;

public class YukataAction : MonoBehaviour {
	public CharacterController characterControl;
	public Animator animator;

	public float maxSpeed;
	public float animSpeedScale;

	public void SetTargetVelocity(Vector3 tgtVelocity) {
		tgtVelocity = tgtVelocity.normalized * Mathf.Min (maxSpeed, tgtVelocity.magnitude);
		if (tgtVelocity.magnitude < 0.5f) {
			tgtVelocity = Vector3.zero;
		}

		// position
		characterControl.Move (tgtVelocity * Time.deltaTime);

		// angle
		if (tgtVelocity.magnitude > 0.1f) {
			Transform trans = transform;
			trans.LookAt (trans.position + tgtVelocity);
		}

		// animation
		animator.SetFloat("Speed", tgtVelocity.magnitude * animSpeedScale);

	}

}
