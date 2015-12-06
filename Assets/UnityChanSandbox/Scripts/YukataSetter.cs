using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class YukataSetter : MonoBehaviour {

	[ContextMenu("Set Receive-Shadow-On To All Renderers")]
	public void SetAllReceiveShadowOn() {
		GetComponentsInChildren<Renderer> ()
			.ToList ().ForEach (r => r.receiveShadows = true);
	}

	[ContextMenu("Set Receive-Shadow-Off To All Renderers")]
	public void SetAllReceiveShadowOff() {
		GetComponentsInChildren<Renderer> ()
			.ToList ().ForEach (r => r.receiveShadows = false);
	}

}
