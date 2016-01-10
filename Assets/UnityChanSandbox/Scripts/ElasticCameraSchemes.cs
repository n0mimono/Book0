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

}

public partial class ElasticCameraOperator {

	[System.Serializable]
	public class ForwardingParams {
		public float height;
		public float back;
		public float forward;
	}
	[Header("Forwarding Mode Details")]
	public ForwardingParams forwardingParams;

	private void ForwardingUpdate() {
		float height = forwardingParams.height;
		float back = forwardingParams.back;
		float forward = forwardingParams.forward;
		cur.lerpSpeed = cur.scheme.lerpSpeed;

		Vector3 pos = Player.trans.position + Player.trans.forward * back;
		opTrans.position = pos.Ground (height);

		Vector3 fwd = Player.trans.position + Player.trans.forward * forward;
		opTrans.LookAt (fwd);
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

		Vector3 pos = Player.trans.position + transform.forward * back;
		opTrans.position = pos.Ground (height);
	}
}

public partial class ElasticCameraOperator {

	[System.Serializable]
	public class MultiTargetingParams {
		public float height;
	}
	[Header("Multi-Targeting Mode Details")]
	public MultiTargetingParams mTargettingParams;

	private void MultiTargettingUpdate() {
		float height = mTargettingParams.height;
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

public partial class ElasticCameraOperator {

	private IEnumerator Magi() {
		cur.lerpSpeed = cur.scheme.lerpSpeed;

		Transform player = targets.Where (t => t.isPlayer).Select (t => t.trans).FirstOrDefault ();
		Transform enemy = targets.Where (t => t.isEnemy).Select (t => t.trans).FirstOrDefault ();

		transform.LookAt (player.position + Vector3.up * 1f);
		transform.position = player.position - transform.forward * 2f;
		transform.SetPositionY (1.5f);

		for (float time = 0f; time < 2f; time += Time.deltaTime) yield return null;
		yield return null;

		transform.LookAt (enemy.position + Vector3.up * 1f);
		for (float time = 0f; time < 5f; time += Time.deltaTime) {
			transform.AddEulerAngleY (Time.deltaTime * 15f);
			transform.position = enemy.position - transform.forward * (4f + time * 1.5f);
			yield return null;
		}

		transform.LookAt (enemy.position + Vector3.up * 7f);
		for (float time = 0f; time < 5f; time += Time.deltaTime) yield return null;
	}

}

public partial class ElasticCameraOperator {

	private IEnumerator Salute() {
		cur.lerpSpeed = cur.scheme.lerpSpeed;

		Transform player = targets.Where (t => t.isPlayer).Select (t => t.trans).FirstOrDefault ();

		transform.position = player.position + player.forward * 2f;
		transform.SetPositionY (1f);
		transform.LookAt (player.position + Vector3.up * 1f);

		for (float time = 0f; time < 3f; time += Time.deltaTime) yield return null;
		yield return null;
	}

}
