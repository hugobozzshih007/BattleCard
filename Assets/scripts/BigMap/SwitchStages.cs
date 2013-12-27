using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SwitchStages : MonoBehaviour {
	public GameObject[] Stage = new GameObject[6];
	public GameObject[] StageSepia = new GameObject[6];
	public GameObject Mainland, MainlandSepia; 
	// Use this for initialization
	void Start () {
		for(int i=0; i<6; i++){
			DeActivateAll(Stage[i]);
			ActivateAll(StageSepia[i]);
		}
		ActivateAll(Stage[0]);
		DeActivateAll(StageSepia[0]);
	}
	
	public void ActivateAll(GameObject asset){
		asset.SetActiveRecursively(true);
	}
	
	public void DeActivateAll(GameObject asset){
		asset.SetActiveRecursively(false);
	}
	
	public void SwitchStage(int selection){
		for(int i=0; i<6; i++){
			DeActivateAll(Stage[i]);
			ActivateAll(StageSepia[i]);
		}
		ActivateAll(Stage[selection]);
		DeActivateAll(StageSepia[selection]);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
