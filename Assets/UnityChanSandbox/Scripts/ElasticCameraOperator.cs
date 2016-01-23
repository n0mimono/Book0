using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Custom;

public partial class ElasticCameraOperator : MonoBehaviour {

	[Header("Common Settings")]
	public Transform opTrans;

	public enum Mode {
		Forwarding       =  0,
		LookupForwarding =  1,
		PlayerTargeting  =  2,
		MultiTargeting   =  3,
		Magi             = 11,
		Salute           = 13
	}

	[System.Serializable]
	public class Scheme {
		public Mode mode;
		public float lerpSpeed;

		public System.Func<IEnumerator> routiner;
	}
	public List<Scheme> schemes;

	private class Status : InnerClass<ElasticCameraOperator> {
		public IEnumerator routine;
		public float lerpSpeed;
		public Mode  mode;
		public Mode  baseMode;

		public Scheme scheme { get { return Parent.GetScheme(mode); } }
	}
	private Status cur;

	public delegate void ModeChangeHandler (Mode mode);
	public event ModeChangeHandler OnModeChanged;

	void Awake() {
		OnModeChanged += (mode) => {};
	}

	void Start() {
		Initilize ();
	}

	void LateUpdate() {
		if (cur.routine != null) {
			cur.routine.MoveNext ();
		}
		CommonUpdate ();
	}

	private void Initilize() {
		cur = this.InnerCreate<Status, ElasticCameraOperator> ();

		GetScheme (Mode.Forwarding).routiner = () => UpdateRoutine(ForwardingUpdate);
		GetScheme (Mode.LookupForwarding).routiner = () => UpdateRoutine(ForwardingUpdate); // dummy
		GetScheme (Mode.PlayerTargeting).routiner = () => UpdateRoutine(PlayerTargettingUpdate); // dummy
		GetScheme (Mode.MultiTargeting).routiner = () => UpdateRoutine(MultiTargettingUpdate);
		GetScheme (Mode.Magi).routiner = () => TemporaryRoutine(Magi);
		GetScheme (Mode.Salute).routiner = () => TemporaryRoutine(Salute);
	}

	public Scheme GetScheme(Mode mode) {
		return schemes.Where (s => s.mode == mode).FirstOrDefault ();
	}

	private void CommonUpdate() {
		Vector3 pos = opTrans.position;
		Vector3 ang = opTrans.eulerAngles;

		Transform camTrans = Camera.main.transform;
		camTrans.eulerAngles = Custom.Utility.AngleLerp (camTrans.eulerAngles, ang, cur.lerpSpeed * Time.deltaTime);
		camTrans.position = Vector3.Lerp(camTrans.position, pos, cur.lerpSpeed * Time.deltaTime) + Shaker.Instance.offset;
	}

	void StartLateCoroutine(IEnumerator routine) {
		cur.routine = routine;
	}

	private IEnumerator UpdateRoutine(System.Action update) {
		while (true) {
			update ();
			yield return null;
		}
	}

	private IEnumerator TemporaryRoutine(System.Func<IEnumerator> routine) {		
		IEnumerator iter = routine ();
		while (iter.MoveNext ()) {
			yield return null;
		}
		SetMode (cur.baseMode);
	}

	public void SetMode(Mode mode, bool isAlsoSetBaseMode = false) {
		if (isAlsoSetBaseMode) {
			cur.baseMode = mode;
		}
		cur.mode = mode;

		StartLateCoroutine (cur.scheme.routiner ());

		OnModeChanged (cur.mode);
	}

}
