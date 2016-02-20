using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;
using System.IO;
using Custom;

public class PlayLogManager : SingletonMonoBehavior<PlayLogManager> {

	public struct AssembledObject {
		public RecordableObject obj;
		public MethodInfo[] methods;
	}
	public Dictionary<string, AssembledObject> methodDict;

	[Serializable]
	public struct Record {
		public float time;
		public string name;
		public string method;
	}

	[Serializable]
	public class RecordAssemble {
		public List<Record> records;
	}
	public RecordAssemble rassem = new RecordAssemble();
	public List<Record> records { 
		get { return rassem.records; }
		set { rassem.records = value; }
	}

	private bool isIninialized;
	private bool isActive;
	private float time;

	public bool IsReady { get { return isIninialized && isActive; } }

	public void Initialize() {
		if (isIninialized) {
			methodDict.Clear ();
			records.Clear ();
		}
		methodDict = new Dictionary<string, AssembledObject> ();
		records = new List<Record> ();

		isIninialized = true;
	}

	public void StartRecord() {
		isActive = true;
		StartCoroutine (ProcRecord ());
	}

	public void StartReplay() {
		isActive = true;
		StartCoroutine (ProcReplay ());
	}

	public void Stop() {
		isActive = false;
	}

	private IEnumerator ProcRecord() {
		time = 0f;

		while (isActive) {
			yield return null;

			time += Time.deltaTime;
		}
	}

	private IEnumerator ProcReplay() {
		time = 0f;

		var iter = records.GetEnumerator ();
		bool hasNext = iter.MoveNext ();

		while (isActive) {

			while (hasNext && iter.Current.time <= time) {
				Do (iter.Current.name, iter.Current.method);
				hasNext = iter.MoveNext ();
			}

			yield return null;

			time += Time.deltaTime;
		}

	}

	public void Do(string name, string method) {
		Debug.Log ("Play: " + name + "." + method + "@" + time);

		AssembledObject a = methodDict [name];
		MethodInfo methodInfo = a.methods
			.Where (m => m.Name == method)
			.FirstOrDefault ();

		methodInfo.Invoke(a.obj, null);
	}

	public void DoWithRecord(string name, string method) {
		if (isActive) {
			Debug.Log ("Record: " + name + " => " + method + "@" + time);
			records.Add (new Record () { time = time, name = name, method = method });
		}

		Do(name, method);
	}

	public void Add(RecordableObject obj) {
		Type type = obj.GetType ();
		MethodInfo[] methods = type
			.GetMethods (BindingFlags.Instance | BindingFlags.Public)
			.Where (m => Attribute.GetCustomAttribute (m, typeof(RecordableAttribute)) != null)
			.ToArray ();

		methodDict.Add (obj.RecordName, new AssembledObject() { obj = obj, methods = methods });
	}

}

public static class PlayLogManagement {
	private static bool isRecordMode = true;

	public static void Initialize(bool isRecord) {
		isRecordMode = isRecord;

		PlayLogManager.Instance.Initialize ();
	}

	public static void StartRecordOrReplay() {
		if (isRecordMode) {
			PlayLogManager.Instance.StartRecord ();
		} else {
			PlayLogResourceManager.Instance.Load (PlayLogManager.Instance.rassem);

			PlayLogManager.Instance.StartReplay ();
		}
	}

	public static void StopRecordOrReplay() {
		PlayLogManager.Instance.Stop ();

		if (isRecordMode) {
			PlayLogResourceManager.Instance.Save (PlayLogManager.Instance.rassem);
		}
	}

	public static void Add(RecordableObject obj) {
		PlayLogManager.Instance.Add (obj);
	}

	public static void Do(this RecordableObject obj, Action action) {
		bool isRecordable = Attribute.GetCustomAttribute (action.Method, typeof(RecordableAttribute)) != null;

		if (!PlayLogManager.Instance.IsReady || !isRecordable) {
			action ();
			return;
		}

		if (isRecordMode) {
			string method = action.Method.Name;
			PlayLogManager.Instance.DoWithRecord (obj.RecordName, method);
		}
	}

}

public interface RecordableObject {
	string RecordName { get; }
}

public class RecordableAttribute : Attribute {
	public RecordableAttribute() {
	}
}

public class PlayLogResourceManager : SingletonBase<PlayLogResourceManager> {

	private string FilePath { get { return Application.persistentDataPath + "/rec.json"; } }

	public void Save(PlayLogManager.RecordAssemble rassem) {
		string json = JsonUtility.ToJson (rassem);
		File.WriteAllText (FilePath, json);

		Debug.Log ("Save: " + json);
	}

	public void Load(PlayLogManager.RecordAssemble rassem) {
		string json = File.ReadAllText(FilePath);
		PlayLogManager.RecordAssemble r = 
			JsonUtility.FromJson<PlayLogManager.RecordAssemble> (json);

		rassem.records = r.records;

		Debug.Log ("Load: " + json);
	}

}
