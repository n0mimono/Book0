using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIScrollItem : MonoBehaviour {
	public Text text;
	public Button button;

	public void SetText(string text) {
		this.text.text = text;
	}

	public void AddOnClickHandler(System.Action action) {
		button.onClick.AddListener (() => action ());
	}
}
