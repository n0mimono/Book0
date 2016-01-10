using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Custom;

public partial class ElasticTouch : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {
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
	[Header("Flick")]
	public float flickTime;
	public float flickDistance;
	public Color flickColor;
	public float elasticDistance;
	public float elasticDotSimilarity;

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
		public CountFireHandler OnHold = (count) => {};
		public CountFireHandler OnRelease = (count) => {};
		public DirectionHandler OnFlicked = (vec) => {};
	}
	public EventHandler handler = new EventHandler();

	public void OnPointerDown (PointerEventData eventData) {
		if (!isActive) return;

		Vector2 pos = LocalPos (eventData.position);
		SetPos (Local2Screen (pos));

		StartCoroutine (ripple (Local2Screen(pos), GetChainColor()));

		st.start = pos;
		st.end   = pos;
		SetTargetAlpha (1f);
		SetStretch (0f);
	}
	public void OnPointerUp (PointerEventData eventData) {
		if (!isActive) return;

		Vector2 pos = LocalPos (eventData.position);
		Vector2 delta = LocalDelta (eventData.position, eventData.delta);
		PushHistory (Local2Screen(delta));

		StartCoroutine (ripple (Local2Screen(pos), GetChainColor()));
		StartCoroutine (FlickAction(Local2Screen(pos), PopHistory(flickTime)));
		handler.OnRelease (st.holdCount);

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
		Vector2 delta = LocalDelta (eventData.position, eventData.delta);
		PushHistory (Local2Screen(delta));

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

	private Vector2 LocalDelta(Vector2 pos, Vector2 delta) {
		Vector2 curPos = LocalPos (pos);
		Vector2 prevPos = LocalPos (pos - delta);
		return curPos - prevPos;
	}

	[ContextMenu("Set Sorting Order Of Touch Effect")]
	public void SetSortingOrderOfTouchEffect() {
		Renderer effectRenderer = touchEffect.GetComponent<Renderer> ();
		effectRenderer.sortingOrder = 1;
	}

	private void SetPos(Vector2 pos) {
		touchEffectRect.localPosition = pos;
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

	private void SetCurrentAlpha(float alpha) {
		cur.alpha = alpha;
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

			handler.OnHold(st.holdCount);
		};

		while (isActive) {
			bool prevIsHold = curIsHold;
			curIsHold = isHold ();

			if (!prevIsHold && curIsHold) {
				startHold();
			} else if (prevIsHold && !curIsHold) {
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

			handler.OnHold(st.holdCount);
		};

		yield return new WaitForSeconds (holdCountTime);
		holdEvent ();

		while (true) {
			yield return new WaitForSeconds (holdCountTime);
			holdEvent ();
		}
	}

	private IEnumerator FlickAction(Vector3 pos, Vector3 vec) {
		Vector3 org = pos - vec;

		bool isWeak  = vec.magnitude < flickDistance;
		bool isDist  = vec.Similarity (st.Vec) < elasticDotSimilarity;
		bool isShort = st.Vec.magnitude < elasticDistance;
		if (isWeak || isDist || isShort) {
			yield break;
		}
		Color effectColor = flickColor;

		StartCoroutine(ripple(org, effectColor));
		StartCoroutine(ripple(pos, effectColor));
		yield return null;
		handler.OnFlicked (vec);

		SetPos (org);
		SetStretch (vec.magnitude * stretchScale);
		SetDirection (vec.normalized);

		SetTargetColor (effectColor);
		SetTargetAlpha (1f);
		SetCurrentAlpha (0f);

		yield return new WaitForSeconds(0.3f);
		SetTargetColor (Color.white);
		SetTargetAlpha (0f);
	}

}

public partial class ElasticTouch {

	private struct TouchPoint {
		public Vector3 vec;
		public float   time;
	}
	private Queue<TouchPoint> touchHistory = new Queue<TouchPoint>();

	private void PushHistory(Vector3 vec) {
		touchHistory.Enqueue(new TouchPoint() { vec = vec, time = Time.time });
	}
	private Vector3 PopHistory(float dt) {
		List<Vector3> curVecs = touchHistory.Where (t => Time.time - t.time < dt).Select(t => t.vec).ToList();
		int count = curVecs.Count;

		if (count == 0) {
			return Vector3.zero;
		}
		float w = 1f / count;
		Vector3 avg = curVecs.Aggregate ((m, v) => m += w * v);

		touchHistory.Clear ();
		return avg;
	}

}
