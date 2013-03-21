using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;

public class DeathFX : MonoBehaviour {
	CommonFX cFX; 
	IList unDeads;
	public Transform Chess;
	Transform attacker;
	public bool StartDie = false;
	RoundUI rUI;
	MainInfoUI infoUI;
	// Use this for initialization
	void Start () {
		cFX = transform.GetComponent<CommonFX>();
		rUI = transform.GetComponent<RoundUI>();
		infoUI = transform.GetComponent<MainInfoUI>();
		unDeads = new List<DeathUI>();
	}
	
	
	// Update is called once per frame
	void Update () {
	}
}
