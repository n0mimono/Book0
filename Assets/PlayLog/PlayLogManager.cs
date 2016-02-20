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
		public int type;
		public string name;
		public string method;
		public Vector3 pos;
		public Vector3 ang;

		public void Execute(PlayLogManager man) {
			if (type == 0) {
				man.Do (name, method);
			} else if (type == 1) {
				man.SyncTransform (name, pos, ang);
			}
		}
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
				iter.Current.Execute (this);
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
			records.Add (new Record () { time = time, type = 0, name = name, method = method });
		}

		Do(name, method);
	}

	public void SyncTransform(string name, Vector3 pos, Vector3 ang) {
		if (!methodDict.ContainsKey (name)) return; // wtf
		
		AssembledObject a = methodDict [name];

		Transform transform = a.obj.transform;
		transform.position = Vector3.Lerp (transform.position, pos, 1f);
		transform.eulerAngles = Vector3.Lerp (transform.eulerAngles, ang, 1f);
	}

	public void RecordTransform(string name, Transform trans) {
		if (isActive) {
			records.Add (new Record () { time = time, type = 1, name = name, pos = trans.position, ang = trans.eulerAngles });
		}
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

	public static void Sync(this RecordableObject obj, Transform trans) {
		if (!PlayLogManager.Instance.IsReady) return;

		if (isRecordMode) {
			PlayLogManager.Instance.RecordTransform (obj.RecordName, trans);
		}

	}

}

public interface RecordableObject {
	string RecordName { get; }
	Transform transform { get; }
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
