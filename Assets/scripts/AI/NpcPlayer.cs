using UnityEngine;
using System.Collections;
using MapUtility;
using System.Collections.Generic;
using System.Linq;

public class NpcPlayer : MonoBehaviour {
	GeneralSelection currentSel;
	RoundCounter currentRC;
	public Transform currentInMove; 
	Transform lastInMove; 
	public bool InMove, InPause;
	bool NpcPlaying; 
	Vector3 oldCamPosition, newCamPosition;
	public bool NpcSummonCam = false;
	const float viewOffsetZ = 30.0f;
	float pauseTime = 0.0f;
	int count = 0;
	IList playerBList, firstPhaseList, GFs;
	bool initPhase, firstPhase, secondPhase, thirdPhase, endPhase, summonPhase;
	public bool npcReviveMode = false;
	public bool MoveCam;
	IList currentCmdList = new List<NpcCommands>();
	CalculateTactics calTactic;
	int next = 0;
	int nextGF = 0;
	float camSeg = 0.0f;
	FollowCam fCam;
	EndSummonland endGame;
	MainInfoUI chessUI;
	StatusMachine sMachine;
	float interSeg = 0.0f;
	// Use this for initialization
	void Start () {
		InMove = false;
		InPause = false;
		currentSel = Camera.main.GetComponent<GeneralSelection>();
		currentRC = Camera.main.GetComponent<RoundCounter>();
		playerBList = new List<Transform>();
		firstPhaseList = new List<Transform>();
		
		GFs = new List<Transform>();
		endGame = GameObject.Find("EndStage").transform.GetComponent<EndSummonland>();
		calTactic = transform.GetComponent<CalculateTactics>();
		initPhase = firstPhase = secondPhase = thirdPhase = endPhase = summonPhase = false;
		fCam = Camera.mainCamera.GetComponent<FollowCam>();
		chessUI = Camera.main.GetComponent<MainInfoUI>();
		sMachine = GameObject.Find("StatusMachine").GetComponent<StatusMachine>();
	}
	
	public void InsertNewGf(Transform gf){
		if(gf!=null && !gf.GetComponent<CharacterProperty>().Death)
			firstPhaseList.Add(gf);
	}
	
	public void InitNPCTurn(){
		playerBList.Clear();
		firstPhaseList.Clear();
		GFs.Clear();
		initPhase = true;
		foreach(Transform chess in currentRC.PlayerBChesses){
			if(!chess.GetComponent<CharacterProperty>().Death){
				playerBList.Add(chess);
			}
		}
		foreach(Transform chess in playerBList){
			if(!chess.GetComponent<CharacterProperty>().Tower)
				firstPhaseList.Add(chess);
		}
		GFs = calTactic.GetSummonGF(2);
		next = 0;
		nextGF = 0;
		NpcSummonCam = false;
	}
	
