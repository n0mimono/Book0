using UnityEngine;
using System.Collections;
using Custom;
using System.Collections.Generic;
using System.Linq;

public class DragonMaster : MonoBehaviour {
	public Dragon dragon;
	public Transform dragonChair;
	public float tgtAngleSpeed;

	[Button("SetState", "Set Idle", Dragon.State.Idle)] public float ButtonIdle;
	[Button("SetState", "Set FlyIdle", Dragon.State.FlyIdle)] public float ButtonFlyIdle;
	[Button("SetState", "Set Fly", Dragon.State.Fly)] public float ButtonFly;
	[Button("StartEightCurve", "Start Fly")] public float ButtonEightCurve;
	[Button("StartFire", "Start Fire")] public float ButtonFire;

	void Start() {
		//StartEightCurve ();
	}

	void Update() {
		dragon.SetAngleSpeed (tgtAngleSpeed);
	}

	public void SetState(Dragon.State state) {
		dragon.SetState (state);
	}

	public void StartEightCurve() {
		EightCurve ().StartBy (this);
	}

	private IEnumerator EightCurve() {
		SetState (Dragon.State.Idle);
		while (!dragon.Is(Dragon.State.Idle)) yield return null;

		SetState (Dragon.State.FlyIdle);
		while (!dragon.Is(Dragon.State.FlyIdle)) yield return null;

		SetState (Dragon.State.Fly);
		while (!dragon.Is(Dragon.State.Fly)) yield return null;

		tgtAngleSpeed = 0f;
		yield return new WaitForSeconds (6f);

		tgtAngleSpeed = 25f;
		yield return new WaitForSeconds (10f);

		tgtAngleSpeed = 0f;
		yield return new WaitForSeconds (10f);

		tgtAngleSpeed = -15f;
		yield return new WaitForSeconds (10f);

		System.Func<bool> dstPredicate = () =>
			Vector3.Distance (dragon.transform.position.Ground (), dragonChair.position.Ground ()) > 5f;
		Task dst = RotateTo (dragonChair).While (dstPredicate);
		yield return StartCoroutine (dst);

		SetState (Dragon.State.FlyIdle);
		while (!dragon.Is(Dragon.State.FlyIdle)) yield return null;

		yield return StartCoroutine (RotateAlign (dragonChair).While (() => Mathf.Abs (tgtAngleSpeed) > 1f));
		tgtAngleSpeed = 0f;

		yield return null;
		SetState (Dragon.State.Idle);
	}

	private IEnumerator RotateAlign(Transform target) {
		Transform drans = dragon.transform;

		while (true) {
			tgtAngleSpeed = Utility.Angle (drans.forward, target.forward);
			yield return null;
		}
	}

	private IEnumerator RotateTo(Transform target) {
		Transform drans = dragon.transform;

		while (true) {
			Vector3 tgtFwd = (target.position.Ground() - drans.position.Ground()).normalized;
			tgtAngleSpeed = Utility.Angle (drans.forward, tgtFwd);

			yield return null;
		}
	}

	public void StartFire() {
		dragon.StartBreathe ();
	}

}
