using UnityEngine;
using System.Collections;
using MapUtility;
using System.Collections.Generic;

public class TutorStageB : MonoBehaviour {
	
	bool showUI = false;
	Rect rectContinue = new Rect();
	public Transform MainCharacter;
	public Transform Opponent;
	public Transform OppMap;
	Transform opMap;
	Tutorial tutorMom; 
	MainInfoUI chessUI;
	CharacterProperty mainP, oppP;
	CharacterSelect mainS, oppS;
	string[] content  = new string[26];
	int contentIndex = 0;
	Texture2D clearBt;
	GeneralSelection currentSel; 
	StatusMachine sMachine; 
	RoundCounter currentRC;
	Transform root, rootB; 
	IList moveList = new List<Transform>();
	bool subLessonA = false;
	bool subLessonB = false;
	bool subLessonC = false;
	bool pause = false;
	bool[] arrowOn = new bool[10];
	float pauseTime = 3.0f;
	float timeSeg = 0.0f;
	
	Rect arrowRect = new Rect();
	
	PlaceSummoner pSummoner;
	FollowCam fc;
	TutorStageC nextStage;
	ArrowUI aUI; 
	Vector3 screenPos;
	SystemSound sSound;
	// Use this for initialization
	void Start () {
		clearBt = new Texture2D(20,20);
		currentSel = Camera.main.GetComponent<GeneralSelection>();
		currentRC = Camera.main.GetComponent<RoundCounter>();
		sMachine = GameObject.Find("StatusMachine").GetComponent<StatusMachine>();
		tutorMom = transform.GetComponent<Tutorial>();
		fc = Camera.main.GetComponent<FollowCam>();
		rectContinue = tutorMom.GetContinueRect();
		pSummoner = GameObject.Find("InitStage").GetComponent<PlaceSummoner>();
		chessUI = Camera.main.GetComponent<MainInfoUI>();
		for(int i=0;i<10;i++){
			arrowOn[i] = false;
		}
		nextStage = transform.GetComponent<TutorStageC>();
		sSound = GameObject.Find("SystemSoundB").GetComponent<SystemSound>();
	}
	
	public void StartPause(float sec){
		pauseTime = sec;
		pause = true;
	}
	
	public void InitTutorStageB(){
		content[0] = "Lesson 3: Battle!";
		content[1] = "To start a battle is simple.";
		content[2] = "When an enemy is in your attack range, and you want to attack it...";
		content[3] = "just click your character first and then choose the enemy that you wanna attack.";
		content[4] = "Now, please try! ";
		content[5] = "Good job! You may notice that the enemy also fights back.";
		content[6] = "As long as the enemy can hit you, after damaging it, it will fight back.";
		content[7] = "However, you should know that a melee attacker cannot attack an enemy can fly.";
		content[8] = "Let's go to next lesson";
		content[9] = "Lesson 4: Act Points.";
		content[10] = "In each turn, every character has three act points shown as green bar under it.";
		content[11] = "In each turn, each character can only attack once..."; 
		content[12] = "but no limitation on moving and extending territory.";
		content[13] = "After running out all of your act points on every character...";
		content[14] = "Your turn is over and becomes your opponent's.";
		content[15] = "Now, try to move once, attack once, and extend once.";
		content[16] = "Great job! Let's move on!";
		content[17] = "Lesson 5: More territories you own, more buff on your characters!";
		content[18] = "Every character can get different buff when your territories... ";
		content[19] = "exceed certain % of entire maps. For example:...";
		content[20] = "Our leader can get territory buff on Atk and Def while standing on our territories.";
		content[21] = "On the contrary, our allies will be debuffed while standing on opponent's territories";
		content[22] = "On next practice, you are going to extend your territories over 25%...";
		content[23] = "and force your opponent standing on your territory. At last give him a lethal strike";
		content[24] = "Great job! Now you know how to use territory buff and debuff to win battles.";
		content[25] = "Now, it's time to teach you how to summon allies.";
		MainCharacter = currentRC.playerA;
		nextStage.MainCharacter = currentRC.playerA;
		Opponent = currentRC.playerB;
		mainP = MainCharacter.GetComponent<CharacterProperty>();
		mainS = MainCharacter.GetComponent<CharacterSelect>();
		oppP = Opponent.GetComponent<CharacterProperty>();
		oppS = Opponent.GetComponent<CharacterSelect>();
		root = GameObject.Find("unit_start_point_A").transform;
		rootB = GameObject.Find("unit_start_point_B").transform;
		int index = Random.Range(0,5);
		opMap = root.GetComponent<Identy>().neighbor[index];
		pSummoner.ResetSummoner(true, null, opMap);
		tutorMom.ShowContinue(true);
		chessUI.SomeoneTaking(MainCharacter, content[0], false);
		fc.CamFollowMe(MainCharacter);
		aUI = transform.GetComponent<ArrowUI>();
		showUI = true;
		currentSel.chess = null;
		currentSel.CleanMapsMat();
	}
	
