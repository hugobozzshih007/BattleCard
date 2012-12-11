using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NameMaps : MonoBehaviour {
	
	// Use this for initialization
	void Awake() {
		Transform allMap = GameObject.Find("Maps").transform;
		IList allUnits = new List<Transform>();
		int ac = allMap.childCount;
		for(int i=0;i<ac;i++){
			if(allMap.GetChild(i).name == "unit"){
				allUnits.Add(allMap.GetChild(i));
			}
		}
		int gg = 0;
		foreach(Transform child in allUnits){
			child.name = child.name + gg;
			gg+=1;
		}
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
