using UnityEngine;
using System.Collections;
using Custom;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

public partial class Dragon : Creature {
	[Header("Dragon")]
	public Transform ctrlTrans;
	public Transform myTrans;
	public Animator animator;

	public Transform breathePoint;
	public MagicCannon cannon;
	public Transform target;

	public enum State {
		Idle     =  0,
		FlyIdle  =  1,
		Fly      =  2,
		Dead     = 20,
	}

	[System.Serializable]
	public class StateScheme {
		public State state;
		public string path;
		public Vector3 pos;
		public Vector3 fwd;
	}
	public List<StateScheme> schemes;
	public StateScheme cur;

	public float maxAngleSpeed;
	private float angleSpeed;

	protected override void Initialize() {
		base.Initialize ();

		SetState (State.Idle);

		animator.GetBehaviours<DragonStateMachine> ().ToList ()
			.ForEach (d => {
			d.OnEnter = OnStateEnter;
			d.OnExit = OnStateExit;
		});

		UpdateByScheme ().StartBy (this);
		Rotate ().StartBy (this);

		InitilizeSpell ();
	}

	public void SetState(State state) {
		animator.SetInteger ("State", (int)state);
	}

	IEnumerator UpdateByScheme() {
		while (true) {
			ctrlTrans.localPosition = Vector3.Lerp (ctrlTrans.localPosition, cur.pos, 2f * Time.deltaTime);
			ctrlTrans.forward = Vector3.Slerp (ctrlTrans.forward, cur.fwd, 2f * Time.deltaTime);

			yield return null;
		}
	}

	private void OnStateEnter(int hash) {
		StateScheme next = schemes.Where (s => Animator.StringToHash(s.path) == hash).FirstOrDefault ();
		if (next != null) {
			cur = next;
		}
	}

	private void OnStateExit(int hash) {
	}

	public void SetAngleSpeed(float angleSpeed) {
		this.angleSpeed = Mathf.Clamp (angleSpeed, -maxAngleSpeed, maxAngleSpeed);
	}

	private IEnumerator Rotate() {
		while (true) {
			transform.Rotate (Vector3.up, angleSpeed * Time.deltaTime);
			yield return null;
		}
	}

	public float GetAngleSpeed(Vector3 targetPosition) {
		Vector3 tgtFwd = (targetPosition.Ground() - transform.position.Ground()).normalized;
		return Utility.Angle (transform.forward, tgtFwd);
	}

	public bool Is(State state) {
		return cur.state == state;
	}

}

public partial class Dragon {

	public enum HeadState {
		None    = 0,
		Breathe = 1,
		Dead    = 20,
	}
	private bool isBreathing;

	public void InitilizeSpell() {
		cannon.Initilize (gameObject.tag);
	}

	public void StartSpell() {

		if (isBreathing) {
			ProcSpell ().StartBy (this);
		} else {
			// Roar only
			Shaker.Shake(Shaker.Type.High, transform.position);
		}
	}

	public void EndSpell() {
	}

	IEnumerator ProcSpell() {
		cannon.Load ();
		yield return new WaitForSeconds (0.5f);

		cannon.Fire (target);
		yield return null;
	}

	public void StartBreathe(bool isActiveBreath) {
		isBreathing = isActiveBreath;
		animator.SetInteger ("Head", (int)HeadState.Breathe);
	}

	public void StopBreathe() {
		isBreathing = false;
		animator.SetInteger ("Head", (int)HeadState.None);
	}

}

public partial class Dragon {


	protected override void InitializeDamageControl() {
		base.InitializeDamageControl ();

		DeadHandler += OnDead;
	}

	protected override void OnDamage(DamageSource src) {
		base.OnDamage (src);
	}

	private void OnDead() {

		cur = schemes.Where (s => s.state == State.Dead).FirstOrDefault ();
		animator.SetTrigger ("Kill");
	}

}
