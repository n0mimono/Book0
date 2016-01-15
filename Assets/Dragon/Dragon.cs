using UnityEngine;
using System.Collections;
using Custom;
using System.Collections.Generic;
using System.Linq;

public partial class Dragon : MonoBehaviour {
	public Transform ctrlTrans;
	public Transform myTrans;
	public Animator animator;

	public Transform breathePoint;
	public MagicCannon cannon;

	public enum State {
		Idle     = 0,
		FlyIdle  = 1,
		Fly      = 2,
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

	void Start() {
		Initilize ();
	}

	private void Initilize() {
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

	public bool Is(State state) {
		return cur.state == state;
	}

}

public partial class Dragon {

	public enum HeadState {
		None    = 0,
		Breathe = 1,
	}

	public void InitilizeSpell() {
		cannon.Initilize (gameObject.tag);
	}

	public void StartSpell() {
	}

	public void EndSpell() {
	}

	public void StartBreathe() {
		animator.SetInteger ("Head", (int)HeadState.Breathe);
	}

	public void StopBreathe() {
		animator.SetInteger ("Head", (int)HeadState.None);
	}

}
