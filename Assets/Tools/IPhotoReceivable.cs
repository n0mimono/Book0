using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public interface IPhotoRceivable : IEventSystemHandler {
	void OnRecieve(Transform target);
}
