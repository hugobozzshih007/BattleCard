using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Sliding FX/Buff")]

public class BuffSlidingFX : MonoBehaviour {
	public Font UIFont;
	public int DCounter = 60;
	Rect startPoint;
	int boxW = 150; int boxH = 40;
	Vector3 screenPos;
	float movingSpeed = 0.1f;
	float diffHeight = 0.0f;
	bool fadeInUI = false;
	bool showUI = false;
	float _textAlpha;
	int delayCounter;
	StatusMachine sMachine; 
	Dictionary<BuffType, int> BuffDict;
	bool soundPlayed = false;
	SystemSound sSound;
	// Use this for initialization
	void Start () {
		BuffDict = new Dictionary<BuffType, int>();
		delayCounter = DCounter;
		sMachine = GameObject.Find("StatusMachine").transform.GetComponent<StatusMachine>();
		sSound = GameObject.Find("SystemSound").transform.GetComponent<SystemSound>();
	}

	//old GUI
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
				s.normal.textColor = Color.magenta;
				break;
			case BuffType.CriticalHit:
				s.normal.textColor = new Color(97.0f/255.0f, 189.0f/255.0f, 1.0f, 1.0f);
				break;
			case BuffType.Defense:
				s.normal.textColor = Color.blue;
				break;
			case BuffType.MoveRange:
				s.normal.textColor = new Color(1.0f, 103.0f/255.0f, 221.0f/255.0f, 1.0f);
				break;
			case BuffType.SkillRate:
				s.normal.textColor = new Color(206.0f/255.0f, 108.0f/255.0f, 41.0f/255.0f, 1.0f);
				break;
			case BuffType.Hp:
				s.normal.textColor = Color.green;
				break;
		}
		return new GUIStyle(s);
	}

	//NGUI
	Color GetGUIColor(BuffType type){
		switch(type){
		case BuffType.Attack:
			return Color.red;
			break;
		case BuffType.AttackRange:
			return  Color.magenta;
			break;
		case BuffType.CriticalHit:
			return  new Color(97.0f/255.0f, 189.0f/255.0f, 1.0f, 1.0f);
			break;
		case BuffType.Defense:
			return  Color.blue;
			break;
		case BuffType.MoveRange:
			return new Color(1.0f, 103.0f/255.0f, 221.0f/255.0f, 1.0f);
			break;
		case BuffType.SkillRate:
			return new Color(206.0f/255.0f, 108.0f/255.0f, 41.0f/255.0f, 1.0f);
			break;
		case BuffType.Hp:
			return Color.green;
			break;
		}
		return Color.white;
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
				content = "def";
				break;
			case BuffType.MoveRange:
				content = "move";
				break;
			case BuffType.Hp:
				content = "hp";
				break;
			case BuffType.SkillRate:
				content = "cmd";
				break;
		}
		return content;
	}
	
	string GetPlus(int num){
		string plus = "+";
		if(num<0)
			plus = "-";
		return plus+num.ToString();
	}
	
	public void ActiveBuffSlidingFX(Dictionary<BuffType, int> buffDict){
		//fadeInUI = true; 
		//BuffDict = buffDict; 
		foreach(var buff in buffDict){
			string init = GetContent((BuffType)buff.Key);
			string val = GetPlus((int)buff.Value);
			Color showColor = GetGUIColor((BuffType)buff.Key);
			transform.GetComponent<CharacterProperty>().UpdateHudText(init+val, showColor);
		}
	}
	
	void FadeIn(){
		delayCounter -= 1;
		if(delayCounter<=0){
			showUI = true;
			_textAlpha = Mathf.Lerp(_textAlpha,1,Time.deltaTime*5);
			diffHeight+=movingSpeed;
			if(_textAlpha>=0.9f){
				fadeInUI = false;
				delayCounter = DCounter;
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
			delayCounter = DCounter;
			diffHeight = 0.0f;
			BuffDict.Clear();
		}
	}
	
	// Update is called once per frame
	void Update () {
		/*
		if(fadeInUI)
			FadeIn();
		else if(showUI)
			FadeOut();
		if(showUI){
			screenPos = Camera.main.WorldToScreenPoint(this.transform.position);
			screenPos.y = Screen.height - screenPos.y;
			startPoint = new Rect(screenPos.x-boxW/2, screenPos.y-40-boxH/2,boxW,boxH);
		}*/
		
	}
	
	void OnGUI(){
		/*
		GUI.color = new Color(1.0f,1.0f,1.0f,_textAlpha);
		GUI.backgroundColor = Color.clear;
		if(sMachine.InitGame){
			if(BuffDict.Count>0 && showUI){
				int num = 0;
				foreach(var pair in BuffDict){
					GUI.Box(new Rect(startPoint.x,startPoint.y+25*num-diffHeight,startPoint.width,startPoint.height),GetContent(pair.Key) + GetPlus(pair.Value)+pair.Value.ToString(), GetGUIStyle(pair.Key));
					num += 1;
				}
			}
		}
		*/
	}
}
