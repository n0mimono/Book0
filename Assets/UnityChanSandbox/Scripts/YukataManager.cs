using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Custom;

public partial class YukataManager : MonoBehaviour {
	public List<YukataActionMaster> yukataList;
	public int yukataIndex;

	public ElasticCameraOperator cameraOperator;
	public ElasticTouch elasticTouch;

	public UICameraControl uiCameraControl;
	public UIInputControl  uiInputControl;
	public UIGaugeYukata   uiHpGauge;
	public UIGaugeYukata   uiMpGauge;

	public YukataAction yukataAction;
	public Transform    cameraTrans;

	public float speedScale;

	IEnumerator Start() {
		bool isReady = false;
		uiInputControl.OnCharacterSelected += (index) => {
			yukataIndex = index;
			isReady = true;
		};
		yield return new WaitUntil (() => isReady);

		FieldManager.Instance.Initilize ();

		yukataList.ForEach (y => y.gameObject.SetActive (false));
		yukataList [yukataIndex].gameObject.SetActive(true);
		yukataAction = yukataList [yukataIndex].action;

		elasticTouch.handler.OnUpdate += OnTouchUpdate;
		elasticTouch.handler.OnChain += OnChainAction;
		elasticTouch.handler.OnHold += OnHoldction;
		elasticTouch.handler.OnRelease += OnReleaseAction;
		elasticTouch.handler.OnFlicked += OnFlickSlide;

		uiCameraControl.OnCameraChanged += OnCameraChanged;
		uiCameraControl.OnCameraForwardUpdate = cameraOperator.ForwardUpdateImmediate;
		cameraOperator.OnModeChanged += uiCameraControl.UpdateCameraButtons;

		uiInputControl.OnControlEvent += OnControl;

		yukataAction.InLockAction += LockAction;
		yukataAction.OutLockAction += UnlockAction;
		yukataAction.OnSetTarget = cameraOperator.SetEnemyTarget;
		yukataAction.DeadHandler += OnGameOver;

		cameraOperator.SetPlayerTarget (yukataAction.transform);
		yield return null;

		uiCameraControl.OnCameraChangeButtonClicked ((int)ElasticCameraOperator.Mode.PlayerTargeting);
		yukataAction.HpChangeHander += uiHpGauge.SetRate;
		yukataAction.MpChangeHander += uiMpGauge.SetRate;

		yukataAction.ForceHpUpdate (); // initialization
	}

	private void OnTouchUpdate(Vector2 touchDir) {
		Vector3 velocity = cameraTrans.ToWorldVec (touchDir) * speedScale;
		yukataAction.Move (velocity);
	}

	private void OnChainAction(int count) {
		if (count == 0) {
			yukataAction.StopChainSpell ();
		} else if (count == 2) {
			yukataAction.StartChainSpell ();
		} else if (count >= 3) {
			yukataAction.ChainSpell ();
		}
			
	}

	private void OnHoldction(int count) {
		if (count == 0) {
			yukataAction.StopHoldSpell ();
		} else if (count == 1) {
			yukataAction.StartHoldSpell ();
		}
	}

	private void OnReleaseAction(int count) {
		yukataAction.StopHoldSpell ();
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

	private void OnControl(int index) {
		yukataAction.SpellAngel ((act) => {});
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
