using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Custom;

public partial class YukataAction : MonoBehaviour {
	public CharacterController characterControl;
	public Animator animator;

	public float maxSpeed;
	public float diveSpeed;
	public float acceleration;
	public float charSpeedScale;
	public float animSpeedScale;

	private List<UnityChanLocoSD> stateMachines;

	private bool isWalkable;
	private Vector3 tgtVelocity;
	private Vector3 curVelocity;

	public enum AnimeAction {
		None    = 0,
		Salute  = 1,
		Jump    = 2,
		Dive    = 3,
		Damaged = 4,
	}
	private AnimeAction inAction;
	public AnimeAction InAction { get { return inAction; } }
	public bool Is(AnimeAction act) { return InAction == act; }

	public delegate void LockHandler(AnimeAction act);
	public event LockHandler InLockAction;
	public event LockHandler OutLockAction;

	void Start() {
		InitilizeActions ();

		InitilizeDamagers ();
		InitilizeEnchantress ();
	}

	private void InitilizeActions() {
		InLockAction += (act) => {
			SetWalkable (false);
			inAction = act;
		};
		OutLockAction += (act) => {
			SetWalkable (true);
			inAction = AnimeAction.None;
		};

		stateMachines = animator.GetBehaviours<UnityChanLocoSD> ().ToList();

		SetWalkable (true);
	}

	void Update() {
		UpdateVelocity ();
	}

	private void UpdateVelocity() {
		// move control
		curVelocity = Vector3.Lerp (curVelocity, tgtVelocity, acceleration * Time.deltaTime);

		// position
		characterControl.Move (charSpeedScale * curVelocity * Time.deltaTime);

		// angle
		if (curVelocity.magnitude > 0.1f) {
			Transform trans = transform;
			trans.LookAt (trans.position + tgtVelocity);
		}

		// animation control
		float animSpeed = isWalkable ? curVelocity.magnitude * animSpeedScale : 0f;
		animator.SetSpeed (animSpeed);
	}

	private void SetTargetVelocity(Vector3 tgtVelocity) {
		tgtVelocity = tgtVelocity.normalized * Mathf.Min (maxSpeed, tgtVelocity.magnitude);;
		tgtVelocity = tgtVelocity.magnitude < 0.2f ?  Vector3.zero : tgtVelocity;
		this.tgtVelocity = tgtVelocity;
	}

	private void SetDiveVelocity(Vector3 dir) {
		transform.forward = dir;
		tgtVelocity = dir * diveSpeed;
	}

	private void ForceStop() {
		tgtVelocity = Vector3.zero;
		curVelocity = Vector3.zero;
		UpdateVelocity ();
	}

	private void StartLockedAction(AnimeAction act, LockHandler onCompleted, bool isForceStop = false) {
		if (isForceStop) {
			ForceStop ();
		}

		// anime
		InLockAction(act);
		stateMachines.ForEach (m => m.OnExit = (hash) => {
			OutLockAction(act);
			onCompleted(act);
		});
		animator.SetAnimeAction ((int)act);
	}

	private void SetWalkable(bool isWalkable) {
		this.isWalkable = isWalkable;
	}

}

public partial class YukataAction {

	public void Move(Vector3 velocity) { 
		SetTargetVelocity (velocity);
	}

	public void Dive(Vector3 dir, LockHandler onCompleted) {
		StartLockedAction (AnimeAction.Dive, onCompleted, false);
		SetDiveVelocity (dir);
	}

	public void SpellFlower(LockHandler onCompleted) {
		StartLockedAction (AnimeAction.Salute, onCompleted, true);
		StartGuardianMagic ();
	}

}

public partial class YukataAction {
	
	[Header("Damage Control")]
	public DamageSource damageSource;
	public DamageReceptor damageReceptor;

	private void InitilizeDamagers() {
		damageSource.IsDamageable = () => Is (AnimeAction.Dive);
		damageReceptor.IsDamageable = () => !Is (AnimeAction.Damaged);
		damageReceptor.OnDamage += OnDamage;
	}

	private void BodyDown() {
		YukataAction.LockHandler onCompleted = (act) => {};
		StartLockedAction (AnimeAction.Damaged, onCompleted, true);
	}

	private void OnDamage(DamageSource src) {
		//Debug.Log (src.name + " > " + src.gameObject.tag + " => " + gameObject.IsOppositeTo(src.gameObject));
		CancelActions ();

		BodyDown();
		transform.LookAtOnGround (src.transform.position);
	}

	private void CancelActions() {
		if (enchantress != null) StopSpell();
	}

	[Header("Targetting")]
	public Transform target;
	public System.Action<Transform> OnSetTarget = (tgt) => {};

	public void SetTargetList(List<Transform> targets) {
		if (enchantress != null) {
			enchantress.SetTarget (targets);
		}
	}

	public void SetTarget(Transform target) {
		this.target = target;
		if (enchantress != null) {
			enchantress.SetTarget (target);
		}

		OnSetTarget (target);
	}

	private IEnumerator Targettting() {
		yield return null;


		while (true) {
			List<Transform> targetList = gameObject.FindOppositeCharacters ().Select (g => g.transform).ToList ();
			SetTargetList (targetList);
			Transform front = transform.WhichInFront (targetList);

			if (front != null) {
				SetTarget (front);
			}

			yield return new WaitForSeconds(1f);
		}
	}

	[Header("Magic")]
	public EnchantControl enchantress;
	public MagicShieldSpawner guardian;
	private bool isSpelling = false;
	private bool isGuarding = false;

	private void InitilizeEnchantress() {
		Targettting ().StartBy (this);

		if (enchantress != null) {
			enchantress.Initilize (gameObject.tag);
		}
		if (guardian != null) {
			guardian.Initilize (gameObject.tag);
		}
	}

	public void StartSpell() {
		if (isSpelling) return;

		isSpelling = true;
		Spell ()
			.While (() => isSpelling)
			.StartBy (this);
	}

	private IEnumerator Spell() {
		enchantress.Hold ();
		while (true) {
			yield return new WaitForSeconds(1f);
			enchantress.Load ();
		}
	}

	public void StopSpell() {
		isSpelling = false;
		enchantress.Unload ();
		enchantress.Release ();
	}

	public void ReleaseSpell() {
		isSpelling = false;
		enchantress.Fire (EnchantControl.TargetMode.Single);
		enchantress.Release ();
	}

	private void StartGuardianMagic() {
		if (isGuarding) return;
		isGuarding = true;

		GuardianMagic ().StartBy (this);		
	}

	private IEnumerator GuardianMagic() {
		yield return null;
		guardian.transform.position = transform.position;
		guardian.Load();

		yield return new WaitForSeconds(2f);
		BodyDown ();

		yield return new WaitForSeconds(10f);
		guardian.Fire(null);

	    yield return new WaitForSeconds(2f);
		isGuarding = false;
	}

}

