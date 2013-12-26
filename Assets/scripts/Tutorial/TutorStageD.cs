using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorStageD : MonoBehaviour {
	public Transform MainCharacter; 
	RoundCounter currentRC; 
	selection currentSel; 
	bool showUI = false;
	Rect rectContinue = new Rect();
	Tutorial tutorMom; 
	MainInfoUI chessUI;
	CharacterProperty mainP;
	string[] content  = new string[26];
	int contentIndex = 0;
	Texture2D clearBt;
	StatusMachine sMachine; 
	bool subLessonA = false;
	bool subLessonB = false;
	bool subLessonC = false;
	bool pause = false;
	bool[] arrowOn = new bool[10];
	float pauseTime = 3.0f;
	float timeSeg = 0.0f;
	Rect arrowRect = new Rect();
	PlacePrizes pPrize; 
	PlaceSummoner pSummoner;
	FollowCam fc;
	TutorStageE nextStage;
	ArrowUI aUI; 
	Vector3 screenPos;
	Transform root;
	Transform prizeMap;
	SystemSound sSound;
	// Use this for initialization
	void Start () {
		currentRC = Camera.main.GetComponent<RoundCounter>();
		currentSel = Camera.main.GetComponent<selection>();
		tutorMom = transform.GetComponent<Tutorial>();
		rectContinue = tutorMom.GetContinueRect();
		sMachine = GameObject.Find("StatusMachine").GetComponent<StatusMachine>();
		pPrize = GameObject.Find("Maps").GetComponent<PlacePrizes>();
		pSummoner = GameObject.Find("InitStage").GetComponent<PlaceSummoner>();
		aUI = transform.GetComponent<ArrowUI>();
		fc = Camera.main.GetComponent<FollowCam>(); 
		sSound = GameObject.Find("SystemSoundB").GetComponent<SystemSound>();
		nextStage = transform.GetComponent<TutorStageE>();
		chessUI = Camera.main.GetComponent<MainInfoUI>();
		for(int i=0; i<10; i++){
			arrowOn[i] = false;
		}
	}
	
	public void StartPause(float sec){
		pauseTime = sec; 
		pause = true;
	}
	
	void InitTutorStageD(){
		content[0] = "Lesson 8: Open a chance!";
		content[1] = "When there are question marks showed up on the map....";
		content[2] ="you can try to open it with claiming that territory.";
		content[3] = "However, you only can open the question mark that matches your team color.";
		content[4] = "The prize from that question mark can buff your ability for one time only.";
		content[5] = "Now, you can try to open your prize";
		content[6] = "Good job! Now you have to get what came out from question mark.";
		content[7] = "Now, you need to move your character to the prize block!";
		content[8] = "Good job! Just remember that, although you cannot open opponents's prize...";
		content[9] = "you still can get it afer it opened by your opponent.";
		content[10] = "Let's move on to the last tutorial.";
		
		
		IList roundMaps = new List<Transform>();
		MainCharacter = currentRC.playerA;
		mainP = MainCharacter.GetComponent<CharacterProperty>();
		root = GameObject.Find("unit_start_point_A").transform;
		pSummoner.ResetSummoner(false, root, null);
		tutorMom.ShowContinue(true);
		fc.CamFollowMe(MainCharacter);
		chessUI.SomeoneTaking(MainCharacter, content[0], false);
		showUI = true;
		currentSel.chess = null;
		
		Identy rootID = root.GetComponent<Identy>();
		foreach(Transform m in rootID.neighbor){
			if(m!=null){
				roundMaps.Add(m);
			}
		}
		
		int index = Random.Range(0,roundMaps.Count);
		prizeMap = roundMaps[index] as Transform;
		pPrize.PlacePrize(1, prizeMap);
		currentSel.CleanMapsMat();
	}
	
	void UpdateScreenPos(Transform rectRef){
		screenPos = Camera.main.WorldToScreenPoint(rectRef.position);
		screenPos.y = Screen.height - screenPos.y;
	}
	
	bool CheckLessonH(){
		bool check = false;
		Identy mID = prizeMap.GetComponent<Identy>();
		if(!mID.PrizeRed)
			check = true;
		if(!check && mainP.CmdTimes==0){
			mainP.CmdTimes = 3;
			mainP.Attacked = false;
			mainP.TurnFinished = false;
		}
		return check; 
	}
	
	bool CheckLessonI(){
		bool check = false;
		if(pPrize.GetPrizeMap().Count == 0){
			check = true;
		}
		if(!check && mainP.CmdTimes==0){
			mainP.CmdTimes = 3;
			mainP.Attacked = false;
			mainP.TurnFinished = false;
		}
		return check;
	}
	
	void ArrowCheckLessonA(){
		if(arrowOn[1]){
			if(currentSel.chess == MainCharacter){
				arrowOn[1] = aUI.HideArrow();
				UpdateScreenPos(prizeMap);
				arrowRect = new Rect(screenPos.x+25, screenPos.y-105, 64, 64);
				arrowOn[2] = aUI.ShowArrow(arrowRect, ArrowUI.ArrowMode.downLeft);
			}
		}
		if(arrowOn[2]){
			if(tutorMom.GetHitMap() == prizeMap)
				arrowOn[2] = aUI.HideArrow();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(pause){
			timeSeg += Time.deltaTime/pauseTime;
			if(timeSeg >= 0.9f){
				timeSeg = 0.0f; 
				pause = false; 
				InitTutorStageD();
			}
		}
		if(arrowOn[2]){
			screenPos = Camera.main.WorldToScreenPoint(prizeMap.position);
			screenPos.y = Screen.height - screenPos.y;
			arrowRect = new Rect(screenPos.x+25, screenPos.y-105, 64, 64);
			aUI.ShowArrow(arrowRect, ArrowUI.ArrowMode.downLeft);
		}
		if(subLessonA){
			if(CheckLessonH()){
				showUI = true;
				tutorMom.ShowContinue(true);
				mainP.CmdTimes = 0;
				mainP.TurnFinished = true;
				contentIndex+=1;
				chessUI.SomeoneTaking(MainCharacter, content[contentIndex], false);
				subLessonA = false;
				arrowOn[0] = aUI.HideArrow();
				sMachine.TutorialBusy = true;
				currentSel.CancelCmds();
				currentSel.chess = null;
			}
		}
		if(subLessonB){
			ArrowCheckLessonA();
			if(CheckLessonI()){
				showUI = true;
				tutorMom.ShowContinue(true);
				mainP.CmdTimes = 0;
				mainP.TurnFinished = true;
				contentIndex+=1;
				chessUI.SomeoneTaking(MainCharacter, content[contentIndex], false);
				subLessonB = false;
				currentSel.CancelCmds();
				currentSel.chess = null;
				sMachine.TutorialBusy = true;
			}
		}
	}
	
	void OnGUI(){
		GUI.depth = 0;
		GUI.backgroundColor = Color.clear;
		if(showUI){
			if(GUI.Button(rectContinue,clearBt)){
				sSound.PlaySound(SysSoundFx.CommandClick);
				if(contentIndex<5){
					contentIndex+=1;
					chessUI.SomeoneTaking(MainCharacter, content[contentIndex], false);
					sMachine.TutorialBusy = true;
				}else if(contentIndex == 5){
					sMachine.TutorialBusy = false;
					tutorMom.ResetPlayerA(MainCharacter);
					tutorMom.ShowContinue(false);
					chessUI.FadeOutUI();
					UpdateScreenPos(MainCharacter);
					arrowRect = new Rect(screenPos.x+25, screenPos.y-105, 64, 64);
					arrowOn[0] = aUI.ShowArrow(arrowRect, ArrowUI.ArrowMode.downLeft);
					subLessonA = true;
					showUI = false;
				}
				
				if(contentIndex>5 && contentIndex<7){
					sMachine.TutorialBusy = true;
					contentIndex+=1;
					chessUI.SomeoneTaking(MainCharacter, content[contentIndex], false);
				}else if(contentIndex == 7){
					sMachine.TutorialBusy = false;
					tutorMom.ResetPlayerA(MainCharacter);
					tutorMom.ShowContinue(false);
					chessUI.FadeOutUI();
					UpdateScreenPos(MainCharacter);
					arrowRect = new Rect(screenPos.x+25, screenPos.y-105, 64, 64);
					arrowOn[1] = aUI.ShowArrow(arrowRect, ArrowUI.ArrowMode.downLeft);
					subLessonB = true;
					showUI = false;
				}
				
				if(contentIndex>7 && contentIndex<10){
					sMachine.TutorialBusy = true;
					contentIndex+=1;
					chessUI.SomeoneTaking(MainCharacter, content[contentIndex], false);
				}else if(contentIndex == 10){
					DeathUI dUI = new DeathUI(MainCharacter,MainCharacter);
					tutorMom.ResetMap(0);
					tutorMom.ShowContinue(false);
					chessUI.FadeOutUI();
					nextStage.StartPause(3.0f);
					currentSel.CancelCmds();
					showUI = false;
				}
			}
		}
	}
}
