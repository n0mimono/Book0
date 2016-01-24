using UnityEngine;
using System.Collections;
using Custom;

public partial class YukataManager : MonoBehaviour {

	public ElasticCameraOperator cameraOperator;
	public ElasticTouch elasticTouch;

	public UICameraControl uiCameraControl;
	public UIHpGaugeYukata uiHpGauge;

	public YukataAction yukataAction;
	public Transform    cameraTrans;

	public float speedScale;

	IEnumerator Start() {
		elasticTouch.handler.OnUpdate += OnTouchUpdate;
		elasticTouch.handler.OnChain += OnChainAction;
		elasticTouch.handler.OnHold += OnHoldction;
		elasticTouch.handler.OnRelease += OnReleaseAction;
		elasticTouch.handler.OnFlicked += OnFlickSlide;

		uiCameraControl.OnCameraChanged += OnCameraChanged;
		uiCameraControl.OnCameraForwardUpdate = cameraOperator.ForwardUpdateImmediate;
		cameraOperator.OnModeChanged += uiCameraControl.UpdateCameraButtons;

		yukataAction.InLockAction += LockAction;
		yukataAction.OutLockAction += UnlockAction;
		yukataAction.OnSetTarget = cameraOperator.SetEnemyTarget;
		yukataAction.DeadHandler += OnGameOver;

		yield return null;

		uiCameraControl.OnCameraChangeButtonClicked ((int)ElasticCameraOperator.Mode.PlayerTargeting);

		yukataAction.HpChangeHander += uiHpGauge.SetHPRate;
		yukataAction.ForceHpUpdate (); // initialization
	}

	private void OnTouchUpdate(Vector2 touchDir) {
		Vector3 velocity = cameraTrans.ToWorldVec (touchDir) * speedScale;
		yukataAction.Move (velocity);
	}

	private void OnChainAction(int count) {
		if (count >= 3) {
			yukataAction.SpellFlower ((act) => {});
			uiCameraControl.OnCameraChangeButtonClicked ((int)ElasticCameraOperator.Mode.Salute);
		}
	}

	private void OnHoldction(int count) {
		if (count == 0) {
			yukataAction.StopSpell ();
		} else if (count == 1) {
			yukataAction.StartSpell ();
		} else if (count == 15) {
			yukataAction.StopSpell ();
			yukataAction.SpellAngel ((act) => {});
			uiCameraControl.OnCameraChangeButtonClicked ((int)ElasticCameraOperator.Mode.Magi);
		}
	}

	private void OnReleaseAction(int count) {
		if (count >= 2) {
			yukataAction.ReleaseSpell ();
		}
	}

	private void OnFlickSlide(Vector2 slideDir) {
		Vector3 dir = cameraTrans.ToWorldVec (slideDir).normalized;
		yukataAction.Dive (dir, (act) => {});
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

public partial class YukataManager {

	private void OnGameOver() {
		uiCameraControl.OnCameraChangeButtonClicked ((int)ElasticCameraOperator.Mode.Dead);

		ReturnEntrance ().StartBy(this);
	}

	private IEnumerator ReturnEntrance() {
		yield return new WaitForSeconds (3f);
		FieldManager.Instance.LoadField (0, true);
		yukataAction.transform.position = Vector3.zero;

		yield return null;
		yukataAction.Revive ();
	}
}
