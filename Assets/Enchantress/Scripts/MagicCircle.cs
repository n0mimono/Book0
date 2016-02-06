using UnityEngine;
using System.Collections;
using Custom;

public class MagicCircle : MonoBehaviour {
	public float rippleScale;
	public float timeScale = 1f;

	private Material mat;
	private Vector3 scales;
	private Color color;

	void Awake() {
		Renderer renderer = GetComponent<Renderer> ();
		mat = Material.Instantiate (renderer.sharedMaterial);
		mat.hideFlags = HideFlags.DontSave;
		renderer.sharedMaterial = mat;

		scales = transform.localScale;
		color = mat.GetColor ("_Color");
		SetAlpha (0f);
	}

	public void Hold() {
		Ripple (t => t);
	}

	public void Release() {
		Ripple (t => 1f - t);
	}

	private void Ripple(System.Func<float,float> timeToValue) {
		float time = 0f;
		new Noop ()
			.OnUpdate (() => time += Time.deltaTime * timeScale)
			.OnUpdate (() => SetAlpha (timeToValue(time)))
			.While (() => 0f <= time && time <= 1f)
			.OnCompleted (() => SetAlpha (timeToValue(time)))
			.StartBy (this);
	}

	private void SetAlpha(float alpha) {
		float val = Mathf.Clamp (alpha, 0f, 1f);
		mat.SetColor("_Color", new Color(color.r, color.g, color.b, val));

		transform.localScale = scales * (1f + rippleScale * (1f - val));
	}

	[Button("Hold", "Hold")] public int ButtonHold;
	[Button("Release", "Release")] public int ButtonRelease;
}
