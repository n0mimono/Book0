using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Custom;
using UnityEngine.UI;

public class PixieGauge : MonoBehaviour {
	public List<Image> images;

	public Color color;
	public Color filledColor;
	public Color crisisColor;

	public void SetNumber(int n) {
		for (int i = 0; i < images.Count; i++) {
			images [i].enabled = i <= n;
		}
		SetFill (n >= images.Count - 1);
	}


	public void SetFullNumber() {
		SetNumber (images.Count - 1);
	}

	private void SetFill(bool isFilled) {
		images.ForEach (i => i.color = isFilled ? filledColor : color);
	}

	public void SetCrisis(bool isCrisis) {
		images.ForEach (i => i.color = isCrisis ? crisisColor : color);
	}

}
