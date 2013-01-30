using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public struct BuffUI{
	public Transform Chess;
	public Rect StartPoint;
	public Dictionary<BuffType, int> BuffDict;
	
	public BuffUI(Transform chess, Dictionary<BuffType, int> buffDict){
		int boxW = 30; int boxH = 40;
		Vector3 screenPos = Camera.main.WorldToScreenPoint(chess.position);
		screenPos.y = Screen.height - screenPos.y;
		Chess = chess;
		BuffDict = buffDict;
		StartPoint = new Rect(screenPos.x-boxW/2+boxW/3, screenPos.y-40-boxH/2,boxW,boxH);
	}
}

public class BuffSlidingUI : MonoBehaviour {
	
	public Font UIFont;
	public IList UIItems;
	float movingSpeed = 0.1f;
	float diffHeight = 0.0f;
	public bool FadeInUI = false;
	bool showUI = false;
	float _textAlpha;
	int delayCounter = 250;
	
	// Use this for initialization
	void Start () {
		UIItems = new List<BuffUI>();
	}
	
	GUIStyle GetGUIStyle(BuffType type){
		GUIStyle s = new GUIStyle();
		s.alignment = TextAnchor.MiddleCenter;
		s.normal.textColor = Color.red;
		s.font = UIFont;
		s.fontSize = 25;
		switch(type){
			case BuffType.Attack:
				s.normal.textColor = Color.red;
				break;
			case BuffType.AttackRange:
				s.normal.textColor = Color.yellow;
				break;
			case BuffType.CriticalHit:
				s.normal.textColor = Color.cyan;
				break;
			case BuffType.Defense:
				s.normal.textColor = Color.green;
				break;
			case BuffType.MoveRange:
				s.normal.textColor = Color.black;
				break;
			case BuffType.SkillRate:
				s.normal.textColor = Color.blue;
				break;
		}
		return new GUIStyle(s);
	}
	
	string GetContent(BuffType type){
		string content = "";
		switch(type){
			case BuffType.Attack:
				content = "atk";
				break;
			case BuffType.AttackRange:
				content = "range";
				break;
			case BuffType.CriticalHit:
				content = "critic";
				break;
			case BuffType.Defense:
				content = "hp";
				break;
			case BuffType.MoveRange:
				content = "move";
				break;
			case BuffType.SkillRate:
				content = "skill";
				break;
		}
		return content;
	}
	
	string GetPlus(int num){
		string plus = "+";
		if(num<0)
			plus = "";
		return plus;
	}
	
	void FadeIn(){
		delayCounter -= 1;
		if(delayCounter<=0){
			showUI = true;
			_textAlpha = Mathf.Lerp(_textAlpha,1,Time.deltaTime*5);
			diffHeight+=movingSpeed;
			if(_textAlpha>=0.9f){
				FadeInUI = false;
				delayCounter = 250;
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
			delayCounter = 250;
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
				foreach(BuffUI bUI in UIItems){
					//int seg = 2;
					int num = 0;
					foreach(var pair in bUI.BuffDict){
						GUI.Box(new Rect(bUI.StartPoint.x-bUI.StartPoint.width*num,bUI.StartPoint.y-diffHeight,bUI.StartPoint.width,bUI.StartPoint.height),GetPlus(pair.Value)+pair.Value.ToString(), GetGUIStyle(pair.Key));
						num += 1;
					}
				}
			}
		}
	}
}
