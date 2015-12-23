using UnityEngine;
using System.Collections;

namespace Custom {
	public static class Utility {

		public static Vector3 AngleLerp(Vector3 a, Vector3 b, float t) {
			return new Vector3 (Mathf.LerpAngle (a.x, b.x, t),
				Mathf.LerpAngle (a.y, b.y, t),
				Mathf.LerpAngle (a.z, b.z, t));
		}

		public static void SetPositionY(this Transform trans, float y) {
			Vector3 pos = trans.position;
			trans.position = new Vector3 (pos.x, y, pos.z);
		}

		public static void AddEulerAngleY(this Transform trans, float y) {
			Vector3 ang = trans.eulerAngles;
			trans.eulerAngles = new Vector3 (ang.x, ang.y + y, ang.z);
		}
	}
}
