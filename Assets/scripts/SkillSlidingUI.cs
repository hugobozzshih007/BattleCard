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
	int delayCounter;
	public int DCounter = 40;
	const string MsgSuccess = "Succeed!";
	const string MsgFail = "Failed!";
	StatusMachine sMachine;
	
	// Use this for initialization
	void Start () {
		UIItems = new List<SkillUI>();
		smallFloating = new GUIStyle();
		smallFloating.alignment = TextAnchor.MiddleCenter;
		smallFloating.normal.textColor = Color.yellow;
		smallFloating.font = UIFont;
		smallFloating.fontSize = 24;
		delayCounter = DCounter;
		sMachine = GameObject.Find("StatusMachine").transform.GetComponent<StatusMachine>();
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
		
		if(_textAlpha <= 0.3f && showUI){
			sMachine.InBusy = false;
		}
		
		if(_textAlpha <= 0.1f){
			showUI = false;
			delayCounter = DCounter;
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
					GUI.Box(new Rect(skill.StartPoint.x,skill.StartPoint.y-diffHeight,skill.StartPoint.width,skill.StartPoint.height),skill.Msg, smallFloating);
				}
			}
		}
	}
}
