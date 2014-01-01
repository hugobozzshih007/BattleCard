using UnityEngine;
using System.Collections;
using MapUtility;
using System.Collections.Generic;

public class TutorStageC : MonoBehaviour {
	bool showUI = false;
	Rect rectContinue = new Rect();
	public Transform MainCharacter;
	public Transform SummonnedA;
	public Transform SummonnedB; 
	public Texture2D BG_GUI;
	public Font font;
	Transform archerInstance;
	Tutorial tutorMom; 
	MainInfoUI chessUI;
	CharacterProperty mainP, summonnedAP, summonnedBP;
	CharacterSelect mainS, summonnedAS, summonnedBS;
	string[] content  = new string[20];
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
	bool subLessonD = false;
	float demoWidth = 298.0f;
	float demoHeight = 64.0f;
	bool pause = false;
	float pauseTime = 3.0f;
	float timeSeg = 0.0f;
	PlaceSummoner pSummoner;
	GUIStyle numberStyle;
	float _alpha = 0.0f;
	bool fadeIn = true; 
	bool fadeOut = false;
	FollowCam fc;
	Vector3 noWhere = new Vector3(0.0f,1000.0f,0.0f);
	Rect bg = new Rect(0,0,Screen.width, Screen.height);
	Rect bt; 
	ArrowUI aUI; 
	bool[] arrowOn = new bool[10];
	Rect arrowRect = new Rect();
	Vector3 screenPos = new Vector3();
	TutorStageD nextStage;
	SystemSound sSound;
	// Use this for initialization
	void Start () {
		clearBt = new Texture2D(20,20);
		currentSel = Camera.main.GetComponent<GeneralSelection>();
		currentRC = Camera.main.GetComponent<RoundCounter>();
		MainCharacter = currentRC.playerA;
		sMachine = GameObject.Find("StatusMachine").GetComponent<StatusMachine>();
		tutorMom = transform.GetComponent<Tutorial>();
		fc = Camera.main.GetComponent<FollowCam>();
		rectContinue = tutorMom.GetContinueRect();
		pSummoner = GameObject.Find("InitStage").GetComponent<PlaceSummoner>();
		chessUI = Camera.main.GetComponent<MainInfoUI>();
		mainP = MainCharacter.GetComponent<CharacterProperty>();
		mainS = MainCharacter.GetComponent<CharacterSelect>();
		summonnedAP = SummonnedA.GetComponent<CharacterProperty>();
		summonnedBP = SummonnedB.GetComponent<CharacterProperty>();
		summonnedAS = SummonnedA.GetComponent<CharacterSelect>();
		summonnedBS = SummonnedB.GetComponent<CharacterSelect>();
		root = GameObject.Find("unit_start_point_A").transform;
		rootB = GameObject.Find("unit_start_point_B").transform;
		numberStyle = new GUIStyle();
		numberStyle.normal.textColor = new Color(1.0f,1.0f,1.0f,_alpha);
		numberStyle.font = font;
		numberStyle.fontSize = 30;
		bt = new Rect(Screen.width/2.0f - demoWidth/2.0f+20, 5.0f/720.0f*Screen.height, 500, demoHeight);
		sSound = GameObject.Find("SystemSoundB").GetComponent<SystemSound>();
		aUI = transform.GetComponent<ArrowUI>();
		for(int i=0;i<10;i++){
			arrowOn[i] = false;
		}
		nextStage = transform.GetComponent<TutorStageD>();
	}
	
	public void StartPause(float sec){
		pauseTime = sec;
		pause = true;
	}
	
