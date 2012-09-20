using UnityEngine;
using System.Collections;

public class CharacterProperty : MonoBehaviour {
	public int Player = 1;
	public int moveRange = 1; 
	public int atkRange = 1;
	public int defPower = 1;
	public int atkPower = 1; 
	public int summonCost = 2;
	public int activeCost = 2;
	public bool character = true;
	public bool death = true;
	public string activeAbility = "";
	public string passiveAbility = "";
	
	// Use this for initialization
	void Start () {
		if(Player==1){
			transform.position = GameObject.Find("unit_start_point_A").transform.position;
			transform.Translate(0.0f,1.5f,0.0f);
		}else if(Player==2){
			transform.position = GameObject.Find("unit_start_point_B").transform.position;
			transform.Translate(0.0f,1.5f,0.0f);
		}
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