	IList GetTotalCommands(Transform gf){
		CharacterProperty gfP = gf.GetComponent<CharacterProperty>();
		IList cmdList = new List<NpcCommands>();
		IList tacticPoint = new List<TacticPoint>();
		tacticPoint = calTactic.GetTactic(gf, gfP.Attacked, gfP.CmdTimes);
		
		foreach(TacticPoint tp in tacticPoint){
			IList cmds = GetCommandList(tp);
			foreach(NpcCommands npcCmd in cmds){
				cmdList.Add(npcCmd);
			}
		}
		if(cmdList.Count>3){
			NpcCommands first = (NpcCommands)cmdList[0];
			NpcCommands third = (NpcCommands)cmdList[2];
			if(first.MapUnit == third.MapUnit && third.Command == UICommands.Move)
				cmdList.RemoveAt(2);
		}
		
		return cmdList;
	}
	
	
	IList GetCommandList(TacticPoint tp){
		IList commands = new List<NpcCommands>();
		CmdMode cmd = CmdMode.None;
		//anyalize map first, if map is the map us local -> Do 
		// if map is in the first range -> move and do  
		// if map is in the second range -> move and move and do 
		IList firstMoveRange = calTactic.GetFirstSteps(tp.Owner, null);
		IList secondMoveRange = calTactic.GetMaxSteps(tp.Owner, null);
		IList thirdMoveRange = calTactic.GetThirdSteps(tp.Owner, null);
		Transform localMap = tp.Owner.GetComponent<CharacterSelect>().getMapPosition();
		
		//first tp
		if(tp.MapUnit == localMap && tp.Tactic != Tactics.none){
			cmd = CmdMode.Do;  
		}else if(firstMoveRange.Contains(tp.MapUnit) && tp.Tactic != Tactics.none){
			cmd = CmdMode.Move_and_Do;
		}else if(secondMoveRange.Contains(tp.MapUnit) && tp.Tactic != Tactics.none){
			cmd = CmdMode.Move_and_Move_and_Do;
		}else if(thirdMoveRange.Contains(tp.MapUnit) && tp.Tactic == Tactics.none){
			cmd = CmdMode.Move_and_Move_and_Move;
		}else if(firstMoveRange.Contains(tp.MapUnit) && tp.Tactic == Tactics.none){
			cmd = CmdMode.Move;
		}
		
		switch(cmd){
			case CmdMode.Do:
				UICommands newCmd = TranslateTactic(tp.Tactic);
				NpcCommands npcCmd = new NpcCommands(newCmd,tp.MapUnit,tp.Target);
				commands.Add(npcCmd);
				break; 
			case CmdMode.Move_and_Do:
				NpcCommands npcMoveCmd = new NpcCommands(UICommands.Move, tp.MapUnit);
				commands.Add(npcMoveCmd);
				UICommands newDoCmd = TranslateTactic(tp.Tactic);
				NpcCommands npcDoCmd = new NpcCommands(newDoCmd, tp.MapUnit, tp.Target);
				commands.Add(npcDoCmd);
				break;
			case CmdMode.Move_and_Move_and_Do:
				//find first move location 
				Transform midMap = GetMidLocation(tp.Owner, tp.MapUnit);
				NpcCommands firstMoveCmd = new NpcCommands(UICommands.Move, midMap);
				commands.Add(firstMoveCmd);
				NpcCommands secondMoveCmd = new NpcCommands(UICommands.Move, tp.MapUnit);
				commands.Add(secondMoveCmd);
				UICommands finalCmd = TranslateTactic(tp.Tactic); 
				NpcCommands thirdCmd = new NpcCommands(finalCmd, tp.MapUnit, tp.Target);
				commands.Add(thirdCmd);
				break;
			case CmdMode.Move:
				NpcCommands onlyMoveCmd = new NpcCommands(UICommands.Move, tp.MapUnit);
				Transform newDest = PreVision.GetDirectionMap(tp.Owner);
				if(newDest!=null){
					onlyMoveCmd.MapUnit = newDest;
				}
				commands.Add(onlyMoveCmd);
				break;
			case CmdMode.Move_and_Move_and_Move:
				//find second move location
				Transform secondMidMap = GetSecondMidLocation(tp.Owner, tp.MapUnit);
				//find first move location
				Transform firstMidMap = GetMidLocation(tp.Owner, secondMidMap);
				NpcCommands fstMoveCmd = new NpcCommands(UICommands.Move, firstMidMap);
				commands.Add(fstMoveCmd);
				NpcCommands sndMoveCmd = new NpcCommands(UICommands.Move, secondMidMap);
				commands.Add(sndMoveCmd);
				NpcCommands trdMoveCmd = new NpcCommands(UICommands.Move, tp.MapUnit);
				commands.Add(trdMoveCmd);
				break;
			default:
				break;
		}
		return commands; 
	}
	
	UICommands TranslateTactic(Tactics t){
		UICommands cmd = UICommands.none;
		switch(t){
			case Tactics.Melee_Attack:
				cmd = UICommands.Attack;
				break;
			case Tactics.Range_Attack:
				cmd = UICommands.Attack;
				break;
			case Tactics.Expend:
				cmd = UICommands.Defense;
				break;
			case Tactics.none:
				cmd = UICommands.Defense;
				break;
			default:
				cmd = UICommands.Skill;
				break;
		}
		return cmd;
	}
	
