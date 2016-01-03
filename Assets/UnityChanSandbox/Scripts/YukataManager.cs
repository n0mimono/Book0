using UnityEngine;
using System.Collections;
using Custom;

public partial class YukataManager : MonoBehaviour {

	public ElasticCameraOperator cameraOperator;
	public ElasticTouch elasticTouch;
	public UICameraControl uiCameraControl;

	public YukataAction yukataAction;
	public Transform    cameraTrans;

	public float speedScale;

	IEnumerator Start() {
		elasticTouch.handler.OnUpdate += OnTouchUpdate;
		elasticTouch.handler.OnChain += OnChainAction;
		elasticTouch.handler.OnRelease += OnReleaseAction;
		elasticTouch.handler.OnFlicked += OnFlickSlide;

		uiCameraControl.OnCameraChanged += OnCameraChanged;
		cameraOperator.OnModeChanged += uiCameraControl.UpdateCameraButtons;

		yukataAction.InLockAction += LockAction;

		yield return null;

		uiCameraControl.OnCameraChangeButtonClicked ((int)ElasticCameraOperator.Mode.Forwarding);
	}

	private void OnTouchUpdate(Vector2 touchDir) {
		Vector3 velocity = cameraTrans.ToWorldVec (touchDir) * speedScale;
		yukataAction.SetTargetVelocity (velocity);
	}

	private void OnChainAction(int count) {
		if (count >= 3) {
			yukataAction.StartLockedAction (YukataAction.AnimeAction.Jump, UnlockAction, true);
		}
	}

	private void OnReleaseAction(int count) {
		if (count >= 3) {
			yukataAction.StartLockedAction (YukataAction.AnimeAction.Salute, UnlockAction, true);
			uiCameraControl.OnCameraChangeButtonClicked ((int)ElasticCameraOperator.Mode.Salute);
		}
	}

	private void OnFlickSlide(Vector2 slideDir) {
		Vector3 dir = cameraTrans.ToWorldVec (slideDir).normalized;

		yukataAction.StartLockedAction (YukataAction.AnimeAction.Dive, UnlockAction, false);
		yukataAction.SetDiveVelocity (dir);
	}

	private void LockAction(YukataAction.AnimeAction act) {
		elasticTouch.SetActive (false);
	}

	private void UnlockAction(YukataAction.AnimeAction act) {
		elasticTouch.SetActive (true);
	}

	private void OnCameraChanged(ElasticCameraOperator.Mode mode, bool isBase) {
		cameraOperator.SetMode (mode, isBase);
	}
}
