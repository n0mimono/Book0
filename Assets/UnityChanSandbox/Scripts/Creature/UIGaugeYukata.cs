using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIGaugeYukata : MonoBehaviour {
	public Image image;
	public float lerpSpeed;

	private float targetRate;

	void Update() {
		image.fillAmount = Mathf.Lerp (image.fillAmount, targetRate, lerpSpeed * Time.deltaTime);
	}

	public void SetRate(float rate) {
		targetRate = rate;
	}

}
