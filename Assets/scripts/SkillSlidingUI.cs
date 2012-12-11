using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct SkillUI{
	public Transform Chess;
	public Rect StartPoint; 
	public bool Success;
	public string Msg;
	
	public SkillUI(Transform chess, bool success, string msg){
		int boxW = 160; int boxH = 18;
		Vector3 screenPos = Camera.main.WorldToScreenPoint(chess.position);
		screenPos.y = Screen.height - screenPos.y;
		Chess = chess;
		Success = success;
		Msg = msg;
		StartPoint = new Rect(screenPos.x-boxW/2, screenPos.y-16-boxH/2,boxW,boxH);
	}
}


public class SkillSlidingUI : MonoBehaviour {
	
	public Font UIFont;
	public IList UIItems;
	GUIStyle smallFloating; 
	float movingSpeed = 0.1f;
	float diffHeight = 0.0f;
	public bool FadeInUI = false;
	bool showUI = false;
	float _textAlpha;
	int delayCounter = 120;
	
	const string MsgSuccess = "Succeed!";
	const string MsgFail = "Failed!";
	
	
	// Use this for initialization
	void Start () {
		UIItems = new List<SkillUI>();
		smallFloating = new GUIStyle();
		smallFloating.alignment = TextAnchor.MiddleCenter;
		smallFloating.normal.textColor = Color.red;
		smallFloating.font = UIFont;
		smallFloating.fontSize = 16;
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
				foreach(SkillUI skill in UIItems){
					if(skill.Success)
						GUI.Box(new Rect(skill.StartPoint.x,skill.StartPoint.y-diffHeight,skill.StartPoint.width,skill.StartPoint.height),MsgSuccess+" "+skill.Msg, smallFloating);
					else
						GUI.Box(new Rect(skill.StartPoint.x,skill.StartPoint.y-diffHeight,skill.StartPoint.width,skill.StartPoint.height),MsgFail+" "+skill.Msg, smallFloating);
				}
			}
		}
	}
}
