using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UIScrollPanel : MonoBehaviour {

	public List<UIScrollItem> items;
	public RectTransform contentRoot;
	public UIScrollItem itemPrefab;
	public float height;

	void Start() {
		items = new List<UIScrollItem> ();
	}

	public void AddItem(string text, System.Action onClicked) {
		StartCoroutine(AddItemProc(text, onClicked));
	}

	private IEnumerator AddItemProc(string text, System.Action onClicked) {
		UIScrollItem item = GameObject.Instantiate<UIScrollItem> (itemPrefab);
		yield return null;

		item.SetText (text);
		item.AddOnClickHandler (onClicked);

		RectTransform trans = item.GetComponent<RectTransform> ();
		trans.SetParent (contentRoot);
		trans.gameObject.SetActive (true);
		trans.localScale = Vector3.one;
		trans.anchoredPosition = new Vector3 (0f, -height * items.Count, 0f);

		items.Add (item);
		contentRoot.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, height * items.Count);

		yield return null;
	}
}
