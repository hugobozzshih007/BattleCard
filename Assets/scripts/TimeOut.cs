using UnityEngine;
using System.Collections;

public class TimeOut : MonoBehaviour {
	public bool timeOut = false; 
	bool showUI = false;
	Rect bgRect = new Rect(0,0,Screen.width, Screen.height);
	Rect[] btRect = new Rect[7];
	public Texture2D BG, Restart, Quit, Team_Editor, ToggleGrid,Resume, Tutorial, Battle;
	public bool gridOn;
	StatusMachine sMachine; 
	string currentLevel;
	NameMaps nMaps;
	public bool SummonLand = false;
	GeneralSelection sel; 
	LoadingFadeIn lf;
	// Use this for initialization
	void Start () {
		currentLevel = Application.loadedLevelName; 
		sel = Camera.main.GetComponent<GeneralSelection>();
		lf = GameObject.Find("LoadingScreen").GetComponent<LoadingFadeIn>();
		if(SummonLand){
			nMaps = GameObject.Find("Maps").GetComponent<NameMaps>();
			sMachine = GameObject.Find("StatusMachine").transform.GetComponent<StatusMachine>();
		}
		for(int i=0; i<7; i++){
			btRect[i] = new Rect(100, 100+(Restart.height+10)*i, Restart.width, Restart.height);
		}
	}
	
	public void ActTimeOut(){
		timeOut = true;
	}
	
	void OnGUI(){
		GUI.depth = 0;
		GUI.backgroundColor = Color.clear;
		if(showUI){
			GUI.DrawTexture(bgRect, BG);
			if(GUI.Button(btRect[5], Restart)){
				Application.LoadLevel(currentLevel);
				Time.timeScale = 1;
			}
			if(SummonLand){
				if(GUI.Button(btRect[1], ToggleGrid)){
					if(gridOn){
						nMaps.ToggleGrid(false);
						gridOn = false;
					}else{
						nMaps.ToggleGrid(true);
						gridOn = true;
					}
					showUI = false;
					timeOut = false;
					if(sMachine != null)
						sMachine.InBusy = false;
					Time.timeScale = 1;
				}
			}
			
			if(GUI.Button(btRect[0], Resume)){
				showUI = false;
				timeOut = false;
				if(SummonLand){
					if(sMachine != null)
						sMachine.InBusy = false;
				}
				Time.timeScale = 1;
			}
			if(GUI.Button(btRect[2], Tutorial)){
				lf.ActivateLoading("tutorial_selection");
				//Application.LoadLevel("tutorial_selection");
				Time.timeScale = 1;
			}
			
			if(GUI.Button(btRect[3], Team_Editor)){
				lf.ActivateLoading("team_editor");
				//Application.LoadLevel("team_editor");
				Time.timeScale = 1;
			}
			
			if(GUI.Button(btRect[4], Battle)){
				lf.ActivateLoading("summon_land");
				//Application.LoadLevel("summon_land");
				Time.timeScale = 1;
			}
			
			if(GUI.Button(btRect[6], Quit)){
				Application.Quit();
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape)){
			if(showUI){
				showUI = false;
				timeOut = false;
				if(sMachine != null)
					sMachine.InBusy = false;
				Time.timeScale = 1;
			}else{
				showUI = true;
				timeOut = true;
				if(sMachine != null)
					sMachine.InBusy = true;
				Time.timeScale = 0;
			}
		}
	}
}
