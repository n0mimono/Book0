using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Custom;

public class FieldModel : SingletonBase<FieldModel> {

	public class FieldData {
		public int id;
		public bool isOver;
	}
	public List<FieldData> fieldDataList;

	public FieldModel() {
		fieldDataList = new List<FieldData> ();

		// tmp
		for (int i = 0; i < 5; i++) {
			fieldDataList.Add(new FieldData() { id = i + 1, isOver = false });
		}
	}

	public void SetFieldOver(int id, bool isOver) {
		FieldData fieldData = fieldDataList.Where (f => f.id == id).FirstOrDefault ();
		fieldData.isOver = isOver;
	}

	public bool IsFieldOver(int id) {
		FieldData fieldData = fieldDataList.Where (f => f.id == id).FirstOrDefault ();
		return fieldData.isOver;
	}

}
