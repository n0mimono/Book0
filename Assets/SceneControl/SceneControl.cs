using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour {
	public UIScrollPanel scrollPanel;
	public GameObject    toTop;
	public FadeManager   fadePanel;

	[System.Serializable]
	public class BuiltScene {
		public string name;
		public bool   enabled;
		public string path;
	}
	public List<BuiltScene> scenes;

	private BuiltScene currentScene = null;

	[Custom.Button("UpdateBuiltScenes", "Update Built Scenes" )]
	public int ButtonUpdateBuiltScenes;

	IEnumerator Start() {
		DontDestroyOnLoad (gameObject);

		foreach (BuiltScene scene in scenes.Where(s => s.enabled)) {
			System.Action action =
				((System.Func<string, System.Action>)((name) => (() => LoadScene (name)))) (scene.name);

			scrollPanel.AddItem (scene.name, action);
			yield return null;
		}

		yield return new WaitUntil (() => fadePanel.IsReady);
		yield return fadePanel.FadeOut();
	}

	public void LoadTopScene() {
		LoadScene (scenes [0].name);
	}

	private void LoadScene(string name) {
		StartCoroutine (LoadSceneAsync (name));
	}

	private IEnumerator LoadSceneAsync(string name) {
		currentScene = scenes.Where (s => s.name == name).FirstOrDefault ();
		yield return fadePanel.FadeIn();

		AsyncOperation op = SceneManager.LoadSceneAsync (currentScene.name);
		yield return op;

		yield return fadePanel.FadeOut();
	}

	#if UNITY_EDITOR
	private void UpdateBuiltScenes() {
		scenes = new List<BuiltScene> ();
		foreach (var s in UnityEditor.EditorBuildSettings.scenes) {
			scenes.Add (
				new BuiltScene() {
					enabled = s.enabled,
					name    = System.IO.Path.GetFileNameWithoutExtension(s.path),
					path    = s.path,
				}
			);
		}
		scenes [0].enabled = false;
	}
	#endif

}
