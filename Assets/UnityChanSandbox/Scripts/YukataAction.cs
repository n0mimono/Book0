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
		StartAutoSpell ();
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

	private void OnDamage(DamageSource src) {
		YukataAction.LockHandler onCompleted = (act) => {};
		StartLockedAction (AnimeAction.Damaged, onCompleted, true);
		transform.LookAtOnGround (src.transform.position);
	}

	[Header("Targetting")]
	public Transform target;
	public System.Action<Transform> OnSetTarget = (tgt) => {};

	public void SetTarget(Transform target) {
		this.target = target;
		if (enchantress != null) {
			enchantress.target = target;
		}

		OnSetTarget (target);
	}

	private IEnumerator Targettting() {
		yield return null;

		while (true) {
			if (target == null) {
				GameObject tgtObj = gameObject.FindOpposites ()
					.Where (g => g.IsLayer (Common.Layer.Character))
					.FirstOrDefault ();
				if (tgtObj != null) {
					SetTarget (tgtObj.transform);
				}
			}
			yield return null;
		}
	}

	[Header("Enchantress")]
	public EnchantControl enchantress;
	private bool isSpelling = false;

	private void InitilizeEnchantress() {
		Targettting ().StartBy (this);

		if (enchantress == null) return;
		enchantress.Initilize (gameObject.tag);
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

	public void RelaseSpell() {
		isSpelling = false;
		enchantress.Fire ();
		enchantress.Release ();
	}

	private void StartAutoSpell() {
		if (isSpelling) return;

		isSpelling = true;
		AutoSpell ()
			.While (() => isSpelling)
			.StartBy (this);		
	}

	private IEnumerator AutoSpell() {
		yield return null;
	}

}

