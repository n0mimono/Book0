using UnityEngine;
using System.Collections;

public static class Common {

	public enum Layer {
		Character = 8,
		Spell = 9,
	}

	public static bool IsLayer(this GameObject obj, Layer layer) {
		return obj.layer == (int)layer;
	}

	public static bool IsPlayerTag(this GameObject obj) {
		return obj.tag == "Player";
	}

	public static bool IsEnemyTag(this GameObject obj) {
		return obj.tag == "Enemy";
	}

	public static bool IsOppositeTo(this GameObject obj, GameObject other) {
		return (obj.IsPlayerTag () && other.IsEnemyTag ())
			|| (obj.IsEnemyTag () && other.IsPlayerTag ());
	}
}
