using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Sliding FX/Damage")]

public class DamageSlidingFX : MonoBehaviour {
	
	public Font UIFont;
	Transform attacker;
	GUIStyle smallFloating; 
	float movingSpeed = 0.1f;
	float diffHeight = 0.0f;
	Rect startPoint;
	int currentDmg;
	int boxW = 80; int boxH = 40;
	Vector3 screenPos; 
	bool fadeInUI = false;
	bool showUI = false;
	bool updateInMove = false;
	float _textAlpha;
	public int DCounter = 1;
	int delayCounter = 1;
	DeathFX dFX;
	
	// Use this for initialization
	void Start () {
		updateInMove = false;
		smallFloating = new GUIStyle();
		smallFloating.alignment = TextAnchor.MiddleCenter;
		smallFloating.normal.textColor = Color.green;
		smallFloating.font = UIFont;
		smallFloating.fontSize = 40;
		dFX = transform.GetComponent<DeathFX>();
		delayCounter = DCounter;
	}
	
	public void ActivateSlidingFX(Transform atk, int dmg){
		attacker = atk; 
		currentDmg = dmg; 
		fadeInUI = true; 
		showUI = true;
	}
	
	void FadeIn(){
		showUI = true;
		_textAlpha = Mathf.Lerp(_textAlpha,1,Time.deltaTime*5);
		diffHeight+=movingSpeed;
		if(_textAlpha>=0.9f){
			fadeInUI = false;
			//update npc movement
			if(updateInMove){
				Transform npcPlayer = GameObject.Find("NpcPlayer").transform;
				NpcPlayer npc = npcPlayer.GetComponent<NpcPlayer>();
				npc.InPause = true;
			}
		}
	}
	
	void FadeOut(){
		delayCounter -= 1;
		diffHeight+=movingSpeed;
		if(delayCounter<=0)
			_textAlpha = Mathf.Lerp(_textAlpha,0,Time.deltaTime*5); 
		if(_textAlpha <= 0.1f){
			delayCounter = DCounter;
			diffHeight = 0.0f;
			if(this.GetComponent<CharacterProperty>().Hp<=0){
				DeathUI dUI = new DeathUI(this.transform, attacker);
			}else{
				RoundUI rUI = Camera.mainCamera.GetComponent<RoundUI>();
				rUI.Wait = false;
			}
			showUI = false;
		}
	}
	
	// Update is called once per frame
	void Update () {

		if(showUI){
			//screenPos = Camera.main.WorldToScreenPoint(this.transform.position);
			//screenPos.y = Screen.height - screenPos.y;
			//startPoint = new Rect(screenPos.x-boxW/2, screenPos.y-40-boxH/2,boxW,boxH);
			if(fadeInUI)
				FadeIn();
			else if(showUI)
				FadeOut();
		}


	}
	
	void OnGUI(){
		/*
		GUI.color = new Color(1.0f,1.0f,1.0f,_textAlpha);
		GUI.backgroundColor = Color.clear;
		if(showUI){
			GUI.Box(new Rect(startPoint.x,startPoint.y-diffHeight,startPoint.width,startPoint.height),"-"+currentDmg.ToString(),smallFloating);
			if(this.GetComponent<CharacterProperty>().Hp>0)
				updateInMove = true;
			else 
				updateInMove = false;
		}
		*/
	}
}
