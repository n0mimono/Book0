using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnchantRotater : MonoBehaviour {
	public Vector3 speeds;

	void Update() {
		transform.Rotate (speeds * Time.deltaTime);
	}

}
