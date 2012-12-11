using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct DamageUI{
	public Transform Chess;
	public Rect StartPoint; 
	public int Damage;
	
	
	public DamageUI(Transform chess, int damage){
		int boxW = 80; int boxH = 40;
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
	float _textAlpha;
	int delayCounter = 120;
	
	// Use this for initialization
	void Awake () {
		UIItems = new List<DamageUI>();
		smallFloating = new GUIStyle();
		smallFloating.alignment = TextAnchor.MiddleCenter;
		smallFloating.normal.textColor = Color.green;
		smallFloating.font = UIFont;
		smallFloating.fontSize = 40;
	}
	
	void FadeIn(){
		showUI = true;
		_textAlpha = Mathf.Lerp(_textAlpha,1,Time.deltaTime*5);
		diffHeight+=movingSpeed;
		if(_textAlpha>=0.9f){
			FadeInUI = false;
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
				}
			}
		}
	}
}
