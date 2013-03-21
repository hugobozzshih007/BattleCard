using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct DamageUI{
	public Transform Chess;
	public Transform Attacker;
	public Rect StartPoint; 
	public int Damage;
	
	
	public DamageUI(Transform chess, int damage, Transform attacker){
		RoundUI rUI = Camera.mainCamera.GetComponent<RoundUI>();
		rUI.Wait = true;
		int boxW = 80; int boxH = 40;
		Attacker = attacker;
		Vector3 screenPos = Camera.main.WorldToScreenPoint(chess.position);
		screenPos.y = Screen.height - screenPos.y;
		Chess = chess;
		Damage = damage;
		StartPoint = new Rect(screenPos.x-boxW/2, screenPos.y-40-boxH/2,boxW,boxH);
	}
}

public class DamageSlidingUI : MonoBehaviour {
	public Font UIFont;
	public IList UIItems;
	GUIStyle smallFloating; 
	float movingSpeed = 0.1f;
	float diffHeight = 0.0f;
	public bool FadeInUI = false;
	bool showUI = false;
	bool updateInMove = false;
	float _textAlpha;
	int delayCounter = 120;
	DeathFX dFX;
	
	
	// Use this for initialization
	void Awake () {
		updateInMove = false;
		UIItems = new List<DamageUI>();
		smallFloating = new GUIStyle();
		smallFloating.alignment = TextAnchor.MiddleCenter;
		smallFloating.normal.textColor = Color.green;
		smallFloating.font = UIFont;
		smallFloating.fontSize = 40;
		dFX = transform.GetComponent<DeathFX>();
	}
	
	void FadeIn(){
		showUI = true;
		_textAlpha = Mathf.Lerp(_textAlpha,1,Time.deltaTime*5);
		diffHeight+=movingSpeed;
		if(_textAlpha>=0.9f){
			FadeInUI = false;
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
			showUI = false;
			delayCounter = 120;
			diffHeight = 0.0f;
			foreach(DamageUI ui in UIItems){
				if(ui.Chess.GetComponent<CharacterProperty>().Hp<=0){
					DeathUI dUI = new DeathUI(ui.Chess, ui.Attacker);
				}else{
					RoundUI rUI = Camera.mainCamera.GetComponent<RoundUI>();
					rUI.Wait = false;
				}
			}
			
			UIItems.Clear();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(FadeInUI)
			FadeIn();
		else
			FadeOut();
	}
	
	void OnGUI(){
		GUI.color = new Color(1.0f,1.0f,1.0f,_textAlpha);
		GUI.backgroundColor = Color.clear;
		if(UIItems.Count>0){
			if(showUI){
				foreach(DamageUI dmg in UIItems){
					GUI.Box(new Rect(dmg.StartPoint.x,dmg.StartPoint.y-diffHeight,dmg.StartPoint.width,dmg.StartPoint.height),"-"+dmg.Damage.ToString(), smallFloating);
					if(dmg.Chess.GetComponent<CharacterProperty>().Hp>0)
						updateInMove = true;
					else 
						updateInMove = false;
				}
			}
		}
	}
}
