using UnityEngine;
using System.Collections;

public class SimpleJumper : MonoBehaviour, RecordableObject {
	public string RecordName { get { return "Jumper"; } }

	public void Jump() {
		this.Do (JumpBody);
	}

	[RecordableAttribute]
	public void JumpBody() {
		GetComponent<Rigidbody>().AddForce (Vector3.up * 700f);
	}

	void Update() {
		//this.SyncTransform ();
		//this.SyncVariables();
	}

	public void Record() {
		PlayLogManagement.Initialize (true);
		PlayLogManagement.StartRecordOrReplay ();

		PlayLogManagement.Add (this);
	}

	public void Stop() {
		PlayLogManagement.StopRecordOrReplay ();
	}

	public void Replay() {
		PlayLogManagement.Initialize (false);
		PlayLogManagement.StartRecordOrReplay ();

		PlayLogManagement.Add (this);
	}

}