	void UpdateScreenPos(Transform rectRef){
		screenPos = Camera.main.WorldToScreenPoint(rectRef.position);
		screenPos.y = Screen.height - screenPos.y;
	}
	
	void PrepareLessonE(){
		
		int index = Random.Range(0,5);
		opMap = root.GetComponent<Identy>().neighbor[index];
		
		foreach(Transform m in root.GetComponent<Identy>().neighbor){
			if(m!=null){
				foreach(Transform n in m.GetComponent<Identy>().neighbor){
					if(n!=null){
						foreach(Transform o in n.GetComponent<Identy>().neighbor){
							if(o!=null){
								if(!currentRC.PlayerATerritory.Contains(o)){
									currentRC.AddTerritory(o, 1);
								}
							}
						}
						
					}
				}
			}
		}
		
		foreach(Transform m in rootB.GetComponent<Identy>().neighbor){
			if(m!=null){
				foreach(Transform n in m.GetComponent<Identy>().neighbor){
					if(n!=null){
						foreach(Transform o in n.GetComponent<Identy>().neighbor){
							if(o!=null){
								if(!currentRC.PlayerBTerritory.Contains(o)){
									currentRC.AddTerritory(o, 2);
								}
							}
						}
					}
				}
			}
		}
		foreach(Transform m in opMap.GetComponent<Identy>().neighbor){
			if(m!=null){
				if(!currentRC.PlayerBTerritory.Contains(m)){
					currentRC.AddTerritory(m, 2);
				}
				if(currentRC.PlayerATerritory.Contains(m)){
					currentRC.RemoveTerritory(m, 1);
				}
			}
		}
		if(currentRC.PlayerATerritory.Contains(opMap)){
			currentRC.RemoveTerritory(opMap, 1);
		}
		if(!currentRC.PlayerBTerritory.Contains(opMap)){
			currentRC.AddTerritory(opMap, 2);
		}
		pSummoner.ResetSummoner(true, null, opMap);
		currentSel.updateTerritoryMat();
		tutorMom.ResetPlayerA(MainCharacter);
		oppP.CmdTimes = 0;
		oppP.TurnFinished = true;
		oppP.Hp = 4;
		currentSel.updateAllCharactersPowers();
	}
	
	
	bool CheckLessonC(){
		bool check = false;
		if(mainP.CmdTimes<3 && oppP.Hp<oppP.MaxHp){
			check = true;
		}
		if(mainP.CmdTimes == 0 && !check){
			mainP.CmdTimes += 1; 
			mainP.TurnFinished = false;
		}
		return check;
	}
	
	bool CheckLessonD(){
		bool check = false;
		if(mainP.CmdTimes == 0 && mainP.Attacked && mainS.getMapPosition()!=root && currentRC.PlayerATerritory.Count>3)
			check = true;
		else if(mainP.CmdTimes == 0 && !check){
			mainP.CmdTimes = 3;
			mainP.Attacked = false;
			mainP.TurnFinished = false;
			oppP.Hp = oppP.MaxHp;
		}
		return check;
	}
	
