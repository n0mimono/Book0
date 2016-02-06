using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Custom;

public class AngelAltar : MonoBehaviour {

	[Header("Altar")]
	public GameObject photographer;

	public Transform target;
	public System.Action OnProcCompleted; 

	public virtual void StartProc() {
	}

}
