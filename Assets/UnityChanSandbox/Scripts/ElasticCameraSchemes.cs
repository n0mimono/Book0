using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Custom;

public partial class ElasticCameraOperator {

	[System.Serializable]
	public class Target {
		public Transform trans;
		public bool isPlayer;
		public bool isEnemy;
	}
	[Header("Targets")]
	public List<Target> targets;

	private Target Player { get { return targets.Where (t => t.isPlayer).FirstOrDefault (); } }

	public void SetEnemyTarget(Transform target) {
		Target enemy = targets.Where (t => t.isEnemy).FirstOrDefault ();
		if (enemy == null) {
			enemy = new Target ();
			enemy.isEnemy = true;
			targets.Add (enemy);
		}
		enemy.trans = target;
	}
}

public partial class ElasticCameraOperator {

	[System.Serializable]
	public class ForwardingParams {
		public float height;
		public float back;
		public float forward;
		public float heightOffset;
	}
	[Header("Forwarding Mode Details")]
	public ForwardingParams forwardingParams;

	private void ForwardingUpdate() {
		float height = forwardingParams.height;
		float back = forwardingParams.back;
		float forward = forwardingParams.forward;
		float heightOffset = forwardingParams.heightOffset;
		cur.lerpSpeed = cur.scheme.lerpSpeed;

		Vector3 pos = Player.trans.position + Player.trans.forward * back;
		opTrans.position = pos.Ground (height);

		Vector3 fwd = Player.trans.position + Player.trans.forward * forward + Vector3.up * heightOffset;
		opTrans.LookAt (fwd);
	}
		
	public void ForwardUpdateImmediate() {
		ForwardingUpdate ();
	}
}

public partial class ElasticCameraOperator {
	[System.Serializable]
	public class PlayerTargetingParams {
		public float height;
		public float back;
	}
	[Header("Player-Targeting Mode Details")]
	public PlayerTargetingParams pTargettingParams;

	private void PlayerTargettingUpdate() {
		float height = pTargettingParams.height;
		float back = pTargettingParams.back;
		cur.lerpSpeed = cur.scheme.lerpSpeed;

		Vector3 pos = Player.trans.position + opTrans.forward * back;
		opTrans.position = pos.Ground (height);
	}
}

public partial class ElasticCameraOperator {

	[System.Serializable]
	public class MultiTargetingParams {
		public float height;
		public float distScale;
		public float distPower;
		public float minDist;
		public float heightOffset;
	}
	[Header("Multi-Targeting Mode Details")]
	public MultiTargetingParams mTargettingParams;

	private void MultiTargettingUpdate() {
		float height = mTargettingParams.height;
		float distScale = mTargettingParams.distScale;
		float distPower = mTargettingParams.distPower;
		float minDist = mTargettingParams.minDist;
		float heightOffset = mTargettingParams.heightOffset;
		cur.lerpSpeed = cur.scheme.lerpSpeed;

		int n = targets.Count;
		Vector3 center = targets.Select (t => t.trans.position).Aggregate (Vector3.zero, (m, p) => m += p * (1f / n));
		float range = targets.Select (t => t.trans.position).Aggregate (0f, (m, p) => m += Vector3.Distance(p, center));
		float dist = Mathf.Max(Mathf.Pow(range, distPower) * distScale, minDist);

		opTrans.LookAt (center + Vector3.up * heightOffset);
		Vector3 pos = center - opTrans.forward * dist;
		opTrans.position = pos.Ground(height);
	}

}

public partial class ElasticCameraOperator {

	private IEnumerator Magi() {
		cur.lerpSpeed = cur.scheme.lerpSpeed;

		Transform player = targets.Where (t => t.isPlayer).Select (t => t.trans).FirstOrDefault ();
		Transform enemy = targets.Where (t => t.isEnemy).Select (t => t.trans).FirstOrDefault ();

		opTrans.LookAt (player.position + Vector3.up * 1f);
		opTrans.position = player.position - opTrans.forward * 2f;
		opTrans.SetPositionY (1.5f);

		for (float time = 0f; time < 2f; time += Time.deltaTime) yield return null;
		yield return null;

		opTrans.LookAt (enemy.position + Vector3.up * 1f);
		for (float time = 0f; time < 5f; time += Time.deltaTime) {
			opTrans.AddEulerAngleY (Time.deltaTime * 15f);
			opTrans.position = enemy.position - opTrans.forward * (4f + time * 1.5f);
			yield return null;
		}

		opTrans.LookAt (enemy.position + Vector3.up * 7f);
		for (float time = 0f; time < 5f; time += Time.deltaTime) yield return null;
	}

}

public partial class ElasticCameraOperator {

	private IEnumerator Salute() {
		cur.lerpSpeed = cur.scheme.lerpSpeed;

		Transform player = targets.Where (t => t.isPlayer).Select (t => t.trans).FirstOrDefault ();

		opTrans.position = player.position + player.forward * 2f;
		opTrans.SetPositionY (1f);
		opTrans.LookAt (player.position + Vector3.up * 1f);

		for (float time = 0f; time < 3f; time += Time.deltaTime) yield return null;
		yield return null;
	}

}

public partial class ElasticCameraOperator {

	private IEnumerator Dead() {
		Vector3 angs = opTrans.eulerAngles;
		cur.lerpSpeed = cur.scheme.lerpSpeed;

		Transform player = targets.Where (t => t.isPlayer).Select (t => t.trans).FirstOrDefault ();

		for (float time = 0f; time < 3f; time += Time.deltaTime) {
			Quaternion qy = Quaternion.AngleAxis (time * 10f, Vector3.up);

			opTrans.position = player.position.Ground(4f) + qy * player.forward * 1f;
			opTrans.LookAt (player.position);

			yield return null;
		}

		yield return null;
		opTrans.eulerAngles = angs;
	}

}
