﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Custom;

public class CreatureFactory : Creature {
	
	public GameObject original;
	public Transform cylinder;

	public Transform parent;
	public Transform spawnPoint;

	public bool autoSpawn;
	public int maxNums;
	public float interval;

	private List<GameObject> objList;

	protected override void Initialize() {
		base.Initialize ();

		objList = new List<GameObject> ();
		if (autoSpawn) {
			AutoSpawn ().While(() => IsAlive).StartBy (this);
		}

		DeadHandler += OnDead;
	}

	public void Spawn() {
		GameObject obj = objList.Where (o => !o.activeInHierarchy).FirstOrDefault ();

		if (obj == null) {
			obj = Instantiate (original);
			objList.Add (obj);

			Creature creature = obj.GetComponent<Creature> ();
			creature.DeadHandler += OnCreatureDead;
		}
		Transform trans = obj.transform;

		trans.SetParent (parent);
		trans.SetFrom (spawnPoint);

		obj.SetActive (true);
	}

	private IEnumerator AutoSpawn() {

		while (true) {

			yield return new Noop ()
				.OnUpdate (() => cylinder.transform.position -= Vector3.up * Time.deltaTime * 2f)
				.WhileInCount (1f).StartBy (this);
			
			yield return new WaitForSeconds (0.2f);
			if (objList.Where (o => o.activeInHierarchy).Count () < maxNums) {
				Spawn ();
			}
			yield return new WaitForSeconds (0.2f);

			yield return new Noop ()
				.OnUpdate (() => cylinder.transform.position += Vector3.up * Time.deltaTime * 2f)
				.WhileInCount (1f).StartBy (this);

			yield return new WaitForSeconds (interval);
		}
	}

	private void OnCreatureDead() {
		OnDamage (damageSource);
	}

	private void OnDead() {
	}

}
