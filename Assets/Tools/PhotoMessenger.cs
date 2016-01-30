using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class PhotoMessenger : MonoBehaviour {

	void OnEnable() {
		GameObject receiver = Common.PhotoReceiver ();
		ExecuteEvents.Execute<IPhotoRceivable> (receiver, null, (t, y) => t.OnRecieve(transform));
	}

}