	bool CheckLessonE(){
		bool check = false;
		if(oppP.Hp == 0)
			check = true;
		if(!check && mainP.CmdTimes==0){
			mainP.CmdTimes = 3;
			mainP.Attacked = false;
			mainP.TurnFinished = false;
		}
		return check;
	}
	
	void ArrowCheckLessonA(){
		if(arrowOn[0]){
			if(currentSel.chess == MainCharacter){
				arrowOn[0] = aUI.HideArrow();
				UpdateScreenPos(Opponent);
				arrowRect = new Rect(screenPos.x+25, screenPos.y-105, 64, 64);
				arrowOn[1] = aUI.ShowArrow(arrowRect, ArrowUI.ArrowMode.downLeft);
			}
		}
		if(arrowOn[1]){
			if(tutorMom.GetHitMap()==oppS.getMapPosition()){
				arrowOn[1] = aUI.HideArrow();
			}
		}
	}
	
	void ArrowCheckLessonB(){
	}
	
	// Update is called once per frame
	void Update () {
		
		if(arrowOn[1]){
			screenPos = Camera.main.WorldToScreenPoint(Opponent.position);
			screenPos.y = Screen.height - screenPos.y;
			arrowRect = new Rect(screenPos.x+25, screenPos.y-105, 64, 64);
			aUI.ShowArrow(arrowRect, ArrowUI.ArrowMode.downLeft);
		}
		
		if(pause){
			timeSeg += Time.deltaTime/pauseTime;
			if(timeSeg >= 0.9f){
				timeSeg = 0.0f; 
				pause = false; 
				InitTutorStageB();
			}
		}
		if(subLessonA){
			ArrowCheckLessonA();
			if(CheckLessonC()){
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
			if(CheckLessonD()){
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
		if(subLessonC){
			if(CheckLessonE()){
				showUI = true;
				tutorMom.ShowContinue(true);
				mainP.CmdTimes = 0;
				mainP.TurnFinished = true;
				contentIndex+=1;
				chessUI.SomeoneTaking(MainCharacter, content[contentIndex], false);
				subLessonC = false;
				currentSel.CancelCmds();
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
				if(contentIndex<4){
					contentIndex+=1;
					chessUI.SomeoneTaking(MainCharacter, content[contentIndex], false);
					sMachine.TutorialBusy = true;
				}else if(contentIndex == 4){
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
				if(contentIndex>4 && contentIndex<15){
					sMachine.TutorialBusy = true;
					contentIndex+=1;
					chessUI.SomeoneTaking(MainCharacter, content[contentIndex], false);
				}else if(contentIndex == 15){
					sMachine.TutorialBusy = false;
					tutorMom.ShowContinue(false);
					chessUI.FadeOutUI();
					pSummoner.ResetSummoner(true, null, OppMap);
					tutorMom.ResetPlayerA(MainCharacter);
					oppP.Hp = oppP.MaxHp;
					UpdateScreenPos(MainCharacter);
					subLessonB = true;
					showUI = false;
				}
				if(contentIndex>15 && contentIndex<23){
					sMachine.TutorialBusy = true;
					contentIndex+=1;
					chessUI.SomeoneTaking(MainCharacter, content[contentIndex], false);
				}else if(contentIndex == 23){
					sMachine.TutorialBusy = false;
					tutorMom.ShowContinue(false);
					chessUI.FadeOutUI();
					PrepareLessonE();
					
					subLessonC = true;
					showUI = false;
				}
				if(contentIndex>23 && contentIndex<25){
					sMachine.TutorialBusy = true;
					contentIndex+=1;
					chessUI.SomeoneTaking(MainCharacter, content[contentIndex], false);
				}else if(contentIndex == 25){
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