	Transform GetMidLocation(Transform gf, Transform finalDest){
		Transform midLocal = null;
		IList initFirstStep = calTactic.GetFirstSteps(gf, null);
		IList newFirstSetp = calTactic.GetFirstSteps(gf, finalDest);
		IList interSectList = new List<Transform>();
		foreach(Transform m in newFirstSetp){
			if(initFirstStep.Contains(m))
				interSectList.Add(m);
		}
		if(interSectList.Count>0)
			midLocal = interSectList[0] as Transform;
		else{ 
			midLocal = null;
			print("fuck! I cannot find a way to there");
		}
		return midLocal;
	}
	
	
	Transform GetSecondMidLocation(Transform gf, Transform finalDest){
		Transform midLocal = null;
		IList initSecondStep = calTactic.GetMaxSteps(gf, null);
		IList newSecondSetp = calTactic.GetFirstSteps(gf, finalDest);
		IList interSectList = new List<Transform>();
		foreach(Transform m in newSecondSetp){
			if(initSecondStep.Contains(m))
				interSectList.Add(m);
		}
		if(interSectList.Count>0)
			midLocal = interSectList[0] as Transform;
		else{ 
			midLocal = null;
			print("fuck! I cannot find a way to there");
		}
		return midLocal;
	}
	
	bool MoveCommand(Transform chess, Transform dest){
		bool moveable = false;
		CharacterSelect chessSelect = chess.GetComponent<CharacterSelect>();
		CharacterProperty chessProperty = chess.GetComponent<CharacterProperty>();
		Transform localUnit = chessSelect.getMapPosition();
		IList pathList = new List<Transform>();
		Transform sel = dest;
		//check if dest is in move range
		currentSel.updateMapSteps();
		chessSelect.MoveRangeList.Clear();
		chessSelect.findMoveRange(localUnit, 0, chessProperty.BuffMoveRange);
		bool possibleDest = chessSelect.MoveRangeList.Contains(dest);
		chessSelect.MoveRangeList.Clear();
		//if not in move range give it a new dest
		if(!possibleDest)
			sel = PreVision.GetDirectionMap(chess);
		if(sel!=null){
			pathList = chessSelect.FindPathList(localUnit,currentSel.GetSteps(localUnit,sel),sel);
			MoveCharacter mc = Camera.main.GetComponent<MoveCharacter>();
			mc.SetSteps(chess,pathList);
			chessProperty.Moved = true;
			moveable = true;
		}else{
			moveable = false;
		}
		//set machine busy
		sMachine.InBusy = true;
		return moveable;
	}
	
	bool AttackCommand(Transform chess, Transform target){
		bool attackable = false;
		oldCamPosition = Camera.mainCamera.transform.position;
		Vector3 centerPos = MapHelper.GetCenterPos(chess,target);
		newCamPosition = centerPos - currentRC.CamOffest;
		NpcSummonCam = true;
		chess.GetComponent<CharacterSelect>().AttackRangeList.Clear();
		Transform sel = target.GetComponent<CharacterSelect>().getMapPosition();
		if(sel!=null){
			AttackCalFX attackerCal = Camera.main.GetComponent<AttackCalFX>();
			bool critiq = attackerCal.CalcriticalHit(chess,AttackType.physical);
			TurnHead th = Camera.main.GetComponent<TurnHead>();  
			th.SetTurnHeadSequence(chess,sel,true,false,critiq);
			attackable = true;
		}else{
			attackable = false;
		}
		//set machine busy
		sMachine.InBusy = true;
		

		return attackable;
	}
	
	bool DefenseCommand(Transform chess){
		bool defable = true;
		currentSel.DefenseCmd(chess);
		oldCamPosition = Camera.mainCamera.transform.position;
		newCamPosition = chess.transform.position -currentRC.CamOffest;
		NpcSummonCam = true;
		return defable;
	}
	
