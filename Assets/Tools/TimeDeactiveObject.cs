using UnityEngine;
using System.Collections;

public class TimeDeactiveObject : MonoBehaviour {
	public float time;

	void OnEnable() {
		StartCoroutine (Proc ());
	}

	IEnumerator Proc() {
		yield return new WaitForSeconds (time);
		gameObject.SetActive (false);
	}

}
