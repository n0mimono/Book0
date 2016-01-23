using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Custom;

public partial class Creature : MonoBehaviour {

	void Start() {
		Initialize ();
	}

	protected virtual void Initialize() {
		InitializeDamageControl ();

		InitilizeBattleStatus ();
	}

}

public partial class Creature {

	[Serializable]
	public struct BattleStatus {
		public int hitPoint;
	}

	[Header("Status Control")]
	public BattleStatus iniBS;
	public BattleStatus curBS;
	public float HpRate { get { return (float)curBS.hitPoint / (float)iniBS.hitPoint; } }

	public bool IsAlive { get { return curBS.hitPoint > 0; } }
	private void DecreaseHitPoint(int point) {
		if (IsAlive) {
			curBS.hitPoint = Mathf.Max (0, curBS.hitPoint - point);
			if (curBS.hitPoint == 0) {
				DeadHandler.Invoke ();
			}
			HpChangeHander (HpRate);
		}
	}

	protected void InitilizeBattleStatus() {
		curBS = iniBS;
		HpChangeHander (HpRate);
	}

	[Header("Damage Control")]
	public DamageSource damageSource;
	public DamageReceptor damageReceptor;

	public event Action DeadHandler;
	public event Action<float> HpChangeHander;

	protected virtual void InitializeDamageControl() {
		DeadHandler += () => {};
		HpChangeHander += (rate) => {};

		damageReceptor.OnDamage += OnDamage;
	}

	protected virtual void OnDamage(DamageSource src) {
		DecreaseHitPoint (1);
	}

	public void ForceHpUpdate() {
		HpChangeHander.Invoke (HpRate);
	}

}
