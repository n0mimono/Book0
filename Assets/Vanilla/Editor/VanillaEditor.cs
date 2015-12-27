using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class VanillaEditor {

	[MenuItem("Assets/Create/Shader/Lit Vanilla")]
	public static void CreateLitVanilla() {
		string filename = "VanillaLight.shader";
		CreateVanilla (filename);
	}

	[MenuItem("Assets/Create/Shader/Unlit Vanilla")]
	public static void CreateUnlitVanilla() {
		string filename = "VanillaUnlit.shader";
		CreateVanilla (filename);
	}

	public static void CreateVanilla(string filename) {
		string curPath = Directory.GetCurrentDirectory ();
		string dirPath = AssetDatabase.GetAssetPath (Selection.activeObject);
		string srcPath = "Assets/Vanilla";

		string srcFullpath = curPath + "/" + srcPath + "/" + filename;
		string dstFullpath = curPath + "/" + dirPath + "/" + filename;

		File.Copy (srcFullpath, dstFullpath);
		AssetDatabase.Refresh ();
	}

}
