using UnityEngine;
using System.Collections;
using Custom;

public partial class YukataManager : MonoBehaviour {

	public MultiTargetCamera cameraManager;
	public ElasticTouch elasticTouch;

	public YukataAction yukataAction;
	public Transform    cameraTrans;

	public float speedScale;

	IEnumerator Start() {
		elasticTouch.handler.OnUpdate += OnTouchUpdate;
		elasticTouch.handler.OnChain += OnChainAction;
		elasticTouch.handler.OnRelease += OnReleaseAction;
		elasticTouch.handler.OnFlicked += OnFlickSlide;

		yield return null;
	}

	public void OnClickedCameraButton() {
		cameraManager.StartMagi ();
	}

	private void OnTouchUpdate(Vector2 touchDir) {
		Vector3 velocity = cameraTrans.ToWorldVec (touchDir) * speedScale;
		yukataAction.SetTargetVelocity (velocity);
	}

	private void OnChainAction(int count) {
		if (count >= 3) {
			LockAction ();
			yukataAction.StartAnimeAction (YukataAction.AnimeAction.Jump, UnlockAction, true);
		}
	}

	private void OnReleaseAction(int count) {
		if (count >= 3) {
			LockAction ();
			yukataAction.StartAnimeAction (YukataAction.AnimeAction.Salute, UnlockAction, true);
			cameraManager.StartSalute();
		}
	}

	private void OnFlickSlide(Vector2 slideDir) {
		Vector3 dir = cameraTrans.ToWorldVec (slideDir).normalized;

		LockAction ();
		yukataAction.StartAnimeAction (YukataAction.AnimeAction.Dive, UnlockAction, false);
		yukataAction.SetDiveVelocity (dir);
	}

	private void LockAction() {
		elasticTouch.SetActive (false);
		yukataAction.SetWalkable (false);
	}

	private void UnlockAction() {
		elasticTouch.SetActive (true);
		yukataAction.SetWalkable (true);
	}

}
