using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Custom;

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
