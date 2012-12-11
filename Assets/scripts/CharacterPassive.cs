using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility; 

public class CharacterPassive : MonoBehaviour {
	
	public PassiveType[] PassiveAbility;
	public Dictionary<PassiveType, bool> PassiveDict;
	
	// Use this for initialization
	void Start () {
		PassiveDict = new Dictionary<PassiveType, bool>();
		foreach(PassiveType passive in Enum.GetValues(typeof(PassiveType))){
			PassiveDict.Add(passive, false);
		}
		if((PassiveAbility!=null) && (PassiveAbility.Length>0)){
			foreach(PassiveType p in PassiveAbility){
				PassiveDict[p] = true;
			}
		}
	
	}
}
