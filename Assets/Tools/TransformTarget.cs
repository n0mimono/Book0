using UnityEngine;
using System.Collections;

public class TransformTarget : MonoBehaviour {
	public Transform target;
}

public static class TransformTargetExtension {

	public static Transform GetTarget(this Transform trans) {
		TransformTarget transTarget = trans.gameObject.GetComponent<TransformTarget>();

		if (transTarget != null) {
			return transTarget.target;
		} else {
			return trans;
		}
	}

}
