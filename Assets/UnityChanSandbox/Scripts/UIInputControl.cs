using UnityEngine;
using System.Collections;
using System;

public class UIInputControl : MonoBehaviour {
	public GameObject selectPanel;

	public event Action<int> OnCharacterSelected;

	public void OnClickCharacterSelect(int index) {
		OnCharacterSelected (index);
		selectPanel.SetActive (false);
	}

}
