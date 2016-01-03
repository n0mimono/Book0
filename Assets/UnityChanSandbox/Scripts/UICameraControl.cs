using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class UICameraControl : MonoBehaviour {
	public delegate void CameraSwitchHandler(ElasticCameraOperator.Mode mode, bool isBase);
	public event CameraSwitchHandler OnCameraSwitch;

	[System.Serializable]
	public class SwitchButton {
		public ElasticCameraOperator.Mode mode;
		public bool   isBaseButton;
		public Button button;
		public bool   isDummy;
	}
	public List<SwitchButton> buttons;

	void Awake() {
		OnCameraSwitch += (mode, isBase) =>  {};
	}

	public void OnCameraSwitchButton(int id) {
		ElasticCameraOperator.Mode mode = (ElasticCameraOperator.Mode)id;

		SwitchButton button = buttons.Where (b => b.mode == mode).FirstOrDefault ();
		if (!button.isDummy && button.isBaseButton) {
			button.button.interactable = false;
			buttons
				.Where(b => !b.isDummy)
				.Where (b => b.mode != mode)
				.Where (b => b.isBaseButton)
				.Select (b => b.button)
				.ToList ()
				.ForEach (b => b.interactable = true);
		}

		OnCameraSwitch (mode, button.isBaseButton);
	}

}
