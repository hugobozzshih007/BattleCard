using UnityEngine;
using System.Collections;
using MapUtility;
using System.Collections.Generic;

public class NpcPlayer : MonoBehaviour {
	selection CurrentSel;
	Decisions decisions;
	RoundCounter CurrentRC;
	Transform currentInMove; 
	Transform lastInMove; 
	public bool InMove, InPause;
	bool NpcPlaying; 
	Vector3 oldCamPosition, newCamPosition;
	int camStep = 0;
	bool camMoveMode = false;
	const float viewOffsetZ = 30.0f;
	const int pauseTime = 500;
	int count = 0;
	IList playerBList, firstPhaseList, secondPhaseList, thirdPhaseList;
	bool firstPhase, secondPhase, thirdPhase;
	public bool npcReviveMode = false;
	public bool MoveCam;
	// Use this for initialization
	void Start () {
		InMove = false;
		InPause = false;
		decisions = transform.GetComponent<Decisions>();
		CurrentSel = Camera.main.GetComponent<selection>();
		CurrentRC = Camera.main.GetComponent<RoundCounter>();
		playerBList = new List<Transform>();
		firstPhaseList = new List<Transform>();
		secondPhaseList = new List<Transform>();
		thirdPhaseList = new List<Transform>();
	}
	
	public void InitNPCTurn(){
		playerBList.Clear();
		firstPhase = true;
		secondPhase = false;
		foreach(Transform chess in CurrentRC.PlayerBChesses){
			if(!chess.GetComponent<CharacterProperty>().death){
				playerBList.Add(chess);
			}
		}
		foreach(Transform chess in playerBList){
			if(chess!=CurrentRC.playerB)
				firstPhaseList.Add(chess);
		}
		if(playerBList.Contains(CurrentRC.playerB)){
			firstPhaseList.Add(CurrentRC.playerB);
		}
		
	}
	
	IList InitSecondPhase(){
		secondPhase = true;
		IList secondList = new List<Transform>();
		
		foreach(Transform chess in playerBList){
			if(!chess.GetComponent<CharacterProperty>().death && chess!=CurrentRC.playerB){
				secondList.Add(chess);
			}
		}
		if(playerBList.Contains(CurrentRC.playerB)){
			secondList.Add(CurrentRC.playerB);
		}
		
		return secondList;
	}
	
	IList InitThirdPhase(){
		thirdPhase = true;
		IList thirdList = new List<Transform>();
		
		foreach(Transform chess in playerBList){
			if(!chess.GetComponent<CharacterProperty>().death && chess!=CurrentRC.playerB){
				thirdList.Add(chess);
			}
		}
		
		if(playerBList.Contains(CurrentRC.playerB)){
			thirdList.Add(CurrentRC.playerB);
		}
		
		return thirdList;
	}
	
	void Awake(){
	}
	
	void SelectCharacter(Transform chess){
		
	}
	
	bool MoveCommand(Transform chess){
		bool moveable = false;
		CharacterSelect chessSelect = chess.GetComponent<CharacterSelect>();
		CharacterProperty chessProperty = chess.GetComponent<CharacterProperty>();
		Transform localUnit = chessSelect.getMapPosition();
		IList pathList = new List<Transform>();
		Transform sel = decisions.GetMoveTarget(chess);
		if(sel!=null){
			pathList = chessSelect.FindPathList(localUnit,CurrentSel.GetSteps(localUnit,sel),sel);
			MoveCharacter mc = Camera.main.GetComponent<MoveCharacter>();
			mc.SetSteps(chess,pathList);
			chessProperty.Moved = true;
			moveable = true;
		}else{
			moveable = false;
		}
		return moveable;
	}
	
