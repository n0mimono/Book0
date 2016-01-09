using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnchantTrailer : MonoBehaviour {
	public float scale;
	public float angle;
	public Vector3 offset;

	void Update() {
		transform.localPosition = Quaternion.AngleAxis(angle, Vector3.up) * (offset * scale);
	}

}