	bool SkillCommand(Transform chess, Transform target){
		bool skillable = false;
		Transform skill = chess.FindChild("Skills").GetChild(0);
		if(!skill.GetComponent<SkillProperty>().NeedToSelect){
			//skill.GetComponent<SkillProperty>().GetRealSkillRate();
			//skill.GetComponent<SkillProperty>().PassSkillRate = MapHelper.Success(skill.GetComponent<SkillProperty>().SkillRate);
			skill.GetComponent<SkillProperty>().ActivateSkill();
			currentSel.AnimStateNetWork(chess,AnimVault.AnimState.skill);
			chess.GetComponent<CharacterProperty>().Activated = true;
			skill.GetComponent<SkillProperty>().DefaultCDRounds();
			chess.GetComponent<CharacterProperty>().CmdTimes -= 1;
			oldCamPosition = Camera.mainCamera.transform.position;
			newCamPosition = chess.transform.position - currentRC.CamOffest;
			NpcSummonCam = true;
			sMachine.InBusy = true;
		}else{
			SkillInterface cSkill = skill.GetComponent(skill.GetComponent<SkillProperty>().ScriptName) as SkillInterface;
			cSkill.InsertSelection(target);
			cSkill.Execute();
			skill.GetComponent<SkillProperty>().DefaultCDRounds();
			currentSel.AnimStateNetWork(chess,AnimVault.AnimState.skill);
			sMachine.InBusy = true;
			chess.GetComponent<CharacterProperty>().CmdTimes -= 1;
			oldCamPosition = Camera.mainCamera.transform.position;
			Vector3 centerPos = MapHelper.GetCenterPos(chess,target);
			newCamPosition = centerPos - currentRC.CamOffest;
			NpcSummonCam = true;
		}

		return true;
	}
	
	bool SummonCommand(Transform chess, Transform gf){
		bool summoned = false;
		Transform map = calTactic.GetSummonPosition(chess);
		if(gf!=null && map!=null){
			currentSel.currentGF = gf;
			currentSel.SummonTrueCmd(chess, gf, map);
			summoned = true;
			oldCamPosition = Camera.mainCamera.transform.position;
			newCamPosition = gf.position - currentRC.CamOffest;
			NpcSummonCam = true;
		}else{
			NpcSummonCam = false;
			summoned = false;
		}

		return summoned;
	}
	
	bool FinishCommand(Transform chess){
		CharacterProperty chessProperty = chess.GetComponent<CharacterProperty>();
		bool finished = true;
		chessProperty.Moved = true;
		chessProperty.Activated = true;
		chessProperty.Attacked = true;
		chessProperty.CmdTimes = 0;
		chessProperty.TurnFinished = true;
		//NpcSummonCam = false;
		return finished;
	}
	
	bool ExcuteCommand(Transform chess, NpcCommands npcCmd){
		bool excuted = false;
		switch(npcCmd.Command){
			case UICommands.Attack:
				excuted = AttackCommand(chess, npcCmd.Target);
				break;
			case UICommands.Defense:
				excuted = DefenseCommand(chess);
				break;
			case UICommands.Move:
				excuted = MoveCommand(chess, npcCmd.MapUnit);
				break;
			case UICommands.Skill:
				excuted = SkillCommand(chess, npcCmd.Target);
				break;
			case UICommands.Turnfinished:
				excuted = FinishCommand(chess);
				break;
			case UICommands.Summon:
				//excuted = SummonCommand(chess);
				break;
			case UICommands.none:
				break;
		}
		return excuted;
	}
	
	public void ReviveSummoner(Transform masterChess){
		Transform map = calTactic.GetRevivePos();
		if(map!=null){
			currentSel.currentGF = masterChess;
			currentSel.SummonTrueCmd(masterChess, masterChess, map);
			npcReviveMode = true;
			currentSel.reviveMode = false;
			sMachine.InBusy = true;
			oldCamPosition = Camera.main.transform.position;
			newCamPosition = map.position - currentRC.CamOffest;
			NpcSummonCam = true;
			//masterChess.GetComponent<CharacterProperty>().TurnFinished = true;
		}
		
		pauseTime = 100;
	}
	
	public void ShowSelection(Transform chess, bool showUp){
		if(showUp){
			chess.gameObject.layer = 11;
			currentSel.MoveToLayer(chess,11);
			//NGUI
			chessUI.ActivateInfoUI(chess);
		}else{
			chess.gameObject.layer = 10;
			currentSel.MoveToLayer(chess,10);
			//for NGUI
			chessUI.DeactivateInfoUI(1);
			chessUI.DeactivateInfoUI(2);
		}
	}
	
	
	
