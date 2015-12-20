using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MultiTargetCamera : MonoBehaviour {
	public List<Transform> targets;

	private Vector3 targetAngle;
	private Vector3 targetPos;

	void LateUpdate() {
		int n = targets.Count;
		Vector3 center = targets.Select (t => t.position).Aggregate (Vector3.zero, (m, p) => m += p * (1f / n));
		float range = targets.Select (t => t.position).Aggregate (0f, (m, p) => m += Vector3.Distance(p, center));

		float dist = Mathf.Max(Mathf.Pow(range, 0.5f) * 2f, 5f);

		transform.LookAt (center);
		Vector3 pos = center - transform.forward * dist;
		transform.position = pos;

		Transform camTrans = Camera.main.transform;
		camTrans.eulerAngles = Custom.Utility.AngleLerp (camTrans.eulerAngles, transform.eulerAngles, 2f * Time.deltaTime);
		camTrans.position = Vector3.Lerp(camTrans.position, transform.position, 2f * Time.deltaTime);
	}

}
