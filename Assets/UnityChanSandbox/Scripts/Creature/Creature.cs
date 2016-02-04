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

	protected virtual void Update() {
		RecoverMagicPoint (Time.deltaTime);
	}

}

public partial class Creature {

	[Serializable]
	public struct BattleStatus {
		public int hitPoint;
		public float magicPoint;
		public float recoverMagicSpeed;
	}

	[Header("Status Control")]
	public BattleStatus iniBS;
	public BattleStatus curBS;
	public float HpRate { get { return (float)curBS.hitPoint / (float)iniBS.hitPoint; } }
	public float MpRate { get { return curBS.magicPoint / iniBS.magicPoint; } }

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
		if (HpChangeHander != null) {
			HpChangeHander (HpRate);
		}
	}

	[Header("Damage Control")]
	public DamageSource damageSource;
	public DamageReceptor damageReceptor;

	public event Action DeadHandler;
	public event Action<float> HpChangeHander;
	public event Action<float> MpChangeHander;

	protected virtual void InitializeDamageControl() {
		DeadHandler += () => {};
		HpChangeHander += (rate) => {};

		damageReceptor.OnDamage += OnDamage;
	}

	protected virtual void OnDamage(DamageSource src) {
		DecreaseHitPoint (src.basePoint);
	}

	public void ForceHpUpdate() {
		HpChangeHander.Invoke (HpRate);
	}

	public void ForceSetHp(int point) {
		curBS.hitPoint = point;
	}

	public void ForceDamage(int point) {
		DecreaseHitPoint (point);
	}

	[Button("ForceDamage", "Suicide", 100000)] public int ButtonSuicide;

	public void RecoverMagicPoint(float dt) {
		if (MpChangeHander != null) {
			float mp = curBS.magicPoint + curBS.recoverMagicSpeed * dt;
			curBS.magicPoint = Mathf.Clamp (mp, 0f, iniBS.magicPoint);
			MpChangeHander (MpRate);
		}
	}

	public bool ConsumeMagicPoint(float point) {
		bool hasMP = curBS.magicPoint >= point;
		if (hasMP) {
			curBS.magicPoint -= point;
			MpChangeHander (MpRate);
		}
		return hasMP;
	}

}
