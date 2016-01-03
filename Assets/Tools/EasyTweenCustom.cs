using UnityEngine;
using System.Collections;

public partial class EasyTween {

	public void OpenObjectAnimation() {
		if (!IsObjectOpened ()) {
			OpenCloseObjectAnimation ();
		}
	}

	public void CloseObjectAnimation() {
		if (IsObjectOpened ()) {
			OpenCloseObjectAnimation ();
		}
	}

}
