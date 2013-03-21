using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {
	RoundCounter rc;
	Transform summoner;
	// Use this for initialization
	void Start () {
		rc = Camera.mainCamera.GetComponent<RoundCounter>();
	}
	
	public void AfterMove(){
		summoner = rc.playerA;
		CharacterProperty ap = summoner.GetComponent<CharacterProperty>();
		ap.Attacked = false; 
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
