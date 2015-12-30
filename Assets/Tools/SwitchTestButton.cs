using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SwitchTestButton : MonoBehaviour {

	public List<GameObject> objs;
	public int activeIndex;

	void Start() {
		activeIndex = -1;
		OnClicked ();
	}

	public void OnClicked() {
		activeIndex = (activeIndex + 1) % objs.Count;
		objs.ForEach (o => o.SetActive (false));
		objs [activeIndex].SetActive (true);
	}

}
