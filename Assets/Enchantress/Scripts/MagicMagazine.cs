using UnityEngine;
using System.Collections;
using Custom;

public class MagicMagazine : MonoBehaviour {
	public MagicCircle circle;
	public MagicBullet bullet;

	public bool IsLoaded { get { return bullet != null; } }

	public void Initilize(string tag) {
		gameObject.tag = tag;
	}

	public void Load() {
		circle.Hold ();

		bullet = PoolManager.Instance.GetInstance ("MagicBullet").GetComponent<MagicBullet>();
		bullet.gameObject.tag = gameObject.tag;
		bullet.Initialize ();
	}

	public void Unload() {
		circle.Release ();

		bullet.Unload ();
		bullet = null;
	}

	public void Fire(Transform target) {
		circle.Release ();

		bullet.target = target;
		bullet.Fire ();
		bullet = null;
	}

	void Update() {
		if (bullet != null) {
			bullet.transform.position = transform.position;
		}
	}

}
		
