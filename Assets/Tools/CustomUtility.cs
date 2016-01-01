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

		public static float ConstLerp(float value, float target, float delta) {
			float d = target - value;
			if (Mathf.Abs(d) < delta) {
				return target;
			} else {
				return value + Mathf.Sign (d) * delta;
			}
		}

		public static Vector3 ConstLerp(Vector3 value, Vector3 target, float delta) {
			return new Vector3 (
				ConstLerp(value.x, target.x, delta),
				ConstLerp(value.y, target.y, delta),
				ConstLerp(value.z, target.z, delta)
			);
		}

		public static Vector3 ToVector3(this Color color) {
			return new Vector3(color.r, color.g, color.b);
		}

		public static Color ToColor(this Vector3 vec, float alpha) {
			return new Color (vec.x, vec.y, vec.z, alpha);
		}

	}
}
