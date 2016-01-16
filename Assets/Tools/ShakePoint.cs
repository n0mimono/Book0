using UnityEngine;
using System.Collections;

public class ShakePoint : MonoBehaviour {
	public Shaker shaker;

	public Vector3 position;

	void Start() {
		position = transform.position;
	}

	void Update() {
		transform.position = position + shaker.offset;
	}

}
