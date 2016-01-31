using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Custom;

public class YukataSanSpecter : CustomMonoBehavior {
	[Header("Yukata Android")]
	public YukataAction yukataAction;

	[Header("Yukata-Chan")]
	public Transform target;
	public float turnSpeed;
	public float foundSpeed;

	[Header("UI")]
	public PixieGauge gauge;

	public float targetSpeed;
	private bool IsFound { get { return targetSpeed > foundSpeed; } }

	IEnumerator Start() {
		yukataAction.DeadHandler += () => enabled = false;
		yield return new WaitWhile (() => yukataAction.target == null);
		target = yukataAction.target;

		StartCheck ();
	}

	private void StartCheck() {
		CheckWait ()
			.While (() => !IsFound)
			.OnCompleted (StartAttack)
			.StartExclusiveBy (this);
	}

	private void StartAttack() {
		targetSpeed = 0f;
		Attack ()
			.OnCompleted (StartCheck)
			.StartExclusiveBy (this);
	}

	private IEnumerator CheckWait() {
		while (true) {
			gauge.Do (g => g.SetNumber (-1));
			for (int i = 0; i < 10; i++) {
				yield return new WaitForSeconds (GetTime());
				gauge.Do (g => g.SetNumber (i));
			}
			yield return StartCoroutine (CheckTarget ().WhileInCount (GetWaitTime()));
		}
	}

	float GetTime() {
		float dist = Vector3.Distance (myPosition, target.position);
		if (dist >= 240) {
			float d = (dist - 150f) / 100f;
			return d + 0.5f;
		} else if (dist >= 170f) {
			float d = (dist - 150f) / 100f;
			return d + 0.5f + 1f.RandomFiltered (0.2f);
		} else if (dist >= 100f) {
			return 0.2f + 0.5f.RandomFiltered (0.2f);
		} else {
			return 0.1f + 0.5f * Random.value.RandomFiltered (0.5f);
		}
	}

	float GetWaitTime() {
		float dist = Vector3.Distance (myPosition, target.position);
		if (dist >= 150f) {
			return 3f;
		} else if (dist >= 150f) {
			return 2f + Random.value * 2f;
		} else {
			return 1f + Random.value * 3f;
		}

	}

	private IEnumerator CheckTarget() {

		Vector3 curPos = target.position;
		while (true) {
			Vector3 prevPos = curPos;
			curPos = target.position;
			myTrans.LookAt (curPos);

			targetSpeed = Vector3.Distance (prevPos, curPos) / Time.deltaTime;
			if (IsFound) break; // immedeately finding, umm..
			yield return null;
		}
	}

	private IEnumerator Attack() {
		yield return null;
		gauge.Do (g => g.SetFullNumber());
		gauge.Do (g => g.SetCrisis (true));
		yield return null;

		yukataAction.enchantress.Hold ();
		yield return new WaitForSeconds (1f);
		yukataAction.enchantress.LoadAll ();
		yield return new WaitForSeconds (2f);
		yukataAction.enchantress.Fire (EnchantControl.TargetMode.Multi);
		yield return new WaitForSeconds (5f);
		yukataAction.enchantress.Release ();
		yield return null;

		gauge.Do (g => g.SetCrisis (false));
		yield return new WaitForSeconds (1f);
	}

}
