using UnityEngine;
using System.Collections;

public class YukataManager : MonoBehaviour {

	public ElasticTouch elasticTouch;
	public YukataAction yukataAction;
	public Transform    cameraTrans;

	public float speedScale;

	IEnumerator Start() {
		yield return null;

		while (true) {
			yield return null;

			Vector2 touchDir = elasticTouch.Dir;
			Vector3 velocity = new Vector3 (touchDir.x, 0f, touchDir.y) * speedScale;
			velocity = Quaternion.AngleAxis (cameraTrans.eulerAngles.y, Vector3.up) * velocity;

			yukataAction.SetTargetVelocity (velocity);
		}

	}

}
