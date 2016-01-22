using UnityEngine;
using System.Collections;
using Custom;

public class EffectFadeManager : MonoBehaviour {
	public Material material;
	public const float FixedFrameRate = 30f;

	private bool isFading;

	void Awake() {
		isFading = false;
		  
		material = Instantiate (material);
		material.hideFlags = HideFlags.DontSave;
	}

	void Start() {
	}

	void OnRenderImage(RenderTexture src, RenderTexture dst) {
		if (isFading) {
			Graphics.Blit (src, dst, material);
		} else {
			Graphics.Blit (src, dst);
		}
	}

	private void SetMaterilProperty(float value) {
		material.SetFloat ("_AnimTime", Mathf.Clamp(value, 0f, 1f));
	}

	public CustomYieldInstruction FadeIn() {
		isFading = true;

		float t = 0f;
		return new WaitWhile (() => {
			t += 1f / FixedFrameRate;
			SetMaterilProperty (t);
			isFading = t <= 1f;
			return isFading;
		});
	}

	public CustomYieldInstruction FadeOut() {
		isFading = true;

		float t = 1f;
		return new WaitWhile (() => {
			t -= 1f / FixedFrameRate;
			SetMaterilProperty(t);
			isFading = t >= 0f;
			return isFading;
		});
	}

}

public static class EffectFadeManagerExtension {

	public static CustomYieldInstruction FadeIn(this Camera camera) {
		EffectFadeManager man = camera.GetComponent<EffectFadeManager> ();
		return man.FadeIn ();
	}

	public static CustomYieldInstruction FadeOut(this Camera camera) {
		EffectFadeManager man = camera.GetComponent<EffectFadeManager> ();
		return man.FadeOut ();
	}

}
