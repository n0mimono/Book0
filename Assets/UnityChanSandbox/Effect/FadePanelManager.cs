using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadePanelManager : MonoBehaviour {
	public Image fadeImage;
	public const float FixedFrameRate = 30f;

	private bool isFading;

	void Awake() {
		isFading = false;
	}

	void Update() {
		fadeImage.enabled = isFading;
	}

	private void SetAlpha(float value) {
		fadeImage.color = new Color (1f, 1f, 1f, Mathf.Clamp (value, 0f, 1f));
	}

	public CustomYieldInstruction FadeIn() {
		isFading = true;

		float t = 0f;
		return new WaitWhile (() => {
			t += 1f / FixedFrameRate;
			SetAlpha (t);
			return t <= 1f;
		});
	}

	public CustomYieldInstruction FadeOut() {
		float t = 1f;
		return new WaitWhile (() => {
			t -= 1f / FixedFrameRate;
			SetAlpha(t);
			isFading = t >= 0f;
			return isFading;
		});
	}

}
