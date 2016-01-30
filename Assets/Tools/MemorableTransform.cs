using UnityEngine;
using System.Collections;

public class MemorableTransform : MonoBehaviour {

	public Vector3 initPosition;
	public Vector3 initLoalPosition;
	public Vector3 initAngles;
	public Vector3 initLocalAngles;

	void Awake() {
		Transform trans = transform;
		initPosition = trans.position;
		initLoalPosition = trans.localPosition;
		initAngles = trans.eulerAngles;
		initLocalAngles = trans.localEulerAngles;
	}

	public void InitilizeLocal() {
		Transform trans = transform;
		trans.localPosition = initLoalPosition;
		trans.localEulerAngles = initLocalAngles;
	}

}
