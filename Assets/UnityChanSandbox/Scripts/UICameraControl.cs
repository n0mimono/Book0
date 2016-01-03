using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using Custom;

public class UICameraControl : MonoBehaviour {
	public delegate void CameraChangeHandler(ElasticCameraOperator.Mode mode, bool isBase);
	public event CameraChangeHandler OnCameraChanged;

	[System.Serializable]
	public class SwitchButton {
		public ElasticCameraOperator.Mode mode;
		public bool   isBaseButton;
		public Button button;
		public bool   isDummy;
	}
	public List<SwitchButton> buttons;

	public Color playingColor;
	public Color inactiveColor;

	void Awake() {
		OnCameraChanged += (mode, isBase) =>  {};
	}

	public void OnCameraChangeButtonClicked(int id) {
		ElasticCameraOperator.Mode mode = (ElasticCameraOperator.Mode)id;

		SwitchButton button = buttons
			.Where (b => b.mode == mode).FirstOrDefault ();

		OnCameraChanged (mode, button.isBaseButton);
	}

	public void UpdateCameraButtons(ElasticCameraOperator.Mode mode) {
		SwitchButton button = buttons
			.Where (b => b.mode == mode).FirstOrDefault ();
		List<SwitchButton> others = buttons
			.Where (b => b.mode != mode)
			.Where (b => !b.isDummy).ToList ();

		// interactive
		if (!button.isDummy && button.isBaseButton) {
			button.button.interactable = false;
			others
				.Where (b => b.isBaseButton)
				.Select (b => b.button).ToList ()
				.ForEach (b => b.interactable = true);
		}

		// color
		if (!button.isDummy) {
			button.button.SetColor (playingColor);
		}
		others.ForEach (b => b.button.SetColor (inactiveColor));
	}

}
