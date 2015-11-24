using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Book))]
public class ManualPager : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {

	private float curPageNumber {
		set {
			GetComponent<Book> ().curPageNumber = value;
		}
		get {
			return GetComponent<Book> ().curPageNumber;
		}
	}
	private float curPageNumberFloor {
		get {
			return Mathf.Floor (curPageNumber);
		}
	}
	private float curPageNumberFrac {
		get {
			return curPageNumber - curPageNumberFloor;
		}
	}

	private bool isStop;

	public void OnPointerDown (PointerEventData eventData) {
		isStop = true;
	}

	public void OnDrag (PointerEventData eventData) {
		curPageNumber = curPageNumber - eventData.delta.x * 0.002f;
	}

	public void OnPointerUp (PointerEventData eventData) {
		isStop = false;
	}

	IEnumerator Start() {

		while (isStop) {
			yield return null;
		}

		while (!isStop) {
			yield return null;
			float targetPageNumber = curPageNumberFrac > 0.5f ? curPageNumberFloor + 1f : curPageNumberFloor;
			curPageNumber = Mathf.Lerp (curPageNumber, targetPageNumber, 0.05f);
		}

		StartCoroutine (Start ());
	}

}
