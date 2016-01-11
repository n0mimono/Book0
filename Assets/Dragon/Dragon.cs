using UnityEngine;
using System.Collections;
using Custom;
using System.Collections.Generic;
using System.Linq;

public class Dragon : MonoBehaviour {
	public Transform ctrlTrans;
	public Transform myTrans;
	public Animator animator;

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

	[Button("SetState", "Set Idle", State.Idle)] public float ButtonIdle;
	[Button("SetState", "Set FlyIdle", State.FlyIdle)] public float ButtonFlyIdle;
	[Button("SetState", "Set Fly", State.Fly)] public float ButtonFly;

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
			Debug.Log (cur.state + " > " + next.state + ": " + cur.pos + " > " + next.pos);
			cur = next;
		}
	}

	private void OnStateExit(int hash) {
	}

}
