using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;

public class Tower : MonoBehaviour {
	
	CharacterProperty towerProperty;
	public int Stage; 
	// Use this for initialization
	void Start () {
		towerProperty = transform.GetComponent<CharacterProperty>();
	}
	
	int GetNextLevel(int stageNum){
		int level = 0;
		switch(stageNum){
			case 1: 
				level = 1;
				break;
			case 4: 
				level = 3;
				break;
		}
		return level;
	}
	
	// Update is called once per frame
	void Update () {
		/*
		if(towerProperty.death){
			if(towerProperty.Player ==1)
				Application.LoadLevel(4);
			else if(towerProperty.Player == 2){
				Application.LoadLevel(GetNextLevel(Stage));
			}
		}
		*/
	}
	
	void OnGUI(){
	}
}
