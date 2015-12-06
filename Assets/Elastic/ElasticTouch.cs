using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ElasticTouch : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {
	public RectTransform touchEffectRect;
	public GameObject touchEffect;

	public float stretchScale;

	private class TouchStatus {
		public Vector2 start;
		public Vector2 end;

		public Vector2 Dir { get { return (end - start).normalized; } }
		public float   Mag { get { return (end - start).magnitude; } }
	}
	private TouchStatus st = new TouchStatus ();
	public Vector2 Dir { get { return st.Mag != 0f ? st.Mag * st.Dir : Vector2.zero; } }
			
	private class EffectStatus {
		public float alpha;
		public float targetAlpha;

		public void UpdateAlpha(float da) {
			float dAlpha = targetAlpha - alpha;
			if (Mathf.Abs (dAlpha) < da) {
				alpha = targetAlpha;
			} else {
				alpha += Mathf.Sign (dAlpha) * da;
			}
		}
		public void InitilizeAlpha() {
			alpha = 0f;
			targetAlpha = 0f;
		}
	}
	private EffectStatus cur = new EffectStatus();

	public void OnPointerDown (PointerEventData eventData) {
		Vector2 pos = LocalPos (eventData.position);
		touchEffectRect.localPosition = new Vector3(pos.x, pos.y, 0f);

		st.start = pos;
		st.end = pos;
		SetTargetAlpha (1f);
		SetStretch (0f);
	}
	public void OnPointerUp (PointerEventData eventData) {
		st.start = Vector2.zero;
		st.end = Vector2.zero;
		SetTargetAlpha (0f);
	}
	public void OnDrag (PointerEventData eventData) {
		Vector2 pos = LocalPos (eventData.position);

		st.end = pos;
		SetStretch (st.Mag * stretchScale);
		SetDirection (st.Dir);
	}

	void Awake() {
		Renderer effectRenderer = touchEffect.GetComponent<Renderer> ();
		effectRenderer.sharedMaterial = Material.Instantiate (effectRenderer.material);
		effectRenderer.sharedMaterial.hideFlags = HideFlags.HideAndDontSave;

		StartCoroutine (AlphaSetter ());
	}

	private Vector2 LocalPos(Vector2 pos) {
		RectTransform rectTrans = GetComponent<RectTransform> ();
		Vector2 vec = Vector2.zero;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTrans, pos, Camera.main, out vec);
		return vec;
	}

	[ContextMenu("Set Sorting Order Of Touch Effect")]
	public void SetSortingOrderOfTouchEffect() {
		Renderer effectRenderer = touchEffect.GetComponent<Renderer> ();
		effectRenderer.sortingOrder = 1;
	}

	private void SetStretch(float strength) {
		Renderer effectRenderer = touchEffect.GetComponent<Renderer> ();
		effectRenderer.sharedMaterial.SetFloat ("_StretchStrength", strength);
	}

	private void SetDirection(Vector2 dir) {
		Renderer effectRenderer = touchEffect.GetComponent<Renderer> ();
		effectRenderer.sharedMaterial.SetVector ("_StretchDirection", new Vector4(dir.x, 0f, -dir.y, 1f));
	}

	private IEnumerator AlphaSetter() {
		cur.InitilizeAlpha ();
		ApplyCurrentAlpha ();

		while (true) {
			yield return true;

			cur.UpdateAlpha (Time.deltaTime * 6f);
			ApplyCurrentAlpha ();
		}
	}

	private void ApplyCurrentAlpha() {
		Renderer effectRenderer = touchEffect.GetComponent<Renderer> ();
		effectRenderer.sharedMaterial.SetFloat ("_Alpha", cur.alpha);
	}

	private void SetTargetAlpha(float alpha) {
		cur.targetAlpha = alpha;
	}

	private void SetCurrentAlpha(float alpha) {
		cur.alpha = alpha;
	}

}

