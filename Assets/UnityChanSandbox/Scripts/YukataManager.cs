﻿using UnityEngine;
using System.Collections;

public class YukataManager : MonoBehaviour {

	public MultiTargetCamera cameraManager;
	public ElasticTouch elasticTouch;

	public YukataAction yukataAction;
	public Transform    cameraTrans;

	public float speedScale;

	IEnumerator Start() {
		elasticTouch.handler.OnUpdate += OnTouchUpdate;
		elasticTouch.handler.OnChain += OnChainAction;

		yield return null;
	}

	public void OnClickedCameraButton() {
		cameraManager.StartMagi ();
	}

	private void OnTouchUpdate(Vector2 touchDir) {
		Vector3 velocity = new Vector3 (touchDir.x, 0f, touchDir.y) * speedScale;
		velocity = Quaternion.AngleAxis (cameraTrans.eulerAngles.y, Vector3.up) * velocity;

		yukataAction.SetTargetVelocity (velocity);
	}

	private void OnChainAction(int count) {
		if (count == 3) {
			LockAction ();
			yukataAction.StartAnimeAction (YukataAction.AnimeAction.Salute, UnlockAction);
		}
	}

	private void LockAction() {
		elasticTouch.SetActive (false);
	}

	private void UnlockAction() {
		elasticTouch.SetActive (true);
	}

}
