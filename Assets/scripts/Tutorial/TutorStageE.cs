using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorStageE : MonoBehaviour {
	public Transform MainCharacter;
	public Texture2D ArrowDownLeft, ArrowDownRight, ArrowUpRight; 
	public Texture2D ArrowDownLeftRoll, ArrowDownRightRoll, ArrowUpRightRoll, Pin; 
	float pauseTime = 3.0f;
	bool pause = false;
	float timeSeg = 0.0f;
	Vector2 mousePos;
	string[] content  = new string[5];
	string[] explain  = new string[9];
	int contentIndex = 0;
	ArrowUI aUI; 
	Rect[] arrowRect = new Rect[9];
	StatusMachine sMachine;
	Texture2D clearBt;
	Rect rectContinue = new Rect();
	bool showUI = false;
	Tutorial tutorMom; 
	MainInfoUI chessUI;
	FollowCam fc;  
	RoundCounter currentRC;
	bool showArrows = false;
	PlaceSummoner pSummoner;
	Transform root; 
	IList arrows = new List<Rect>();
	IList pins = new List<Rect>();
	SystemSound sSound; 
	// Use this for initialization
	void Start () {
		aUI = transform.GetComponent<ArrowUI>();
		tutorMom = transform.GetComponent<Tutorial>();
		rectContinue = tutorMom.GetContinueRect();
		sMachine = GameObject.Find("StatusMachine").GetComponent<StatusMachine>(); 
		sSound = GameObject.Find("SystemSoundB").GetComponent<SystemSound>();
		chessUI = Camera.main.GetComponent<MainInfoUI>();
		currentRC = Camera.main.GetComponent<RoundCounter>();
		pSummoner = GameObject.Find("InitStage").GetComponent<PlaceSummoner>();
		fc = Camera.main.GetComponent<FollowCam>(); 
		root = GameObject.Find("unit_start_point_A").transform;
		arrowRect[0] = new Rect(56.0f/1280.0f*Screen.width, 617.0f/720.0f*Screen.height, 64,64);
		arrowRect[1] = new Rect(484.0f/1280.0f*Screen.width, 521.0f/720.0f*Screen.height, 64,64);
		arrowRect[2] = new Rect(676.0f/1280.0f*Screen.width, 585.0f/720.0f*Screen.height, 64,64);
		arrowRect[3] = new Rect(1151.0f/1280.0f*Screen.width, 614.0f/720.0f*Screen.height, 64,64);
		arrowRect[4] = new Rect(886.0f/1280.0f*Screen.width, 39.0f/720.0f*Screen.height, 64,64);
		arrowRect[5] = new Rect(967.0f/1280.0f*Screen.width, 39.0f/720.0f*Screen.height, 64,64);
		arrowRect[6] = new Rect(1031.0f/1280.0f*Screen.width, 39.0f/720.0f*Screen.height, 64,64);
		arrowRect[7] = new Rect(1095.0f/1280.0f*Screen.width, 39.0f/720.0f*Screen.height, 64,64);
		arrowRect[8] = new Rect(1159.0f/1280.0f*Screen.width, 39.0f/720.0f*Screen.height, 64,64);
		foreach(Rect mRect in arrowRect){
			arrows.Add(mRect);
		}
	}
	
	public void StartPause(float sec){
		pauseTime = sec; 
		pause = true;
	}
	
	void InitTutorStageE(){
		content[0] = "Lesson 9: Knowing Other Informations!";
		content[1] = "In this lesson, you are going to learn about other informations shown on the Screen.";
		content[2] = "You just need to click arrows, and I will tell you what they are pointing.";
		content[3] = "Now you have the basic knowledge of this game, let's goto prepare your team!";
		content[4] = "";
		explain[0] = "The quantity of your territories";
		explain[1] = "The ability data of current selected character";
		explain[2] = "Total rounds left in the game";
		explain[3] = "The quantity of your opponent's territories";
		explain[4] = "The percenteage of your territory in the entire map.";
		explain[5] = "The amount of buff for your certain abilities";
		explain[6] = "The amount of debuff for your certain abilities";
		explain[7] = "The amount of buff your critical hit chances";
		explain[8] = "The amount of debuff your critical hit chances";
		sMachine.TutorialBusy = true;
		MainCharacter = currentRC.playerA;
		tutorMom.ShowContinue(true);
		chessUI.SomeoneTaking(MainCharacter, content[0], false);
		pSummoner.ResetSummoner(false, root, null);
		fc.CamFollowMe(MainCharacter);
		showUI = true;
	}
	
	Texture2D GetArrow(int dir){
		if(dir<3){
			return ArrowDownLeft; 
		}else if(dir==3){
			return ArrowDownRight;
		}else{
			return ArrowUpRight;
		}
	}
	
	Texture2D GetArrowRoll(int dir){
		if(dir<3){
			return ArrowDownLeftRoll; 
		}else if(dir==3){
			return ArrowDownRightRoll;
		}else{
			return ArrowUpRightRoll;
		}
	}
	
	void ShowButtons(){
		for(int i=0; i<9; i++){
			if(arrows.Contains(arrowRect[i])){
				if(arrowRect[i].Contains(mousePos)){
					if(GUI.Button(arrowRect[i],GetArrowRoll(i))){
						sSound.PlaySound(SysSoundFx.CommandClick);
						chessUI.SomeoneTaking(MainCharacter, explain[i], false);
						arrows.Remove(arrowRect[i]);
					}
				}else{
					GUI.Button(arrowRect[i],GetArrow(i));
				}
			}else{
				if(GUI.Button(arrowRect[i],Pin)){
					sSound.PlaySound(SysSoundFx.CommandClick);
					chessUI.SomeoneTaking(MainCharacter, explain[i], false);
				}
			}
		}
		if(arrows.Count == 0){
			tutorMom.ShowContinue(true);
			contentIndex+=1;
			chessUI.SomeoneTaking(MainCharacter, content[contentIndex], false);
			arrows.Add(new Rect());
		}
	}
	
	
	// Update is called once per frame
	void Update () {
		mousePos.x = Input.mousePosition.x;
		mousePos.y = Screen.height-Input.mousePosition.y;
		if(pause){
			timeSeg += Time.deltaTime/pauseTime;
			if(timeSeg >= 0.9f){
				timeSeg = 0.0f; 
				pause = false; 
				InitTutorStageE();
			}
		}
	}
	
	void OnGUI(){
		GUI.depth = 0; 
		GUI.backgroundColor = Color.clear;
		if(showUI){
			if(GUI.Button(rectContinue,clearBt)){
				sSound.PlaySound(SysSoundFx.CommandClick);
				if(contentIndex<2){
					contentIndex+=1;
					chessUI.SomeoneTaking(MainCharacter, content[contentIndex], false);
					sMachine.TutorialBusy = true;
				}else if(contentIndex == 2){
					tutorMom.ShowContinue(false);
					showArrows = true;
				}else if(contentIndex == 3){
					Application.LoadLevel("team_editor");
				}
			}
			if(showArrows)
				ShowButtons();
		}
	}
}
