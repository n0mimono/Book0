using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class YukataSetter : MonoBehaviour {

	[ContextMenu("Set Shadow-On To All Renderers")]
	public void SetAllReceiveShadowOn() {
		GetComponentsInChildren<Renderer> ()
			.ToList ().ForEach (r => r.receiveShadows = true);
		GetComponentsInChildren<Renderer> ()
			.ToList ().ForEach (r => r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On);
	}

	[ContextMenu("Set Shadow-Off To All Renderers")]
	public void SetAllReceiveShadowOff() {
		GetComponentsInChildren<Renderer> ()
			.ToList ().ForEach (r => r.receiveShadows = false);
		GetComponentsInChildren<Renderer> ()
			.ToList ().ForEach (r => r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off);
	}

}
