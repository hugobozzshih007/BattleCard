using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Sliding FX/Buff")]

public class BuffSlidingFX : MonoBehaviour {
	StatusMachine sMachine; 
	Dictionary<BuffType, int> BuffDict;
	SystemSound sSound;
	// Use this for initialization
	void Start () {
		BuffDict = new Dictionary<BuffType, int>();
		sMachine = GameObject.Find("StatusMachine").transform.GetComponent<StatusMachine>();
		sSound = GameObject.Find("SystemSound").transform.GetComponent<SystemSound>();
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
		foreach(var buff in buffDict){
			string init = GetContent((BuffType)buff.Key);
			string val = GetPlus((int)buff.Value);
			Color showColor = GetGUIColor((BuffType)buff.Key);
			transform.GetComponent<CharacterProperty>().UpdateHudText(init+val, showColor);
		}
	}
}
