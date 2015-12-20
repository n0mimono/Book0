using UnityEngine;
using System.Collections;

namespace Custom {
	public static class Utility {

		public static Vector3 AngleLerp(Vector3 a, Vector3 b, float t) {
			return new Vector3 (Mathf.LerpAngle (a.x, b.x, t),
				Mathf.LerpAngle (a.y, b.y, t),
				Mathf.LerpAngle (a.z, b.z, t));
		}

	}
}