	void InitTutorStageC(){
		mainP.soldiers = new Transform[2];
		mainP.soldiers[0] = SummonnedA;
		mainP.soldiers[1] = SummonnedB;
		
		foreach(Transform gf in mainP.soldiers){
			if(gf!=null){
				Transform gfClone = Instantiate(gf,noWhere,Quaternion.identity) as Transform;
				gfClone.gameObject.layer = 10;
				currentRC.PlayerAChesses.Add(gfClone);
				currentRC.AllChesses.Add(gfClone);
				gfClone.GetComponent<CharacterProperty>().Death = true;
				gfClone.GetComponent<CharacterProperty>().Player = 1;
				gfClone.GetComponent<CharacterProperty>().InitPlayer = 1;
			}
		}

		
		content[0] = "Lesson 6: How to summon your allies!";
		content[1] = "Summonning your allies is very important to learn.";
		content[2] = "However, it's also very easy.";
		content[3] = "All of your allies are listed on the gray bar at bottom of the screen.";
		content[4] = "Once, one of your allies started to flash, it means you can summon it on to the field.";
		content[5] = "After clicking the ally icon, you just need to click one of the highlighted maps.";
		content[6] = "But, you only can summon your allies on to your field. However...";
		content[7] = "Once you want to re-summon your leader, you can summon it onto anywhere of the map.";
		content[8] = "Now, try to expend your territories and summon one of your allies...";
		content[9] = "Great job! BTW, there's no act points assigned to the monster just summoned.";
		content[10] = "Act points will be assigned at the beginning of next turn. Let's go to the next lesson...";
		content[11] = "Lesson 7: How to use skills!";
		content[12] = "Each monster has a unique skill. When it cools down, you can use it through small icon on the bar.";
		content[13] = "It's simple as summonning monsters. When the small icon shows 'SKILL' on it, just click it,...";
		content[14] = "if it needs you to select a target, it will show highlighted maps for you to choose.";
		content[15] = "Now it's your turn to try to cast our leader's skill to buff your archer!";
		content[16] = "Great job! You may noticed that the description of the skill shows on the top bar of the screen...";
		content[17] = "while mouse rolling over the monster small icons.";
		content[18] = "Now you have the basic knowledge of playing this game. Now you need to know what has shown on the Screen.";
		
		pSummoner.ResetSummoner(false, null, null);
		tutorMom.ShowContinue(true);
		chessUI.SomeoneTaking(MainCharacter, content[0], false);
		fc.timeSeg = 0.0f;
		fc.CamFollowMe(MainCharacter);
		currentSel.ChessInSelection = null;
		currentSel.CleanMapsMat();
		showUI = true;
	}
	
	void UpdateScreenPos(Transform rectRef){
		screenPos = Camera.main.WorldToScreenPoint(rectRef.position);
		screenPos.y = Screen.height - screenPos.y;
	}
	
	void PrepareLessonF(){
		Transform archer = (Transform)currentRC.PlayerAChesses[1];
		archerInstance = archer;
		CharacterProperty archerP = archer.GetComponent<CharacterProperty>();
		archerP.WaitRounds = 0;
		archerP.Ready = true;
	}
	
	void PrepareLessonG(){
		//pSummoner.ResetSummoner(false, null, null);
		fc.timeSeg = 0.0f;
		fc.CamFollowMe(MainCharacter);
		int index = Random.Range(0,5);
		//Transform opMap = root.GetComponent<Identy>().neighbor[index];
		//Transform archer = (Transform)currentRC.PlayerAChesses[1];
		//archer.position = opMap.position;
		//archer.Translate(new Vector3(0.0f, 1.5f, 0.0f));
		SkillProperty skillP = MapHelper.FindAnyChildren(MainCharacter, "Skills").GetChild(0).GetComponent<SkillProperty>();
		skillP.WaitingRounds = 0; 
		skillP.SkillReady = true;
	}
	
	bool CheckLessonF(){
		bool check = false;
		Transform archer = (Transform)currentRC.PlayerAChesses[1];
		CharacterProperty archerP = archer.GetComponent<CharacterProperty>();
		if(!archerP.Death){
			check = true;
			arrowOn[2] = aUI.HideArrow();
		}
		if(!check && mainP.CmdTimes == 0){
			mainP.CmdTimes = 3;
			mainP.TurnFinished = false;
		}
		return check;
	}
	
	bool CheckLessonG(){
		bool check = false;
		Transform archer = (Transform)currentRC.PlayerAChesses[1];
		CharacterProperty archerP = archer.GetComponent<CharacterProperty>();
		archerP.CmdTimes = 0;
		archerP.TurnFinished = true;
		if(archerP.Damage > archerP.AtkPower){
			check = true;
		}
		if(!check && mainP.CmdTimes == 0){
			mainP.CmdTimes = 3;
			mainP.TurnFinished = false;
			SkillProperty skillP = MapHelper.FindAnyChildren(MainCharacter, "Skills").GetChild(0).GetComponent<SkillProperty>();
			skillP.WaitingRounds = 0; 
			skillP.SkillReady = true;
		}
		return check; 
	}
	
	void ArrowCheckLessonA(){
		if(arrowOn[0]){
			if(currentRC.PlayerATerritory.Count>2){
				arrowOn[0] = aUI.HideArrow();
				arrowRect = new Rect((float)Screen.width*192.0f/1280.0f, (float)Screen.height*615.0f/720.0f, 64, 64);
				arrowOn[1] = aUI.ShowArrow(arrowRect, ArrowUI.ArrowMode.downLeft);
			}
		}
		if(arrowOn[1]){
			if(currentSel.GetSummonMode()){
				arrowOn[1] = aUI.HideArrow();
				IList territory = new List<Transform>();
				Transform currentPos = mainS.getMapPosition();
				Identity posID = currentPos.GetComponent<Identity>();
				foreach(Transform m in posID.Neighbor){
					if(m!=null)
						territory.Add(m);
				}
				int mID = Random.Range(0, territory.Count);
				Transform randomSel = territory[mID] as Transform;
				UpdateScreenPos(randomSel);
				arrowRect = new Rect(screenPos.x+10, screenPos.y-70, 64, 64);
				arrowOn[2] = aUI.ShowArrow(arrowRect, ArrowUI.ArrowMode.downLeft);
			}
		}
	}
	
