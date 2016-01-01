using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Custom;

public class ElasticTouch : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {
	public RectTransform touchEffectRect;
	public GameObject touchEffect;

	[Header("Elastics")]
	public float stretchScale;
	[Header("Chains")]
	public float chainTime;
	public float chainDistance;
	public List<Color> chainColors;
	[Header("Hold")]
	public float holdCountTime;
	public float holdDistance;
	public List<Color> holdColors;

	private bool isActive;

	private class TouchStatus {
		public Vector2 start;
		public Vector2 end;

		public Vector2 last;
		public float   time;
		public int     count;

		public int holdCount;

		public Vector2 Vec { get { return end - start; } }
		public Vector2 Dir { get { return (end - start).normalized; } }
		public float   Mag { get { return (end - start).magnitude; } }
	}
	private TouchStatus st = new TouchStatus ();

	private class EffectStatus {
		public Vector3 color;
		public Vector3 targetColor;
		public float alpha;
		public float targetAlpha;

		public void Update(float delta) {
			alpha = Utility.ConstLerp (alpha, targetAlpha, delta);
			color = Utility.ConstLerp (color, targetColor, delta);
		}

		public void Initilize() {
			alpha       = 0f;
			targetAlpha = 0f;
			color       = Vector3.one;
			targetColor = Vector3.one;
		}
	}
	private EffectStatus cur = new EffectStatus();

	public class EventHandler {
		public delegate void DirectionHandler(Vector2 vec);
		public delegate void CountFireHandler (int count);
		public DirectionHandler OnUpdate = (vec) => {};
		public CountFireHandler OnChain = (count) => {};
		public CountFireHandler OnRelease = (count) => {};
	}
	public EventHandler handler = new EventHandler();

	public void OnPointerDown (PointerEventData eventData) {
		if (!isActive) return;

		Vector2 pos = LocalPos (eventData.position);
		touchEffectRect.localPosition = Local2Screen(pos);

		StartCoroutine (ripple (Local2Screen(pos), GetChainColor()));

		st.start = pos;
		st.end   = pos;
		SetTargetAlpha (1f);
		SetStretch (0f);
	}
	public void OnPointerUp (PointerEventData eventData) {
		if (!isActive) return;

		Vector2 pos = LocalPos (eventData.position);

		StartCoroutine (ripple (Local2Screen(pos), GetChainColor()));

		st.start = Vector2.zero;
		st.end   = Vector2.zero;
		SetTargetAlpha (0f);

		st.count = Vector3.Distance (st.last, pos) < chainDistance ? st.count : 0;
		handler.OnChain (st.count);
		st.count++;
		st.last = pos;
		st.time = chainTime;
	}
	public void OnDrag (PointerEventData eventData) {
		if (!isActive) return;

		Vector2 pos = LocalPos (eventData.position);

		st.end = pos;
		SetStretch (st.Mag * stretchScale);
		SetDirection (st.Dir);
	}

	void Awake() {
		Renderer effectRenderer = touchEffect.GetComponent<Renderer> ();
		effectRenderer.sharedMaterial = Material.Instantiate (effectRenderer.material);
		effectRenderer.sharedMaterial.hideFlags = HideFlags.HideAndDontSave;

		StartCoroutine (UpdateEffectColor ());
		StartCoroutine (CheckHoldDown ());

		SetActive (true);
	}

	void Update() {
		if (!isActive) return;

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

	private IEnumerator UpdateEffectColor() {
		cur.Initilize ();
		ApplyCurrentEffectColor ();

		while (true) {
			yield return true;

			cur.Update (Time.deltaTime * 6f);
			ApplyCurrentEffectColor ();
		}
	}

	private void ApplyCurrentEffectColor() {
		Renderer effectRenderer = touchEffect.GetComponent<Renderer> ();
		Material mat = effectRenderer.sharedMaterial;
		mat.SetFloat ("_Alpha", cur.alpha);

		float baseAlpha = mat.GetColor ("_Color").a;
		mat.SetColor ("_Color", cur.color.ToColor (baseAlpha));
	}

	private void SetTargetAlpha(float alpha) {
		cur.targetAlpha = alpha;
	}

	public void SetTargetColor(Color color) {
		cur.targetColor = color.ToVector3();
	}

	private IEnumerator ripple(Vector3 pos, Color color) {
		GameObject obj = UIPoolManager.Instance.GetObject (UIPoolManager.Type.Ripple);
		//obj.transform.position = touchEffectRect.position;
		obj.transform.localPosition = pos;
		obj.GetComponent<UIRipple> ().Initilize (color);
		yield return null;
	}

	private Color GetChainColor() {
		return st.count < chainColors.Count ? chainColors [st.count] : chainColors.LastOrDefault ();
	}

	private Color GetHoldColor() {
		return st.holdCount < holdColors.Count ? holdColors [st.holdCount] : holdColors.LastOrDefault ();
	}

	public void SetActive(bool isActive) {
		this.isActive = isActive;

		if (!isActive) {
			ClearState ();
		}
	}

	private void ClearState() {
		st.start = Vector2.zero;
		st.end   = Vector2.zero;
		SetTargetAlpha (0f);

		st.count = 0;
		st.last = Vector2.zero;
		st.time = 0f;
	}

	private IEnumerator CheckHoldDown() {
		bool curIsHold = false;
		IEnumerator holdRoutine = null;

		System.Func<bool> isHold = () => 
			st.start != Vector2.zero && Vector2.Distance (st.start, st.end) < holdDistance;
		System.Action startHold = () => {
			holdRoutine = HoldDown ();
			StartCoroutine (holdRoutine);
		};
		System.Action stopHold = () => {
			st.holdCount = 0;
			if (holdRoutine != null) {
				StopCoroutine (holdRoutine);
			}
			SetTargetColor (Color.white);
		};

		while (isActive) {
			bool prevIsHold = curIsHold;
			curIsHold = isHold ();

			if (!prevIsHold && curIsHold) {
				startHold();
			} else if (prevIsHold && !curIsHold) {
				handler.OnRelease (st.holdCount);
				stopHold();
			}

			yield return null;
		}
		stopHold ();

		while (!isActive) {
			yield return null;
		}

		StartCoroutine (CheckHoldDown ());
	}

	private IEnumerator HoldDown() {
		st.holdCount = 0;

		System.Action holdEvent = () => {
			st.holdCount++;
			Color effectColor = GetHoldColor();
			StartCoroutine (ripple (Local2Screen(st.end), effectColor));
			SetTargetColor(effectColor);
		};

		yield return new WaitForSeconds (holdCountTime);
		holdEvent ();

		while (true) {
			yield return new WaitForSeconds (holdCountTime);
			holdEvent ();
		}
	}
}
