using UnityEngine;
using System.Collections;
using System;

public class UIInputControl : MonoBehaviour {
	public GameObject selectPanel;

	public event Action<int> OnCharacterSelected;
	public event Action<int> OnControlEvent;

	public void OnClickCharacterSelect(int index) {
		OnCharacterSelected (index);
		selectPanel.SetActive (false);
	}

	public void OnClickControlButton(int index) {
		OnControlEvent (index);
	}

}
