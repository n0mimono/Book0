using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Custom;
using UnityEngine.SceneManagement;

public class FieldManager : SingletonMonoBehaviorWithoutCreate<FieldManager> {
	public List<string> commonScenes;
	public List<string> specScenes;

	public FadePanelManager fadeMan;

	public bool startOnStart;
	public bool load0OnInit;

	public bool isReady;

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
		if (load0OnInit) {
			yield return StartCoroutine (ProcLoadField (0));
		}

		isReady = true;
	}

	public void LoadField(int index, bool withFade) {
		if (withFade) {
			ProcLoadFieldWithFade (index).StartBy (this);
		} else {
			ProcLoadField (index).StartBy (this);
		}
	}

	private IEnumerator ProcLoadFieldWithFade(int index) {
		Time.timeScale = 0f;
		yield return fadeMan.FadeIn ();

		yield return StartCoroutine (ProcLoadField (index));

		yield return fadeMan.FadeOut ();
		Time.timeScale = 1f;
	}

	private IEnumerator ProcLoadField(int index) {
		yield return null;
		if (isFieldLoaded) {
			SceneManager.UnloadScene (curSpecScenePath);
		}
		isFieldLoaded = true;
		yield return null;

		curSpecScenePath = specScenes [index];
		yield return SceneManager.LoadSceneAsync (curSpecScenePath, LoadSceneMode.Additive);
		SceneManager.SetActiveScene (SceneManager.GetSceneByName (curSpecScenePath));
		yield return null;
	}
		
}
