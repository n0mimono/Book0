using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class VanillaEditor {

	[MenuItem("Assets/Create/Shader/Lit Vanilla")]
	public static void CreateLitVanilla() {
		string curPath = Directory.GetCurrentDirectory ();
		string dirPath = AssetDatabase.GetAssetPath (Selection.activeObject);
		string filename = "VanillaLight.shader";
		string srcPath = "Assets/Vanilla";

		string srcFullpath = curPath + "/" + srcPath + "/" + filename;
		string dstFullpath = curPath + "/" + dirPath + "/" + filename;

		File.Copy (srcFullpath, dstFullpath);
		AssetDatabase.Refresh ();
	}

}