	void ArrowCheckLessonB(){
		if(arrowOn[3]){
			if(currentSel.ChessInSelection == MainCharacter){
				arrowOn[3] = aUI.HideArrow();
				arrowRect = new Rect((float)Screen.width*132.0f/1280.0f, (float)Screen.height*615.0f/720.0f, 64, 64);
				arrowOn[4] = aUI.ShowArrow(arrowRect, ArrowUI.ArrowMode.downLeft);
			}
		}
	}
	
	void FadeIn(){
		if(fadeIn){
			_alpha = Mathf.Lerp(_alpha,1.0f,Time.deltaTime*4);
			if(_alpha>=0.9f){
				fadeOut = true;
				fadeIn = false;
			}
		}
	}
	
	void FadeOut(){
		if(fadeOut){
			_alpha = Mathf.Lerp(_alpha,0.0f,Time.deltaTime*4);
			if(_alpha<=0.1f){
				fadeOut = false;
				fadeIn = true;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(subLessonD){
			FadeIn();
			FadeOut();
		}
		if(pause){
			timeSeg += Time.deltaTime/pauseTime;
			if(timeSeg >= 0.9f){
				timeSeg = 0.0f; 
				pause = false; 
				InitTutorStageC();
			}
		}
		if(subLessonA){
			ArrowCheckLessonA();
			if(CheckLessonF()){
				showUI = true;
				tutorMom.ShowContinue(true);
				mainP.CmdTimes = 0;
				mainP.TurnFinished = true;
				contentIndex+=1;
				chessUI.SomeoneTaking(MainCharacter, content[contentIndex], false);
				subLessonA = false;
				currentSel.ChessInSelection = null;
				currentSel.CancelCmds();
				sMachine.TutorialBusy = true;
			}
		}
		if(subLessonB){
			ArrowCheckLessonB();
			if(CheckLessonG()){
				showUI = true;
				arrowOn[4] = aUI.HideArrow();
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
			if(GUI.Button(rectContinue,clearBt)){
				sSound.PlaySound(SysSoundFx.CommandClick);
				if(contentIndex <8){
					contentIndex+=1;
					sMachine.TutorialBusy = true;
					chessUI.SomeoneTaking(MainCharacter, content[contentIndex], false);
				}else if(contentIndex == 8){
					PrepareLessonF();
					tutorMom.ShowContinue(false);
					//chessUI.FadeOutUI();
					sMachine.TutorialBusy = false;
					UpdateScreenPos(MainCharacter);
					arrowRect = new Rect(screenPos.x+25, screenPos.y-105, 64, 64);
					arrowOn[0] = aUI.ShowArrow(arrowRect, ArrowUI.ArrowMode.downLeft);
					subLessonA = true;
					showUI = false;
				}
				
				if(contentIndex>8 && contentIndex<15){
					sMachine.TutorialBusy = true;
					contentIndex+=1;
					chessUI.SomeoneTaking(MainCharacter, content[contentIndex], false);
				}else if(contentIndex == 15){
					PrepareLessonG();
					tutorMom.ShowContinue(false);
					//chessUI.FadeOutUI();
					sMachine.TutorialBusy = false;
					UpdateScreenPos(MainCharacter);
					arrowRect = new Rect(screenPos.x+25, screenPos.y-105, 64, 64);
					arrowOn[3] = aUI.ShowArrow(arrowRect, ArrowUI.ArrowMode.downLeft);
					subLessonB = true;
					showUI = false;
				}
				
				if(contentIndex>15 && contentIndex<18){
					sMachine.TutorialBusy = true;
					contentIndex+=1;
					chessUI.SomeoneTaking(MainCharacter, content[contentIndex], false);
				}else if(contentIndex == 18){
					MainCharacter.GetComponent<SummonFX>().StartDelayDeath(0.2f);
					archerInstance.GetComponent<SummonFX>().StartDelayDeath(0.2f);
					tutorMom.ShowContinue(false);
					//chessUI.FadeOutUI();
					showUI = false;
					tutorMom.ResetMap(0);
					nextStage.StartPause(3.0f);
					currentSel.CancelCmds();
				}
			}
		}
	}
}
