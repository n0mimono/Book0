using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour {
	public Image image;
	public float fadeTimeScale;

	private Material material;
	private bool isReady = false;

	public  bool IsReady { get { return isReady; } }

	void Start() {
		material = Material.Instantiate (image.material);
		image.material = material;
		SetOpen (0);

		isReady = true;
	}

	public void SetOpen(float value) {
		image.material.SetFloat ("_Open", value);
		image.gameObject.SetActive (Mathf.Abs (value) < 1f);
	}

	public Custom.ActionWithTime FadeIn() {
		return new Custom.ActionWithTime ((t,dt) => SetOpen(-1+t), fadeTimeScale);
	}

	public Custom.ActionWithTime FadeOut() {
		return new Custom.ActionWithTime ((t,dt) => SetOpen(t), fadeTimeScale);
	}

}
