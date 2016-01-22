using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Custom;
using UnityEngine.SceneManagement;

public class FieldManager : MonoBehaviour {
	public List<string> commonScenes;
	public List<string> specScenes;
	public FadePanelManager fadeMan;

	public bool startOnStart;

	private bool isReady;
	private bool isFieldLoaded;
	private string curSpecScenePath;

	void Awake() {
		isReady = false;
		isFieldLoaded = false;
	}

	void Start() {
		if (startOnStart) {
			Initilize ();
		}
	}

	public void Initilize() {
		ProcInit ().OnCompleted (() => isReady = false).StartBy (this);
	}

	private IEnumerator ProcInit() {
		foreach (string scenePath in commonScenes) {
			yield return SceneManager.LoadSceneAsync (scenePath, LoadSceneMode.Additive);
			yield return null;
		}
		isReady = true;
	}

	public void LoadField(int index) {
		ProcLoadField (index).StartBy (this);
	}

	private IEnumerator ProcLoadField(int index) {
		yield return null;
		yield return fadeMan.FadeIn ();
		yield return null;

		if (isFieldLoaded) {
			SceneManager.UnloadScene (curSpecScenePath);
		}
		isFieldLoaded = true;
		yield return null;

		curSpecScenePath = specScenes [index];
		yield return SceneManager.LoadSceneAsync (curSpecScenePath, LoadSceneMode.Additive);
		yield return null;
		yield return fadeMan.FadeOut ();
	}

}
