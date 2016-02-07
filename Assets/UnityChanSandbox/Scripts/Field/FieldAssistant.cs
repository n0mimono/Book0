using UnityEngine;
using System.Collections;

public class FieldAssistant : MonoBehaviour {

	public int id;

	public Transform movableTrans;
	public Transform anchorTrans;
	public float lerpSpeed;
	public bool forceOver;

	void Update() {
		if (FieldModel.Instance.IsFieldOver (id) || forceOver) {
			movableTrans.position = Vector3.Lerp (movableTrans.position, anchorTrans.position, lerpSpeed * Time.deltaTime);
		}
	}

}
