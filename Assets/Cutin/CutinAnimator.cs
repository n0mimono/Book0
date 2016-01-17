using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Custom;

public partial class CutinAnimator : MonoBehaviour {
	public Image image;
	[Range(0.1f, 3f)] public float enterTime;
	[Range(0.1f, 3f)] public float inTime;
	[Range(0.1f, 3f)] public float exitTime;


	private Material material;
	private bool isPlaying;

	void Start() {
		material = Instantiate(image.material);
		material.hideFlags = HideFlags.DontSave;
		image.material = material;

		SetAnimTime (0f);
	}

	public void StartAnim() {
		if (isPlaying) return;

		isPlaying = true;
		ProcAnim ().OnCompleted(() => isPlaying = false).StartBy (this);
	}

	private IEnumerator ProcAnim() {
		float time = 0f;

		SetAnimTime (0f);
		yield return null;

		while (time <= 1f) {
			time += Time.deltaTime / enterTime;
			SetAnimTime (time);
			yield return null;
		}

		while (time <= 2f) {
			time += Time.deltaTime / inTime;
			SetAnimTime (time);
			yield return null;
		}

		while (time <= 3f) {
			time += Time.deltaTime / exitTime;
			SetAnimTime (time);
			yield return null;
		}

		SetAnimTime (0f);
		yield return null;
	}

	private void SetAnimTime(float time) {
		material.SetFloat ("_AnimTime", time);
	}

}

public partial class CutinAnimator {
	[Button("StartAnim", "Start Animation")] public int ButtonStartAnim; 
	[Button("StartAnimSlow", "Start Animation (Slow)")] public int ButtonStartAnimSlow; 

	public void StartAnimSlow() {
		if (isPlaying) return;

		float saveEnterTime = enterTime;
		float saveInTime = inTime;
		float saveExitTime = exitTime;
		enterTime = 3f;
		inTime = 3f;
		exitTime = 3f;

		isPlaying = true;
		ProcAnim ().OnCompleted(() => {
			isPlaying = false;

			enterTime = saveEnterTime;
			inTime = saveInTime;
			exitTime = saveExitTime;
		}).StartBy (this);

	}

}
