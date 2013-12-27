using UnityEngine;
using System.Collections;
using MapUtility;
using System.Collections.Generic;

public class TutorStageA : MonoBehaviour {
	bool showUI = false;
	public Transform MainCharacter;
	Tutorial tutorMom; 
	MainInfoUI chessUI;
	CharacterProperty mainP;
	CharacterSelect mainS;
	bool pause = false;
	string[] content  = new string[15];
	int contentIndex = 0;
	Texture2D clearBt;
	GeneralSelection currentSel; 
	StatusMachine sMachine; 
	RoundCounter currentRC;
	TutorStageB nextStage;
	Vector3 screenPos;
	Transform root; 
	int oldAMaps = 0;
	float timeSeg = 0.0f;
	IList moveList = new List<Transform>();
	bool subLessonA = false;
	bool subLessonB = false;
	bool[] arrowOn = new bool[10];
	Rect arrowRect = new Rect();
	ArrowUI aUI;
	IList roundMaps = new List<Transform>();
	PlaceSummoner pSummoner;
	float pauseTime = 1.0f;
	SystemSound sSound;
	// Use this for initialization
	void Start () {
		sSound = GameObject.Find("SystemSoundB").GetComponent<SystemSound>();
	}
	
	public void StartPause(float sec){
		pauseTime = sec;
		pause = true;
	}
	
	public void InitTutorStageA(){
		content[0] = "Welcome to tutorials";
		content[1] = "Lesson 1: How to move your characters";
		content[2] = "Click the character that you want to move";
		content[3] = "and click the destination among the highlight blocks";
		content[4] = "Now! Please try it!";
		content[5] = "Great job! Now, let's go to lesson 2";
		content[6] = "Lesson 2: How to extend your territory";
		content[7] = "Press your character and hold till the defense UI finishes!";
		content[8] = "Now, please try it!";
		content[9] = "Great job! Now, let's go to lesson 2.1";
		content[10] = "Lesson 2.1: Leader can extend more territories!";
		content[11] = "Leader is a passive ability, and the character with it...";
		content[12] = "can extend the territory under it and around it.";
		content[13] = "However, other characters only can extend the territory under them";
		content[14] = "Now we will teach you how to fight! Let's go to lesson 3";
		pSummoner =  GameObject.Find("InitStage").GetComponent<PlaceSummoner>();
		for(int i=0; i<10; i++){
			arrowOn[i] = false;
		}
		
		aUI = transform.GetComponent<ArrowUI>();
		
		clearBt = new Texture2D(20,20);
		sMachine = GameObject.Find("StatusMachine").GetComponent<StatusMachine>();
		sMachine.TutorialBusy = true;
		currentRC = Camera.main.GetComponent<RoundCounter>();
		MainCharacter = currentRC.playerA;
		currentSel = Camera.main.GetComponent<GeneralSelection>();
		chessUI = Camera.main.GetComponent<MainInfoUI>();
		tutorMom = transform.GetComponent<Tutorial>();
		chessUI.SomeoneTaking(MainCharacter, content[0], false);
		mainP = MainCharacter.GetComponent<CharacterProperty>();
		mainS = MainCharacter.GetComponent<CharacterSelect>();
		nextStage = transform.GetComponent<TutorStageB>();
		tutorMom.ShowContinue(true);
		root = GameObject.Find("unit_start_point_A").transform;
		currentSel.updateMapSteps();
		mainS.findMoveRange(root, 0, mainP.moveRange);
		showUI = true;
		foreach(Transform m in mainS.MoveRangeList){
			moveList.Add(m);
		}
		if(moveList.Contains(root))
			moveList.Remove(root);
		pSummoner.ResetSummoner(false,root,null);
	}
	
	bool CheckLessonA(){
		bool check = false;
		if(!sMachine.InBusy && mainP.CmdTimes<3){
			//Transform currentSpot = mainS.getMapPosition(); 
			foreach(Transform map in moveList){
				if(MapHelper.IsMapOccupied(map)){
					check = true; 
					break;
				}
			}
		}
		if(mainP.CmdTimes==0 && !check){
			mainP.CmdTimes = 3;
			mainP.TurnFinished = false;
		}
		return check;
	}
	
	bool CheckLessonB(){
		bool check = false;		
		Transform localMap = mainS.getMapPosition();
		if(!sMachine.InBusy && mainP.CmdTimes<3){
			if((currentRC.PlayerATerritory.Count - oldAMaps)>2){
				check = true;
				arrowOn[2] = aUI.HideArrow();
			}
		}
		if(mainP.CmdTimes==0 && !check){
			mainP.CmdTimes = 3;
			mainP.TurnFinished = false;
		}
		return check; 
	}
	