	Transform GetCurrentInMove(){
		Dictionary<Transform, int> sortDict = new Dictionary<Transform, int>();
		IList gfsOnField = new List<Transform>();
		Transform inMove = null;
		foreach(Transform gf in currentRC.PlayerBChesses){
			CharacterProperty gfp = gf.GetComponent<CharacterProperty>();
			if(!gfp.Death && gfp.CmdTimes>0 && !gfp.Tower){
				gfsOnField.Add(gf);
			}
		}
		if(gfsOnField.Count>0){
			foreach(Transform gf in gfsOnField){
				CharacterProperty gfP = gf.GetComponent<CharacterProperty>();
				IList tacticPoint = new List<TacticPoint>();
				tacticPoint = calTactic.GetTactic(gf, gfP.Attacked, gfP.CmdTimes);
				int totalPoint = 0;
				if(tacticPoint.Count>1){
					foreach(TacticPoint tp in tacticPoint){
						totalPoint += tp.Point;
					}
					
				}else{
					TacticPoint tp = (TacticPoint)tacticPoint[0];
					totalPoint = tp.Point;
				}
				totalPoint = Mathf.RoundToInt((float)totalPoint /(float)tacticPoint.Count);
				sortDict.Add(gf, totalPoint);
			}
			var sortedDict = (from entry in sortDict orderby entry.Value descending select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
			inMove =(Transform)sortedDict.First().Key;  
		}
		
		return inMove; 
	}
	
	void InitStep(){
		if(initPhase && !sMachine.InBusy && firstPhaseList.Count>0 && !InPause){
			InPause = false;

			lastInMove = currentInMove;
			
			if(lastInMove!=null)
				ShowSelection(lastInMove, false);
			
			currentInMove = GetCurrentInMove();
			
			if(currentInMove!=null){
				//Get all cmds
				currentCmdList.Clear();
				currentCmdList = GetTotalCommands(currentInMove);
				foreach(NpcCommands npcCmd in currentCmdList){
					Debug.Log(currentInMove.name + ": " + npcCmd.Command);
				}
				ShowSelection(currentInMove, true);
				
				initPhase = false;
				firstPhase = true;
				NpcSummonCam = false;
			}else{
				endPhase = true;
			}
		}
	}
	
	void FirstStep(){
		if(firstPhase && !sMachine.InBusy && currentInMove!=null && !InPause){
			if(currentCmdList.Count>0){
				NpcCommands firstCmd = (NpcCommands)currentCmdList[0];
				ExcuteCommand(currentInMove, firstCmd);
				if(firstCmd.Command == UICommands.Defense)
					SetPause(2.0f);
				if(calTactic.GetSummonGF(3).Count>0 && currentInMove.GetComponent<CharacterProperty>().Summoner){
					GFs.Clear(); 
					GFs = calTactic.GetSummonGF(3);
					nextGF = 0;
					summonPhase = true; 
				}else{
					secondPhase = true;
				}
				firstPhase = false;
				//Debug.Log("NPC:"+ currentInMove.name +" 1st Step");
			}
		}
	}
	
	void SecondStep(){
		if(secondPhase && !sMachine.InBusy && currentInMove!=null && !InPause){
			InPause = false;
			if(!CheckDeath(currentInMove) && currentCmdList.Count>1){
				NpcCommands secondCmd = (NpcCommands)currentCmdList[1];
				ExcuteCommand(currentInMove, secondCmd);
				if(secondCmd.Command == UICommands.Defense)
					SetPause(2.0f);
				secondPhase = false;
				thirdPhase = true;
				//Debug.Log("NPC:"+ currentInMove.name +" 2nd Step");
			}else{
				if(GetCurrentInMove()!=null){
					secondPhase = false;
					initPhase = true;
				}else{
					endPhase = true;
				}
			}
		}
	}
	
	void SummonPhase(){
		if(summonPhase && !sMachine.InBusy && currentInMove!=null && !CheckDeath(currentInMove)&& !InPause){
			//Debug.Log("NPC: Summon Step");
			Transform newGF = (Transform)GFs[nextGF];
			sMachine.InBusy = SummonCommand(currentInMove, newGF);
			if(nextGF<(GFs.Count-1))
				nextGF += 1;
			else{
				summonPhase = false;
				secondPhase = true;
			}
		}
	}
	
	void ThirdStep(){
		if(thirdPhase && !sMachine.InBusy && currentInMove!=null && !InPause){
			InPause = false;
			if(currentCmdList.Count>2 && !CheckDeath(currentInMove)){
				IList liveList = new List<Transform>();
				foreach(Transform gf in currentRC.PlayerBChesses){
					if(!gf.GetComponent<CharacterProperty>().Death)
						liveList.Add(gf);
				}
				if(liveList.Count > firstPhaseList.Count){
					fCam.timeSeg = 0.0f;
					fCam.CamFollowMe(currentInMove);
				}
				NpcCommands thirdCmd = (NpcCommands)currentCmdList[2];
				ExcuteCommand(currentInMove, thirdCmd);
				if(thirdCmd.Command == UICommands.Defense)
					SetPause(2.0f);
				thirdPhase = false;
				//Debug.Log("NPC:"+ currentInMove.name +" 3rd Step");
				if(GetCurrentInMove()!=null){
					thirdPhase = false;
					initPhase = true;
				}else{
					endPhase = true;
				}
			}else{
				if(GetCurrentInMove()!=null){
					thirdPhase = false;
					initPhase = true;
				}else{
					endPhase = true;
				}
			}
		}
	}
	
	bool CheckDeath(Transform chess){
		return chess.GetComponent<CharacterProperty>().Death;  
	}
	
	
	void EndStep(){
		if(endPhase && !sMachine.InBusy && !InPause && currentInMove!=null){
			//Debug.Log("NPC: End Step");
			InPause = false;
			foreach(Transform chess in playerBList){
				//NpcCommands endCmd = new NpcCommands(UICommands.Turnfinished, null);
				//ExcuteCommand(chess,endCmd);
				if(!chess.GetComponent<CharacterProperty>().Death){
					ShowSelection(chess, false);
					chess.GetComponent<CharacterProperty>().TurnFinished = true;
					chess.GetComponent<CharacterProperty>().CmdTimes = 0;
				}
			}
			endPhase = false;
			initPhase = false;
			thirdPhase = false;
			summonPhase = false;
			secondPhase = false;
			ShowSelection(currentInMove, false);
		}
	}
	
	void translateMainCam(float timeToReach){
		camSeg+= Time.deltaTime/timeToReach;
		Vector3 newPos = Vector3.Lerp(oldCamPosition, newCamPosition, camSeg);
		Camera.main.transform.position = newPos;
		float d = Vector3.Distance(Camera.main.transform.position, newCamPosition);
		if(d<0.001f){
			NpcSummonCam = false;
			camSeg = 0.0f;
		}
	}
	
	public void SetPause(float duration){
		InPause = true;  
		pauseTime = duration;
	}
	
	// Update is called once per frame
	void Update () {
		if(currentSel.NpcPlaying && sMachine.InitGame){
			if(InPause){
				interSeg += Time.deltaTime/pauseTime;
				if(interSeg >= 0.9f){
					interSeg = 0.0f;
					InPause = false;
				}
			}
			/*
			if(!InMove && currentInMove!=null){
				//print("current AI: "+currentInMove);
				lastInMove = currentInMove;
				ShowSelection(lastInMove, false);
			}
			*/
			InitStep();
			FirstStep();
			SecondStep();
			SummonPhase();
			ThirdStep();
			EndStep();
		}
		if(NpcSummonCam && MoveCam)
			translateMainCam(0.4f);
	}
}


public struct NpcCommands{
	public UICommands Command;
	public Transform Target; 
	public Transform MapUnit; 
	
	public NpcCommands(UICommands cmd, Transform map){
		Command = cmd; 
		MapUnit = map;
		Target = null;
	}
	
	public NpcCommands(UICommands cmd, Transform map, Transform target){
		Command = cmd; 
		MapUnit = map; 
		Target = target;
	}
}

public enum CmdMode{
	Move_and_Do,
	Do,
	Move_and_Move_and_Do,
	Move_and_Move_and_Move,
	Move, 
	None,
}