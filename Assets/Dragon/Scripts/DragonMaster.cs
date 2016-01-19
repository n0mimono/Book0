using UnityEngine;
using System.Collections;
using Custom;
using System.Collections.Generic;
using System.Linq;
using System;

public partial class DragonMaster : MonoBehaviour {
	public Dragon dragon;
	public EnchantControl spellBullet;
	public EnchantControl meteorSwarm;

	public List<Transform> dragonChairs;
	public Transform focusCenter;
	public float tgtAngleSpeed;

	public bool isPlayOnStart;
	public bool isIdlingOnly;

	public enum State {
		None  = 0,
		Idle  = 1,
		Move  = 2,
		Fire  = 3,
		Spell = 4,
		Meter = 5,
	}
	public State state;

	void Start() {
		state = State.None;

		if (isPlayOnStart) {
			spellBullet.Do (e => e.Initilize (gameObject.tag));
			meteorSwarm.Do (e => e.Initilize (gameObject.tag));

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
			Fire,
			SpellBullet,
			MeteorSwarm
		};
		while (true) {
			yield return new WaitForSeconds (1f);
			if (state == State.None) {
				if (isIdlingOnly) {
					procs.FirstOrDefault () ();
				} else {
					procs.RandomOrDefault () ();
				}
			}
		}
	}

	private IEnumerator ProcIdle() {
		state = State.Idle;
		yield return new WaitForSeconds (3f);

		yield return StartCoroutine (SubProcRoar ());

		yield return new WaitForSeconds (3f);
		state = State.None;
	}

	public void Move() {
		Transform nearest = dragonChairs.WhichMin (c => Vector3.Distance (transform.position, c.position));
		Transform next = dragonChairs.Where (c => c != nearest).RandomOrDefault ();

		GoTo (next);
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

	private IEnumerator SubProcRoar() {
		yield return null;
		dragon.StartBreathe (false);

		yield return new WaitForSeconds (1f);
		dragon.StopBreathe ();

		yield return new WaitForSeconds (3f);
	}

	public void SpellBullet() {
		if (spellBullet == null) return;

		GameObject enemy = gameObject.FindOppositeCharacters ().FirstOrDefault ();
		ProcSpellBullet (enemy.transform).StartBy (this);
	}

	private IEnumerator ProcSpellBullet(Transform target) {
		state = State.Fire;

		SetDragonState (Dragon.State.Idle);
		while (!dragon.Is(Dragon.State.Idle)) yield return null;

		SetDragonState (Dragon.State.FlyIdle);
		while (!dragon.Is(Dragon.State.FlyIdle)) yield return null;

		// set position and angles
		Transform draTrans = dragon.transform;
		Vector3 basePos = draTrans.position.Ground();
		Vector3 baseDir = draTrans.forward.Ground ().normalized;
		Vector3 spellPos = spellBullet.transform.position;
		spellBullet.transform.position = (basePos + baseDir * spellPos.z).Ground (spellPos.y);
		spellBullet.transform.SetEulerAngleY (draTrans.eulerAngles.y);
		yield return null;

		spellBullet.Hold ();
		yield return new WaitForSeconds (1f);

		yield return StartCoroutine (spellBullet.LoadSequential (0.1f));
		yield return new WaitForSeconds (1f);

		yield return StartCoroutine (SubProcRoar ());
		yield return new WaitForSeconds (1.5f);
		spellBullet.SetTarget (target);
		spellBullet.Fire (EnchantControl.TargetMode.Single);

		yield return new WaitForSeconds (3f);
		spellBullet.Release ();
		yield return new WaitForSeconds (2f);

		yield return null;
		SetDragonState (Dragon.State.Idle);

		state = State.None;
	}

	public void MeteorSwarm() {
		if (meteorSwarm == null) return;

		ProcMeteorSwarm ().StartBy (this);
	}

	private IEnumerator ProcMeteorSwarm() {
		state = State.Fire;

		SetDragonState (Dragon.State.Idle);
		while (!dragon.Is(Dragon.State.Idle)) yield return null;

		SetDragonState (Dragon.State.FlyIdle);
		while (!dragon.Is(Dragon.State.FlyIdle)) yield return null;

		meteorSwarm.Hold ();
		yield return new WaitForSeconds (1f);

		yield return StartCoroutine (SubProcRoar ());
		yield return new WaitForSeconds (1.5f);

		bool keep = true;
		DragonMeteor ().While (() => keep).StartBy (this);
		yield return new WaitForSeconds (10f);
		keep = false;

		yield return StartCoroutine (SubProcRoar ());
		yield return new WaitForSeconds (1.5f);

		meteorSwarm.Release ();
		yield return new WaitForSeconds (2f);

		state = State.None;
	}

	private IEnumerator DragonMeteor() {
		while (true) {
			meteorSwarm.RandomLoad ();
			yield return null;

			meteorSwarm.Fire (EnchantControl.TargetMode.Single);
			yield return new WaitForSeconds (0.1f);
		}
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
	[Button("SpellBullet", "Spell Bullet")] public float ButtonSpellBullet;
	[Button("MeteorSwarm", "Meteor Swarm")] public float ButtonMeteorSwarm;

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

}
