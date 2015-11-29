using UnityEngine;
using System.Collections;

public class FoolRotate : MonoBehaviour {

	public Vector3 speed;

	void Update () {
		Vector3 angs = transform.localEulerAngles;

		angs += Time.deltaTime * speed;

		transform.localEulerAngles = angs;
	}

}
