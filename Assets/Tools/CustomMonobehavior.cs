using UnityEngine;
using System.Collections;

namespace Custom {

	public class SingletonBase<T> where T : new() {
		private static T instance;
		public static T Instance {
			get {
				if (instance == null) {
					instance = new T ();
				}
				return instance;
			}
		}
	}

	public class SingletonMonoBehavior<T> : MonoBehaviour where T : MonoBehaviour {
		private static T instance;
		public static T Instance {
			get {
				if (instance == null) {
					instance = (T)FindObjectOfType(typeof(T));
					if (instance == null) {
						GameObject obj = new GameObject (typeof(T).ToString ());
						DontDestroyOnLoad (instance);
						instance = obj.AddComponent<T> ();
					}
				}
				return instance;
			}
		}
	}

	public class SingletonMonoBehaviorDestroyable<T> : MonoBehaviour where T : MonoBehaviour {
		private static T instance;
		public static T Instance {
			get {
				if (instance == null) {
					instance = GameObject.FindObjectOfType<T> ();
					if (instance == null) {
						GameObject obj = new GameObject (typeof(T).ToString ());
						instance = obj.AddComponent<T> ();
					}
				}
				return instance;
			}
		}

		void OnDestroy() {
			instance = null;
		}
	}

	public class SingletonMonoBehaviorWithoutCreate<T> : MonoBehaviour where T : MonoBehaviour {
		private static T instance;
		public static T Instance {
			get {
				if (instance == null) {
					instance = GameObject.FindObjectOfType<T> ();
				}
				return instance;
			}
		}

		void OnDestroy() {
			instance = null;
		}
	}

	public class CustomMonoBehavior : MonoBehaviour {
		private Transform transCache;
		public Transform myTrans {
			get {
				if (transCache == null) {
					transCache = transform;
				}
				return transCache;
			}
			set {
				transCache = value;
			}
		}
		public Vector3 myPosition { get { return myTrans.position; } }
		public Vector3 myAngls    { get { return myTrans.eulerAngles; } }
		public Vector3 myScale    { get { return myTrans.localScale; } }
		public Vector3 myForward  { get { return myTrans.forward; } }

		private IEnumerator exclusiveCoroutine;
		public void StartCoroutineExclusive(IEnumerator routine) {
			if (exclusiveCoroutine != null) {
				StopCoroutine (exclusiveCoroutine);
			}

			exclusiveCoroutine = routine;
			StartCoroutine (exclusiveCoroutine);
		}
	}

	public static class CustomMonoBehaviorExtension {
		public static void StartExclusiveBy(this IEnumerator enumerator, CustomMonoBehavior behav) {
			behav.StartCoroutineExclusive (enumerator);
		}
	}

}