	bool AttackCommand(Transform chess){
		bool attackable = false;
		chess.GetComponent<CharacterSelect>().AttackRangeList.Clear();
		Transform sel = decisions.GetAttackTarget(chess);
		if(sel!=null){
			AttackCalFX attackerCal = Camera.main.GetComponent<AttackCalFX>();
			bool critiq = attackerCal.CalcriticalHit(chess,AttackType.physical);
			TurnHead th = Camera.main.GetComponent<TurnHead>();  
			th.SetTurnHeadSequence(chess,sel,true,false,critiq);
			attackable = true;
		}else{
			attackable = false;
		}
		
		return attackable;
	}
	
	bool DefenseCommand(Transform chess){
		bool defable = false;
		if(!chess.GetComponent<CharacterProperty>().Attacked){
			MainUI mUI = Camera.main.GetComponent<MainUI>();
			mUI.DefenseCmd(chess);
		}
		return defable;
	}
	
	bool SkillCommand(Transform chess){
		bool skillable = false;
		Transform theSkill = decisions.GetSkill(chess);
		if(theSkill!=null){
			theSkill.GetComponent<SkillProperty>().ActivateSkill();
			CurrentRC.playerB.GetComponent<ManaCounter>().Mana -= theSkill.GetComponent<SkillProperty>().SkillCost;
		}else{
			skillable = false;
		}
		return skillable;
	}
	
	bool SummonCommand(Transform chess){
		bool summoned = false;
		Transform gf = decisions.GetSummonGF(chess);
		Transform map = decisions.GetSummonPosition(chess);
		if(gf!=null && map!=null){
			CurrentSel.currentGF = gf;
			CurrentSel.SummonTrueCmd(chess, gf, map);
			summoned = true;
		}
		oldCamPosition = Camera.mainCamera.transform.position;
		newCamPosition = CurrentRC.playerB.transform.position - CurrentRC.CamOffest;
		camMoveMode = true;
		return summoned;
	}
	
	bool FinishCommand(Transform chess){
		CharacterProperty chessProperty = chess.GetComponent<CharacterProperty>();
		bool finished = true;
		chessProperty.Moved = true;
		chessProperty.Activated = true;
		chessProperty.Attacked = true;
		chessProperty.TurnFinished = true;
		return finished;
	}
	
	bool ExcuteCommand(UICommands cmd, Transform chess){
		bool excuted = false;
		switch(cmd){
			case UICommands.Attack:
				excuted = AttackCommand(chess);
				break;
			case UICommands.Defense:
				excuted = DefenseCommand(chess);
				break;
			case UICommands.Move:
				excuted = MoveCommand(chess);
				break;
			case UICommands.Skill:
				excuted = SkillCommand(chess);
				break;
			case UICommands.Turnfinished:
				excuted = FinishCommand(chess);
				break;
			case UICommands.Summon:
				excuted = SummonCommand(chess);
				break;
			case UICommands.none:
				if(!chess.GetComponent<CharacterProperty>().Defensed || !chess.GetComponent<CharacterProperty>().Attacked)
					excuted = DefenseCommand(chess);
				break;
		}
		return excuted;
	}
	
	public void ReviveSummoner(Transform masterChess){
		Transform map = decisions.GetRevivePos();
		if(map!=null){
			CurrentSel.currentGF = masterChess;
			CurrentSel.SummonTrueCmd(masterChess, masterChess, map);
			npcReviveMode = true;
			CurrentSel.reviveMode = false;
			//masterChess.GetComponent<CharacterProperty>().TurnFinished = true;
		}
	}
	
	public void ShowSelection(Transform chess, bool showUp){
		if(showUp){
			chess.gameObject.layer = 11;
			CurrentSel.MoveToLayer(chess,11);
		}else{
			chess.gameObject.layer = 10;
			CurrentSel.MoveToLayer(chess,10);
		}
	}
	
