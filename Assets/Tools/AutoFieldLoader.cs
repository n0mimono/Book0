using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class AutoFieldLoader : MonoBehaviour {
	public string sceneName;
	public bool isSetActive;

	IEnumerator Start() {
		bool isAlreadyLoaded = SceneManager.GetAllScenes ().Where (s => s.name == sceneName).Any ();

		if (isAlreadyLoaded) {
			yield return null;
		} else {
			yield return SceneManager.LoadSceneAsync (sceneName, LoadSceneMode.Additive);

			if (isSetActive) {
				Scene scene = SceneManager.GetSceneByName (sceneName);
				SceneManager.SetActiveScene (scene);
			}
		}

	}
}