	void ArrowCheckLessonA(){
		Transform pointedMap = null;
		
		if(arrowOn[0]){
			if(currentSel.chess == MainCharacter){
				arrowOn[0] = aUI.HideArrow();
				currentSel.updateMapSteps();
				mainS.MoveRangeList.Clear();
				mainS.findMoveRange(mainS.getMapPosition(), 0, mainP.BuffMoveRange);
				foreach(Transform m in mainS.MoveRangeList){
					roundMaps.Add(m);
				}
				mainS.MoveRangeList.Clear();
				int mID = Random.Range(0, roundMaps.Count);
				pointedMap = roundMaps[mID] as Transform;
				UpdateScreenPos(pointedMap);
				arrowRect = new Rect(screenPos.x+10, screenPos.y-70, 64, 64);
				arrowOn[1] = aUI.ShowArrow(arrowRect, ArrowUI.ArrowMode.downLeft);
			}
		}
		if(arrowOn[1]){
			Transform hitMap = tutorMom.GetHitMap(); 
			if(roundMaps.Contains(hitMap)){
				arrowOn[1] = aUI.HideArrow();
			}
		}
	}
	
	void UpdateScreenPos(Transform rectRef){
		screenPos = Camera.main.WorldToScreenPoint(rectRef.position);
		screenPos.y = Screen.height - screenPos.y;
	}
	
	// Update is called once per frame
	void Update () {
		if(pause){
			timeSeg += Time.deltaTime/pauseTime;
			if(timeSeg >= 0.9f){
				timeSeg = 0.0f; 
				pause = false; 
				InitTutorStageA();
			}
		}
		if(arrowOn[2]){
			screenPos = Camera.main.WorldToScreenPoint(MainCharacter.position);
			screenPos.y = Screen.height - screenPos.y;
		}
		if(subLessonA){
			ArrowCheckLessonA();
			if(CheckLessonA()){
				showUI = true;
				tutorMom.ShowContinue(true);
				mainP.CmdTimes = 0;
				mainP.TurnFinished = true;
				contentIndex+=1;
				chessUI.SomeoneTaking(MainCharacter, content[contentIndex], false); 
				subLessonA = false;
				currentSel.CancelCmds();
				sMachine.TutorialBusy = true;
			}
		}
		if(subLessonB){
			if(CheckLessonB()){
				showUI = true;
				tutorMom.ShowContinue(true);
				mainP.CmdTimes = 0;
				mainP.TurnFinished = true;
				contentIndex+=1;
				chessUI.SomeoneTaking(MainCharacter, content[contentIndex], false);
				subLessonB = false;
				currentSel.CancelCmds();
				sMachine.TutorialBusy = true;
			}
		}
	}
	
	void OnGUI(){
		GUI.depth = 0;
		GUI.backgroundColor = Color.clear;
		if(showUI){
			if(GUI.Button(tutorMom.GetContinueRect(),clearBt)){
				sSound.PlaySound(SysSoundFx.CommandClick);
				if(contentIndex<4){
					contentIndex+=1;
					sMachine.TutorialBusy= true;
					chessUI.SomeoneTaking(MainCharacter, content[contentIndex], false);
				}else if(contentIndex == 4){
					tutorMom.ResetPlayerA(MainCharacter);
					tutorMom.ShowContinue(false);
					chessUI.FadeOutUI();
					subLessonA = true;
					sMachine.TutorialBusy = false;
					UpdateScreenPos(MainCharacter);
					arrowRect = new Rect(screenPos.x+25, screenPos.y-105, 64, 64);
					arrowOn[0] = aUI.ShowArrow(arrowRect, ArrowUI.ArrowMode.downLeft);
					showUI = false;
				}
				if(contentIndex>4 && contentIndex<8){
					contentIndex+=1;
					sMachine.TutorialBusy= true;
					chessUI.SomeoneTaking(MainCharacter, content[contentIndex], false);
				}else if(contentIndex == 8){
					tutorMom.ResetPlayerA(MainCharacter);
					sMachine.TutorialBusy = false;
					tutorMom.ShowContinue(false);
					chessUI.FadeOutUI();
					subLessonB = true;
					UpdateScreenPos(MainCharacter);
					arrowRect = new Rect(screenPos.x+25, screenPos.y-105, 64, 64);
					arrowOn[2] = aUI.ShowArrow(arrowRect, ArrowUI.ArrowMode.downLeft);
					showUI = false;
					oldAMaps = currentRC.PlayerATerritory.Count;
				}
				if(contentIndex>8 && contentIndex<14){
					sMachine.TutorialBusy = true;
					contentIndex+=1;
					chessUI.SomeoneTaking(MainCharacter, content[contentIndex], false);
				}else if(contentIndex == 14){
					DeathUI dUI = new DeathUI(MainCharacter,MainCharacter);
					tutorMom.ResetMap(0);
					tutorMom.ShowContinue(false);
					nextStage.StartPause(3.0f);
					currentSel.CancelCmds();
					showUI = false;
				}
			}
		}
	}
}
