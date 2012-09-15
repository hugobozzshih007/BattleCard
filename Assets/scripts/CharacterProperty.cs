using UnityEngine;
using System.Collections;

public class CharacterProperty : MonoBehaviour {
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
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
