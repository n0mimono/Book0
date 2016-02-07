using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Custom;

public class FieldDirector : MonoBehaviour {
	public int id;
	public Creature creature;

	void Start() {
		creature.DeadHandler += OnFieldOver;
	}

	void OnDestroy() {
		creature.DeadHandler -= OnFieldOver;
	}

	private void OnFieldOver() {
		FieldModel.Instance.SetFieldOver (id, true);
	}

}
