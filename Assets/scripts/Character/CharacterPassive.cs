using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility; 

public class CharacterPassive : MonoBehaviour {
	
	public PassiveType[] PassiveAbility;
	public Dictionary<PassiveType, bool> PassiveDict;
	public bool StartDie = false;
	float timeToDie = 1.5f;
	CharacterProperty cp;
	const float seg = 0.003f;
	//int flyTimes = 125;
	int t = 0;
	int f = 0;
	float r = 0.0f;
	bool flying = false;
	RoundUI rUI;
	MainInfoUI infoUI;
	MainUI mUI;
	// Use this for initialization
	void Start () {
		cp = transform.GetComponent<CharacterProperty>();
		PassiveDict = new Dictionary<PassiveType, bool>();
		foreach(PassiveType passive in Enum.GetValues(typeof(PassiveType))){
			PassiveDict.Add(passive, false);
		}
		if((PassiveAbility!=null) && (PassiveAbility.Length>0)){
			foreach(PassiveType p in PassiveAbility){
				PassiveDict[p] = true;
			}
		}
		rUI = Camera.mainCamera.GetComponent<RoundUI>();
		infoUI = Camera.mainCamera.transform.GetComponent<MainInfoUI>();
		mUI = Camera.mainCamera.transform.GetComponent<MainUI>();
	}
	
	void Update(){
		
		if(PassiveDict[PassiveType.Leader]){
			cp.LeadingCharacter = true;
		}else{
			cp.LeadingCharacter = false;
		}
	}
}
