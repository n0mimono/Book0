using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Custom;

public partial class YukataChan : MonoBehaviour {
	[Header("Yukata Android")]
	public YukataAction yukataAction;
	protected Transform myTrans;
	
	[Header("Yukata-Chan")]
	public float walkingSpeed;
	public float runningSpeed;
	public SkinMeshExploder exploder;

	public enum Type {
		Walker,
		Chaser,
	}
	public Type type;
	public bool Is(Type type) { return this.type == type; }

	public enum State {
		Noop,
		Search,
		Chase,
		Attack
	}

	private class Status {
		public State state;
		public float walkSpeed;
		public float turnSpeed;

		public Transform tgtTrans;
		public Transform aimTrans;

		public bool isUpdateCommon;

		public bool Is(State state) {
			return this.state == state;
		}
	}
	private Status cur = new Status();

	private bool IsTargetive { get { return cur.tgtTrans != null; } }

	void Start() {
		// event initilize
		myTrans = yukataAction.transform;
		yukataAction.InLockAction += InLockInterrupt;
		yukataAction.OutLockAction += OutLockInterrupt;
		yukataAction.DeadHandler += OnDead;

		// others
		CreateAim ();
	}

	void OnEnable() {
		cur.isUpdateCommon = true;

		// start update
		UpdateCommon ().When (() => cur.isUpdateCommon).StartBy (this);
		NextState (State.Search);

		// misc
		UpdateTarget().StartBy(this);

		// force revive
		yukataAction.SpawningRevive();
	}

	private void InLockInterrupt(YukataAction.AnimeAction act) {
		if (act == YukataAction.AnimeAction.Damaged) {
			NextState (State.Noop);
		}
		cur.isUpdateCommon = false;
	}
	private void OutLockInterrupt(YukataAction.AnimeAction act) {
		if (act == YukataAction.AnimeAction.Damaged) {
			NextState (State.Search);
		}
		cur.isUpdateCommon = true;
	}

	private void CreateAim() {
		GameObject obj = new GameObject ("_Aim_" + myTrans.name);
		cur.aimTrans = obj.transform;
		cur.aimTrans.SetParent (GameObject.Find ("Misc").transform);
	}

	private void NextState(State next) {
		cur.state = next;
		StartCoroutine (next.ToString());
	}

	private IEnumerator UpdateCommon() {
		yield return null;

		while (true) {
			Vector3 tgtDir = (cur.aimTrans.position - myTrans.position).normalized;

			// direction
			if (!IsStraightToTarget(0.99f)) {
				myTrans.forward = Vector3.Slerp (myTrans.forward, tgtDir, cur.turnSpeed * Time.deltaTime);
			}

			// position
			yukataAction.Move (myTrans.forward * cur.walkSpeed);

			yield return null;
		}
	}

	private IEnumerator UpdateTarget() {
		yield return null;

		while (true) {

			if (cur.tgtTrans == null) {
				GameObject tgtObj = gameObject.FindOppositeCharacters ().RandomOrDefault ();
				if (tgtObj != null) {
					cur.tgtTrans = tgtObj.transform;
				}
			}

			yield return new WaitForSeconds(1f);
		}
	}

	public bool IsStraightToTarget(float threshold) {
		Vector3 tgtDir = (cur.aimTrans.position - myTrans.position).normalized;
		return myTrans.forward.Similarity (tgtDir) > threshold;
	}

	public bool IsNearTarget(float distance) {
		return Vector3.Distance(myTrans.position, cur.aimTrans.position) < distance;
	}

}

public partial class YukataChan {
	
	public void OnDead() {
		exploder.Explosion ();
	}

	[Button("OnDead", "Force Kill")] public int ButtonKill;
}

public partial class YukataChan {

	private IEnumerator Search() {
		if (Is (Type.Walker)) {
			return UpdateSearch ().While(() => cur.Is (State.Search));
		} else {
			return UpdateSearch ().Add(TransSearch()).While(() => cur.Is (State.Search));
		}
	}

	private IEnumerator UpdateSearch() {
		cur.turnSpeed = 1f;

		while (true) {
			yield return null;

			float r = Random.value * 10f + 10f;
			float t = (2f * Random.value - 1f) * 90f;
			Vector3 m = Quaternion.AngleAxis (t, Vector3.up) * myTrans.forward;
			cur.aimTrans.position = myTrans.position + m * r;
			yield return new WaitForSeconds (2f);

			cur.walkSpeed = walkingSpeed;
			yield return new WaitForSeconds (5f);

			cur.walkSpeed = 0.1f;
			yield return new WaitForSeconds (2f);
		}

	}

	private IEnumerator TransSearch() {
		while (true) {
			yield return null;

			if (IsTargetive && IsStraightToTarget(0.8f) && IsNearTarget(15f)) {
				NextState (State.Chase);
			}
		}
	}

	private IEnumerator Noop() {
		cur.turnSpeed = 0f;
		cur.walkSpeed = 0f;

		yield return null;
	}

	private IEnumerator Chase() {
		return UpdateChase ().Add (TransChase ())
			.While (() => cur.Is (State.Chase));
	}

	private IEnumerator UpdateChase() {
		cur.turnSpeed = 10f;

		cur.walkSpeed = runningSpeed;
		while (true) {
			cur.aimTrans.position = cur.tgtTrans.position;
			yield return null;
		}
	}

	private IEnumerator TransChase() {
		while (true) {
			yield return null;

			if (IsTargetive && IsStraightToTarget (0.95f) && IsNearTarget (5f)) {
				NextState (State.Attack);
			} else if (!IsTargetive || (!IsStraightToTarget (0f) && !IsNearTarget (20f))) {
				NextState (State.Search);
			}
		}

	}

	private IEnumerator Attack() {
		return UpdateAttack ().Continue (TransAttack ())
			.While(() => cur.Is (State.Attack));
	}

	private IEnumerator UpdateAttack() {
		cur.turnSpeed = 0.1f;

		yield return null;
		bool isFin = false;
		yukataAction.Dive (myTrans.forward, (act) => isFin = true);

		while (!isFin) {
			yield return null;
		}
	}

	private IEnumerator TransAttack() {
		yield return null;
		NextState (State.Chase);
	}

}