	void FirstStep(){
		if(firstPhase && !InMove && firstPhaseList.Count>0){
			//get chesses
			InPause = false;
			Transform currentSel = firstPhaseList[0] as Transform;
			if(!currentSel.GetComponent<CharacterProperty>().TurnFinished){
				currentInMove = currentSel;  
				ShowSelection(currentInMove, true);
				//doing commands 
				UICommands firstCmd = decisions.GetFirstCommand(currentSel);
				print("1st: "+currentSel+": "+firstCmd);
				InMove = ExcuteCommand(firstCmd, currentSel);
				if(firstCmd == UICommands.Defense)
					InPause = true;
			}
			firstPhaseList.RemoveAt(0);
			if(firstPhaseList.Count==0){
				firstPhaseList.Clear();
				firstPhase = false;
				secondPhaseList = InitSecondPhase();
			}
		}else if(firstPhaseList.Count==0){
			firstPhaseList.Clear();
			firstPhase = false;
		}
	}
	
	void SecondStep(){
		if(secondPhase && !InMove && secondPhaseList.Count>0){
			InPause = false;
			Transform currentSel = secondPhaseList[0] as Transform;
			if(!currentSel.GetComponent<CharacterProperty>().TurnFinished){
				currentInMove = currentSel;  
				ShowSelection(currentInMove, true);
				UICommands secondCmd = decisions.GetSecondCommand(currentSel);
				print("2nd: "+currentSel+": "+secondCmd);
				InMove = ExcuteCommand(secondCmd, currentSel);
				if(secondCmd == UICommands.Defense)
					InPause = true;
			}
			secondPhaseList.RemoveAt(0);
			if(secondPhaseList.Count==0){
				secondPhaseList.Clear();
				secondPhase = false;
				thirdPhaseList = InitThirdPhase();
			}
		}else if(secondPhaseList.Count==0){
			secondPhaseList.Clear();
			secondPhase = false;
		}
	}
	
	void ThirdStep(){
		if(thirdPhase && !InMove && thirdPhaseList.Count>0){
			InPause = false;
			Transform currentSel = thirdPhaseList[0] as Transform;
			if(!currentSel.GetComponent<CharacterProperty>().TurnFinished){
				currentInMove = currentSel;  
				ShowSelection(currentInMove, true);
				UICommands thirdCmd = decisions.GetThirdCommand(currentSel);
				print("3rd: "+currentSel+": "+thirdCmd);
				InMove = ExcuteCommand(thirdCmd, currentSel);
				if(thirdCmd == UICommands.Defense)
					InMove = false;
			}
			thirdPhaseList.RemoveAt(0);
			if(thirdPhaseList.Count==0){
				thirdPhaseList.Clear();
				thirdPhase = false;
			}
		}else if(thirdPhaseList.Count==0){
				thirdPhaseList.Clear();
				thirdPhase = false;
		}
	}
	
	void EndStep(){
		if(!firstPhase && !secondPhase && !thirdPhase && !InMove){
			foreach(Transform chess in playerBList){
				ExcuteCommand(UICommands.Turnfinished, chess);
			}
		}
	}
	
	void translateMainCam(int segment){
		float segX = (oldCamPosition.x-newCamPosition.x)/segment;
		float segZ = (oldCamPosition.z-newCamPosition.z)/segment;
		Transform camObj = Camera.mainCamera.transform;
		camObj.position = new Vector3(camObj.position.x-segX,camObj.position.y,camObj.position.z-segZ);
		camStep+=1;
		if(camStep==segment){
			camMoveMode = false;
			camStep = 0;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(CurrentSel.NpcPlaying){
			if(InPause){
				count++;
				if(count>=pauseTime){
					count = 0;
					InPause = false;
					InMove = false;
				}
			}
			if(!InMove && currentInMove!=null){
				//print("current AI: "+currentInMove);
				lastInMove = currentInMove;
				ShowSelection(lastInMove, false);
			}
			FirstStep();
			SecondStep();
			ThirdStep();
			EndStep();
		}
		if(camMoveMode && MoveCam)
			translateMainCam(120);
	}
}
