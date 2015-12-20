// see: http://unityshader.hatenablog.com/entry/2015/12/13/000657

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Toonizer : MonoBehaviour {
	public Vector4 edgeParams;
	public Vector4 edgePowers;

	private Material material;

	void OnRenderImage(RenderTexture src, RenderTexture dst) {
		if (material == null) {
			material = new Material (Shader.Find ("Hidden/ToonEffect"));
		}

		// camera mode
		Camera.main.depthTextureMode = DepthTextureMode.DepthNormals;

		// buffer
		int width = src.width;
		int height = src.height;
		RenderTexture tmp0 = RenderTexture.GetTemporary (width, height, 0, RenderTextureFormat.ARGB32);
		RenderTexture tmp1 = RenderTexture.GetTemporary (width, height, 0, RenderTextureFormat.ARGB32);

		// 2: color
		Graphics.Blit (src, tmp0, material, 2);

		// 0: edge
		material.SetVector ("_EdgeParams", edgeParams);
		material.SetVector ("_EdgePowers", edgePowers);
		Graphics.Blit (src, tmp1, material, 0);

		// 1: composite
		material.SetTexture("_TmpTex", tmp1);
		Graphics.Blit (tmp0, dst, material, 1);

		// release buffer
		RenderTexture.ReleaseTemporary(tmp0);
		RenderTexture.ReleaseTemporary(tmp1);
	}

}
