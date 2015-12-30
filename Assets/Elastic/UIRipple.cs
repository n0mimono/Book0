using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIRipple : MonoBehaviour {
	public RectTransform trans;
	public Image         image;
	public float         timeSacle;

	public void Initilize(Color color) {
		image.color = new Color (color.r, color.g, color.b, 0f);

		StartCoroutine (Ripple ());
	}

	private IEnumerator Ripple() {
		
		float time = 0f;
		while (time <= 1f) {
			trans.localScale = Vector3.one * (1f + time);
			image.color = new Color (image.color.r, image.color.g, image.color.b, 1f - time);

			time += Time.deltaTime * timeSacle;
			yield return null;
		}

		yield return null;
		gameObject.SetActive (false);
	}

}
