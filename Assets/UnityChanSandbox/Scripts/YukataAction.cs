using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class YukataAction : MonoBehaviour {
	public CharacterController characterControl;
	public Animator animator;

	public float maxSpeed;
	public float animSpeedScale;

	private Texture2D rimMask;

	IEnumerator Start() {
		
		rimMask = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		rimMask.SetPixel (0, 0, Color.black);
		rimMask.Apply ();
		yield return null;

		foreach (Renderer renderer in GetComponentsInChildren<Renderer> ()) {
			Material material = renderer.sharedMaterial;

			Material tmpMaterial = Material.Instantiate (material);
			tmpMaterial.SetFloat ("_EdgeThickness", 0f);
			tmpMaterial.SetTexture("_RimLightSampler", rimMask);

			renderer.sharedMaterial = tmpMaterial;
			yield return null;
		}

	}

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
