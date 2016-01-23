using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Custom;

public class CountDown : MonoBehaviour {
	public List<GameObject> numbers;

	IEnumerator Start() {
		numbers.ForEach (n => n.SetActive (false));

		yield return new WaitForSeconds (1f);
		numbers.ForEach (n => n.SetActive (false));
		numbers [0].SetActive (true);

		yield return new WaitForSeconds (1f);
		numbers.ForEach (n => n.SetActive (false));
		numbers [1].SetActive (true);

		yield return new WaitForSeconds (1f);
		numbers.ForEach (n => n.SetActive (false));
		numbers [2].SetActive (true);

		yield return new WaitForSeconds (1f);
		gameObject.SetActive (false);
	}

}
