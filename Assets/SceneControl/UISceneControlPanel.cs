using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class UISceneControlPanel : MonoBehaviour {
	public UIScrollPanel scrollPanel;

	[System.Serializable]
	public class BuiltScene {
		public string name;
		public bool   enabled;
		public string path;
	}
	public List<BuiltScene> scenes;

	[Custom.Button("UpdateBuiltScenes", "Update Built Scenes" )]
	public int ButtonUpdateBuiltScenes;

	IEnumerator Start() {
		foreach (BuiltScene scene in scenes.Where(s => s.enabled)) {
			System.Action action =
				((System.Func<string, System.Action>)((name) => (() => LoadScene (name)))) (scene.name);

			scrollPanel.AddItem (scene.name, action);
			yield return null;
		}
	}

	private void LoadScene(string path) {
		StartCoroutine (LoadSceneAsync (path));
	}

	private IEnumerator LoadSceneAsync(string path) {
		AsyncOperation op = SceneManager.LoadSceneAsync (path);
		yield return op;
	}

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
	}

}
