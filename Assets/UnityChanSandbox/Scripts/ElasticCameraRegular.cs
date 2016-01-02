using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Custom;

public partial class ElasticCameraOperator {

	[System.Serializable]
	public class TargetingParams {
		public float height;
	}
	[Header("Targeting Mode Details")]
	public TargetingParams targettingParams;

	[System.Serializable]
	public class Target {
		public Transform trans;
		public bool isPlayer;
		public bool isEnemy;
	}
	public List<Target> targets;

	private void TargettingUpdate() {
		float height = targettingParams.height;
		cur.lerpSpeed = cur.scheme.lerpSpeed;

		int n = targets.Count;
		Vector3 center = targets.Select (t => t.trans.position).Aggregate (Vector3.zero, (m, p) => m += p * (1f / n));
		float range = targets.Select (t => t.trans.position).Aggregate (0f, (m, p) => m += Vector3.Distance(p, center));

		float dist = Mathf.Max(Mathf.Pow(range, 0.5f) * 2f, 5f);

		opTrans.LookAt (center);
		Vector3 pos = center - opTrans.forward * dist;
		opTrans.position = pos.Ground(height);
	}

}
