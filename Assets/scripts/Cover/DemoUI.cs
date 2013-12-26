using UnityEngine;
using System.Collections;

public class DemoUI : MonoBehaviour {
	public Texture2D BackGround, demo, Continuing, Winning, Team_Editor, Battle, Tutorial; 
	Rect bg = new Rect(0.0f, 0.0f, Screen.width, Screen.height);
	Rect demoRect = new Rect();
	Rect[] ContRect = new Rect[3];
	float _Alpha = 0.0f;
	bool fadeIn = true; 
	bool fadeOut = false;
	float demoWidth = 298.0f;
	float demoHeight = 64.0f;
	bool nextShowUp = false;
	SystemSound sSoundClick; 
	SystemSound sSoundOpen; 
	SystemSound sSoundGame;
	string levelName = ""; 
	bool showLoading = false;
	LoadingFadeIn lf; 
	// Use this for initialization
	void Start () {
		demoRect = new Rect(Screen.width/2.0f - demoWidth/2.0f, 500.0f/720.0f*Screen.height, demoWidth, demoHeight);
		ContRect[0] = new Rect(demoRect.x-demoWidth-25, demoRect.y+100, demoWidth, demoHeight);
		ContRect[1] = new Rect(demoRect.x, demoRect.y+100, demoWidth, demoHeight);
		ContRect[2] = new Rect(demoRect.x+demoWidth+25, demoRect.y+100, demoWidth, demoHeight);
	 	sSoundClick = GameObject.Find("SystemSoundClick").GetComponent<SystemSound>();
		sSoundOpen = GameObject.Find("SystemSoundOpen").GetComponent<SystemSound>();
		sSoundGame = GameObject.Find("SystemSoundGame").GetComponent<SystemSound>();
		lf = GameObject.Find("LoadingScreen").GetComponent<LoadingFadeIn>();
	}
	
	// Update is called once per frame
	void Update () {
		FadeIn();
		FadeOut();
	}
	
	void FadeIn(){
		if(fadeIn){
			_Alpha = Mathf.Lerp(_Alpha,1.0f,Time.deltaTime*4);
			if(_Alpha>=0.9f){
				fadeOut = true;
				fadeIn = false;
			}
		}
	}
	
	void FadeOut(){
		if(fadeOut){
			_Alpha = Mathf.Lerp(_Alpha,0.0f,Time.deltaTime*4);
			if(_Alpha<=0.1f){
				fadeOut = false;
				fadeIn = true;
			}
		}
	}
	
	
	void OnGUI(){
		GUI.depth = 3;
		//GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		GUI.backgroundColor = Color.clear;
		if(!nextShowUp){
			GUI.DrawTexture(bg, BackGround);
			GUI.color = new Color(1.0f, 1.0f, 1.0f, _Alpha);
			GUI.Button(demoRect, demo);
			if(Input.GetMouseButtonDown(0)){
				sSoundGame.PlaySound();
				nextShowUp = true;
			}	
		}else{
			GUI.DrawTexture(bg, Winning);
			GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			if(GUI.Button(ContRect[1], Team_Editor)){
				sSoundClick.PlaySound();
				
				levelName = "team_editor";
				lf.ActivateLoading(levelName);
			}
			if(GUI.Button(ContRect[0], Tutorial)){
				sSoundClick.PlaySound();
				
				levelName = "tutorial_selection";
				lf.ActivateLoading(levelName);
				
			}
			if(GUI.Button(ContRect[2], Battle)){
				sSoundClick.PlaySound();
				
				levelName = "summon_land";
				lf.ActivateLoading(levelName);
			}
		}
	}
}
