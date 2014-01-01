using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct UnNormalUI{
	public Transform Chess; 
	public Rect StartPoint;
	public CharacterProperty Cp;
	
	public UnNormalUI(Transform chess){
		int boxW = 120; int boxH = 30;
		Vector3 screenPos = Camera.main.WorldToScreenPoint(chess.position);
		screenPos.y = Screen.height - screenPos.y;
		Chess = chess;
		StartPoint = new Rect(screenPos.x-boxW/2, screenPos.y-40-boxH/2,boxW,boxH);
		Cp = chess.GetComponent<CharacterProperty>();
	}
}

public class UnNormalSlidingUI : MonoBehaviour {
	public Font UIFont;
	public IList UIItems;
	GUIStyle smallFloating; 
	float movingSpeed = 0.1f;
	float diffHeight = 0.0f;
	public bool FadeInUI = false;
	bool showUI = false;
	float _textAlpha;
	public int DCounter = 40;
	int delayCounter;
	int seg = 20;
	RoundCounter rc;
	StatusMachine sMachine; 
	
	// Use this for initialization
	void Start () {
		sMachine = GameObject.Find("StatusMachine").transform.GetComponent<StatusMachine>();
		UIItems = new List<UnNormalUI>();
		smallFloating = new GUIStyle();
		smallFloating.alignment = TextAnchor.MiddleCenter;
		smallFloating.normal.textColor = Color.grey;
		smallFloating.font = UIFont;
		smallFloating.fontSize = 18;
		rc = transform.GetComponent<RoundCounter>();
		delayCounter = DCounter;
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
			delayCounter = DCounter;
			diffHeight = 0.0f;
			FadeInUI = true;
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
		if(sMachine.InitGame){
			foreach(Transform gf in rc.AllChesses){
				if(!gf.GetComponent<CharacterProperty>().Death){
					UnNormalUI unUI = new UnNormalUI(gf);
					UIItems.Add(unUI);
				}
			}
			foreach(UnNormalUI unUI in UIItems){
				int uSeg = 0;
				foreach(var pair in unUI.Cp.LastUnStatusCounter){
					if(pair.Value>0){
						GUI.Box(new Rect(unUI.StartPoint.x,unUI.StartPoint.y-seg*uSeg-diffHeight,unUI.StartPoint.width,unUI.StartPoint.height), pair.Key.ToString(),smallFloating);
						uSeg+=1;
					}
				}
			}
			UIItems.Clear();
		}
	}
}
