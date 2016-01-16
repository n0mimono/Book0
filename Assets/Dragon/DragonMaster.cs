using UnityEngine;
using System.Collections;
using Custom;
using System.Collections.Generic;
using System.Linq;
using System;

public partial class DragonMaster : MonoBehaviour {
	public Dragon dragon;
	public List<Transform> dragonChairs;
	public Transform focusCenter;
	public float tgtAngleSpeed;

	public bool isPlayOnStart;

	public enum State {
		None = 0,
		Idle = 1,
		Move = 2,
		Fire = 3,
	}
	public State state;

	void Start() {
		state = State.None;

		if (isPlayOnStart) {
			ChooseProc ().StartBy (this);
		}
	}

	void Update() {
		dragon.SetAngleSpeed (tgtAngleSpeed);
	}

	public void SetDragonState(Dragon.State state) {
		dragon.SetState (state);
	}

	private IEnumerator RotateAlign(Transform target) {
		Transform drans = dragon.transform;

		while (true) {
			tgtAngleSpeed = Utility.Angle (drans.forward, target.forward);
			yield return null;
		}
	}

	private IEnumerator RotateTo(Transform target) {

		while (true) {
			tgtAngleSpeed = dragon.GetAngleSpeed (target.position);
			yield return null;
		}
	}

}

public partial class DragonMaster {

	private IEnumerator ChooseProc() {
		List<Action> procs = new List<Action> () {
			() => ProcIdle().StartBy(this),
			Move,
			Fire
		};
		while (true) {
			if (state == State.None) {
				procs.RandomOrDefault () ();
			}

			yield return new WaitForSeconds (1f);
		}
	}

	private IEnumerator ProcIdle() {
		state = State.Idle;
		yield return new WaitForSeconds (10f);
		state = State.None;
	}

	public void Move() {
		GoTo (dragonChairs.RandomOrDefault ());
	}

	public void GoTo(Transform chair) {
		ProcGoTo (chair).StartBy (this);
	}

	private IEnumerator ProcGoTo(Transform chair) {
		state = State.Move;

		SetDragonState (Dragon.State.Idle);
		while (!dragon.Is(Dragon.State.Idle)) yield return null;

		SetDragonState (Dragon.State.FlyIdle);
		while (!dragon.Is(Dragon.State.FlyIdle)) yield return null;

		SetDragonState (Dragon.State.Fly);
		while (!dragon.Is(Dragon.State.Fly)) yield return null;

		System.Func<bool> dstPredicate = () =>
			Vector3.Distance (dragon.transform.position.Ground (), chair.position.Ground ()) > 5f;
		Task dst = RotateTo (chair).While (dstPredicate);
		yield return StartCoroutine (dst);

		SetDragonState (Dragon.State.FlyIdle);
		RotateTo (focusCenter).StartBy (this);

		while (!dragon.Is(Dragon.State.FlyIdle)) yield return null;
		while (Mathf.Abs (tgtAngleSpeed) > 1f) yield return null;
		tgtAngleSpeed = 0f;

		yield return null;
		SetDragonState (Dragon.State.Idle);

		state = State.None;
	}

	public void Fire() {
		GameObject enemy = gameObject.FindOppositeCharacters ().FirstOrDefault ();

		ProcFire (enemy.transform).StartBy (this);
	}

	private IEnumerator ProcFire(Transform target) {
		state = State.Fire;

		SetDragonState (Dragon.State.Idle);
		while (!dragon.Is(Dragon.State.Idle)) yield return null;

		SetDragonState (Dragon.State.FlyIdle);
		while (!dragon.Is(Dragon.State.FlyIdle)) yield return null;

		dragon.target = target;
		dragon.StartBreathe (true);
		RotateTo (dragon.target).While (() => dragon.target != null).StartBy (this);
		yield return new WaitForSeconds (10f);

		dragon.target = null;
		dragon.StopBreathe ();

		yield return null;
		SetDragonState (Dragon.State.Idle);

		state = State.None;
	}

}

public partial class DragonMaster {
	[Button("SetDragonState", "Set Idle", Dragon.State.Idle)] public float ButtonIdle;
	[Button("SetDragonState", "Set FlyIdle", Dragon.State.FlyIdle)] public float ButtonFlyIdle;
	[Button("SetDragonState", "Set Fly", Dragon.State.Fly)] public float ButtonFly;
	[Button("StartFire", "Start Fire")] public float ButtonFireStart;
	[Button("StopFire", "Stop Fire")] public float ButtonFireStop;

	[Button("GoToBy", "Goto 0", 0)] public float ButtonGoTo0;
	[Button("GoToBy", "Goto 1", 1)] public float ButtonGoTo1;
	[Button("GoToBy", "Goto 2", 2)] public float ButtonGoTo2;
	[Button("GoToBy", "Goto 3", 3)] public float ButtonGoTo3;
	[Button("GoToBy", "Goto 4", 4)] public float ButtonGoTo4;

	[Button("Move", "Move")] public float ButtonMove;
	[Button("Fire", "Fire")] public float ButtonFire;

	public void StartFire() {
		GameObject enemy = gameObject.FindOppositeCharacters ().FirstOrDefault ();

		dragon.target = enemy.transform;
		dragon.StartBreathe (true);

		RotateTo (dragon.target).While (() => dragon.target != null).StartBy (this);
	}

	public void StopFire() {
		dragon.target = null;
		dragon.StopBreathe ();
	}

	public void GoToBy(int index) {
		GoTo (dragonChairs.ElementAtOrDefault (index));
	}

	[Button("Roar", "Roar")] public float ButtonRoar;

	public void Roar() {
		SubProcRoar ().StartBy (this);
	}

	private IEnumerator SubProcRoar() {
		yield return null;
		dragon.StartBreathe (false);

		yield return new WaitForSeconds (1f);
		dragon.StopBreathe ();

		yield return null;
	}

}
