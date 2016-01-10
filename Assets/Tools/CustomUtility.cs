using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Custom {
	public static class Utility {

		public static Vector3 AngleLerp(Vector3 a, Vector3 b, float t) {
			return new Vector3 (Mathf.LerpAngle (a.x, b.x, t),
				Mathf.LerpAngle (a.y, b.y, t),
				Mathf.LerpAngle (a.z, b.z, t));
		}

		public static void SetEulerAngleX(this Transform trans, float x) {
			Vector3 ang = trans.eulerAngles;
			trans.eulerAngles = new Vector3 (x, ang.y, ang.z);
		}

		public static void SetPositionY(this Transform trans, float y) {
			Vector3 pos = trans.position;
			trans.position = new Vector3 (pos.x, y, pos.z);
		}

		public static void AddEulerAngleY(this Transform trans, float y) {
			Vector3 ang = trans.eulerAngles;
			trans.eulerAngles = new Vector3 (ang.x, ang.y + y, ang.z);
		}

		public static Vector3 Ground(this Vector3 pos, float height) {
			return new Vector3 (pos.x, height, pos.z);
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

		public static float Similarity(this Vector3 a, Vector3 b) {
			return Vector3.Dot (a.normalized, b.normalized);
		}

		public static Vector3 ToWorldVec(this Transform cameraTrans, Vector2 touchVec) {
			Vector3 screenVec = new Vector3 (touchVec.x, 0f, touchVec.y);
			Vector3 worldVec = Quaternion.AngleAxis (cameraTrans.eulerAngles.y, Vector3.up) * screenVec;
			return worldVec;
		}

		public static void SetColor(this Button button, Color color) {
			button.targetGraphic.color = color;
		}

		public static Vector3 GenerateRandom(this Vector3 scales) {
			Vector3 r = (new Vector3 (Random.value, Random.value, Random.value) * 2f - Vector3.one);
			return new Vector3(r.x * scales.x, r.y * scales.y, r.z * scales.z);
		}
	}

	public class InnerClass<TP> {
		private TP parent;
		public TP Parent { get { return parent; } }

		public void SetParent(TP parent) {
			this.parent = parent;
		}
	}

	public static class InnerClassExtension {
		
		public static T InnerCreate<T, TP>(this TP parent) where T : InnerClass<TP>, new() {
			T obj = new T (); //InnerClassExtension.Construct<T> ();
			obj.SetParent (parent);
			return obj;
		}

	}

}
