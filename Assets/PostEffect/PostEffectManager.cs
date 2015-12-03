using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PostEffectManager : MonoBehaviour {

	[System.Serializable]
	public class EffectMaterial {
		public Material material;
		public bool isEnabled;
	}
	public List<EffectMaterial> materials;

	void OnRenderImage(RenderTexture src, RenderTexture dst) {
		if (!materials.Any (m => m.isEnabled)) {
			Graphics.Blit (src, dst);
			return;
		}

		materials
			.Where (m => m.isEnabled)
			.Select (m => m.material)
			.ToList ().ForEach (m => Graphics.Blit (src, dst, m));
	}
}
