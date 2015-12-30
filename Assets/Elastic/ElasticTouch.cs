using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ElasticTouch : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {
	public RectTransform touchEffectRect;
	public GameObject touchEffect;

	[Header("Elastics")]
	public float stretchScale;
	[Header("Chains")]
	public float chainTime;
	public List<Color> chainColors;

	private class TouchStatus {
		public Vector2 start;
		public Vector2 end;

		public Vector2 last;
		public float   time;
		public int     count;

		public Vector2 Vec { get { return end - start; } }
		public Vector2 Dir { get { return (end - start).normalized; } }
		public float   Mag { get { return (end - start).magnitude; } }
	}
	private TouchStatus st = new TouchStatus ();

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

	public class EventHandler {
		public delegate void DirectionHandler(Vector2 vec);
		public delegate void ChainHandler (int count);
		public DirectionHandler OnUpdate = (vec) => {};
		public ChainHandler OnChain = (count) => {};
	}
	public EventHandler handler = new EventHandler();

	public void OnPointerDown (PointerEventData eventData) {
		Vector2 pos = LocalPos (eventData.position);
		touchEffectRect.localPosition = Local2Screen(pos);

		StartCoroutine (ripple (Local2Screen(pos)));

		st.start = pos;
		st.end   = pos;
		SetTargetAlpha (1f);
		SetStretch (0f);
	}
	public void OnPointerUp (PointerEventData eventData) {
		Vector2 pos = LocalPos (eventData.position);

		StartCoroutine (ripple (Local2Screen(pos)));

		st.start = Vector2.zero;
		st.end   = Vector2.zero;
		SetTargetAlpha (0f);

		handler.OnChain (st.count);
		st.count++;
		st.last = pos;
		st.time = chainTime;
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

	void Update() {
		st.time = Mathf.Max (st.time - Time.deltaTime, 0f);
		if (st.time == 0f) {
			st.count = 0;
		}

		handler.OnUpdate (st.Vec);
	}

	private Vector3 Local2Screen(Vector2 pos) {
		return new Vector3(pos.x, pos.y, 0f);
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

	private IEnumerator ripple(Vector3 pos) {
		GameObject obj = UIPoolManager.Instance.GetObject (UIPoolManager.Type.Ripple);
		//obj.transform.position = touchEffectRect.position;
		obj.transform.localPosition = pos;
		obj.GetComponent<UIRipple> ().Initilize (GetChainColor());
		yield return null;
	}

	private Color GetChainColor() {
		if (st.count < chainColors.Count) {
			return chainColors [st.count];
		} else {
			return chainColors.LastOrDefault ();
		}
	}
}

