using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;

public class Tower : MonoBehaviour {
	
	CharacterProperty towerProperty;
	
	// Use this for initialization
	void Start () {
		towerProperty = transform.GetComponent<CharacterProperty>();
	}
	
	// Update is called once per frame
	void Update () {
		if(towerProperty.death){
			if(towerProperty.Player ==1)
				Application.LoadLevel(3);
			else if(towerProperty.Player == 2)
				Application.LoadLevel(2);
		}
	}
	
	void OnGUI(){
	}
}
