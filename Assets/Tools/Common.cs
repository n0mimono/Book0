using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class Common {

	public enum Layer {
		Character = 8,
		Spell = 9,
	}
	public const string PlayerTag = "Player";
	public const string EnemyTag = "Enemy";

	public static bool IsLayer(this GameObject obj, Layer layer) {
		return obj.layer == (int)layer;
	}

	public static bool IsPlayerTag(this GameObject obj) {
		return obj.tag == PlayerTag;
	}

	public static bool IsEnemyTag(this GameObject obj) {
		return obj.tag == EnemyTag;
	}

	public static bool IsOppositeTo(this GameObject obj, GameObject other) {
		return (obj.IsPlayerTag () && other.IsEnemyTag ())
			|| (obj.IsEnemyTag () && other.IsPlayerTag ());
	}

	public static GameObject[] FindOpposites(this GameObject go) {
		string op = go.IsPlayerTag () ? EnemyTag : PlayerTag;
		return GameObject.FindGameObjectsWithTag (op);
	}
	public static GameObject[] FindOppositeCharacters(this GameObject go) {
		string op = go.IsPlayerTag () ? EnemyTag : PlayerTag;
		return GameObject.FindGameObjectsWithTag (op).Where (g => g.IsLayer (Layer.Character)).ToArray();
	}

}
