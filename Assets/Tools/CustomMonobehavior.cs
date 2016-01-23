using UnityEngine;
using System.Collections;

namespace Custom {

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

	public class SingletonMonoBehaviorDestroyable<T> : MonoBehaviour where T : MonoBehaviour{
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
	}

	public class SingletonMonoBehaviorWithoutCreate<T> : MonoBehaviour where T : MonoBehaviour{
		private static T instance;
		public static T Instance {
			get {
				if (instance == null) {
					instance = GameObject.FindObjectOfType<T> ();
				}
				return instance;
			}
		}
	}
}
