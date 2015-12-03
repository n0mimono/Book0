using UnityEngine;
using System.Collections;

public class CubeFactory : MonoBehaviour {
	public Transform parent;

	[ContextMenu("Create Cube")]
	public void CreateCube() {
		GameObject go = GameObject.CreatePrimitive (PrimitiveType.Cube);

		float x = (2f * Random.value - 1f) * transform.localScale.x / 2f;
		float y = (1f * Random.value - 0f) * transform.localScale.y / 1f;
		float z = (2f * Random.value - 1f) * transform.localScale.z / 2f;
		go.transform.position = new Vector3 (x, y, z);
		go.transform.SetParent (parent);

	}

	[ContextMenu("Create Cube 500")]
	public void CreateCube100() {
		for (int i = 0; i < 500; i++) {
			CreateCube ();
		}
	}
}
