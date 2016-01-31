using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Custom;
using UnityEngine.UI;

public class BossHpGauge : MonoBehaviour {
	public RectTransform image;

	public float lerpSpeed;

	private float curRate;
	private float tgtRate;

	private Vector2 orgSize;

	void Awake() {
		curRate = 0f;
		tgtRate = 1f;
		orgSize = image.sizeDelta;
	}

	public void SetHpRate(float rate) {
		tgtRate = Mathf.Clamp(rate, 0f, 1f);

		image.sizeDelta = Vector2.Scale (orgSize, new Vector2 (rate, 1f));
	}

	void Update() {
		curRate = Mathf.Lerp (curRate, tgtRate, lerpSpeed * Time.deltaTime);
		image.sizeDelta = Vector2.Scale (orgSize, new Vector2 (curRate, 1f));
	}
}
