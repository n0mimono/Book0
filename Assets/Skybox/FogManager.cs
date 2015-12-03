using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class FogManager : MonoBehaviour {

	[Range(0,1)] public float lightFactor;
	public Color lightColor;
	public Transform mainLight;

	void Update () {
		Shader.SetGlobalFloat ("global_LightFactor", lightFactor);
		Shader.SetGlobalColor ("global_LightColor", lightColor);

		if (mainLight != null) {
			Vector3 a =  -1 * mainLight.forward ;
			Shader.SetGlobalVector ("global_LightPos", new Vector4 (a.x, a.y, a.z, 1f));
		}
	}

}
