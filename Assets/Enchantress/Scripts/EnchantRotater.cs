using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnchantRotater : MonoBehaviour {
	public float scale;

	public Vector3 speeds;
	public List<EnchantTrailer> trailers;

	void Update() {
		transform.Rotate (speeds * Time.deltaTime);
		trailers.ForEach (t => t.scale = scale);
	}
}
