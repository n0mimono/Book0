using UnityEngine;
using System.Collections;

public class FieldDirector : MonoBehaviour {
	public int id;

	public Transform movableTrans;
	public Transform anchorTrans;
	public float lerpSpeed;

	void Update() {
		if (FieldModel.Instance.IsFieldOver (id)) {
			movableTrans.position = Vector3.Lerp (movableTrans.position, anchorTrans.position, lerpSpeed);
		}
	}

	public void OnFieldOver() {
		FieldModel.Instance.SetFieldOver (id, true);
	}

}
