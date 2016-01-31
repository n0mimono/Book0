using UnityEngine;
using System.Collections;

public class BossHpManager : MonoBehaviour {
	public BossHpGauge gauge;
	public Creature creature;

	void Start() {
		creature.HpChangeHander += rate => gauge.SetHpRate (rate);
	}

}
