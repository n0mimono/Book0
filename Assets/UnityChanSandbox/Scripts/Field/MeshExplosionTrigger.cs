using UnityEngine;
using System.Collections;

public class MeshExplosionTrigger : MonoBehaviour {

	public void Explosion() {
		BroadcastMessage("Explode");
		gameObject.SetActive (false);
	}

}
