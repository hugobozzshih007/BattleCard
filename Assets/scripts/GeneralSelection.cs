using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MapUtility;
using BuffUtility;

public class GeneralSelection : MonoBehaviour {
	public bool npcMode;
	public Transform sel;
	public LayerMask MaskMap;
	public LayerMask MaskCharaceter;
	public Transform chess = null;
	Transform oldChess = null;
	IList neighbors = new List<Transform>();
	IList skillrollOverList = new List<Transform>();
	IList attackableLists = new List<Transform>();
	public Transform DefRingFXRed, DefRingFXYel; 
	Transform[] summonArea;
	private Transform selOld;
	private SixGonRays unit;
	public Material[] originalMat = new Material[3];
	private bool moveMode = false;
	private bool summonMode = false;
	public bool selectMode = false;
	private bool attackMode = false;
	private bool skillMode = false;
	public bool APlaying = false;
	public bool BPlaying = false;
	public bool Playing = false;
	public bool NpcPlaying = false;
	public Material[] rollOver = new Material[3];
	public Material[] closeBy = new Material[3];
	public Material[] rollOverEnemy = new Material[3];
	public Material highLightSkill; 
	public Material rollOverSkill;
	public float castLength = 80.0f;
	public bool MoveCam;
	public int steps = 2;
	private const float viewOffsetZ = 30.0f;
	private Vector3 oldCamPosition;
	private Vector3 newCamPosition;
	public Transform currentGF;
	//bool summonList = false;
	//bool skillList = false;
	//bool summoner = false;
	bool camMoveMode = false;
	bool inSelect = false;
	//bool infoUI = false;
	public bool SummonIn = false;
	//int guiSegX = 30;
	//int guiSegY = 20;
	//int guiSeg = 10;
	//int camStep = 0;
	float t = 0;
	float movingSeg = 0.0f;
	float fadeInTime = 2.0f;
	Vector3 screenPos;
	public Transform player, CurrentSkill;
	MainUI mainUI;
	bool processPrize = true;
	StatusUI statusUI;
	RoundCounter players;
	RoundUI rUI;
	MainInfoUI chessUI;
	CommonFX fxVault; 
	AttackCalFX attackerCal;
	Color red = new Color(1.0f,155.0f/255.0f,155.0f/255.0f,1.0f);
	Color yellow = new Color(1.0f,245.0f/255.0f,90.0f/255.0f,1.0f);
	Color rollOverBlue = new Color(0.3f,0.3f,0.3f,1.0f);
	Color rollOverRed = new Color(0.7f,0.7f,0.7f,1.0f);
	public bool reviveMode = false;
	NpcPlayer npc;
	int playerSide = 0; 
	PlacePrizes pp;
	PrizeStorage ps;
	EndSummonland endGame;
	PlaceSummoner pSummoner; 
	Vector2 mousePos;
	const float maxCamY = 65.0f; 
	const float minCamX = 30.0f;
	Rect noColArea = new Rect(0,Screen.height-60,Screen.width,60);
	StatusMachine sMachine;
	SystemSound sysSound, voice;
	NameMaps nMaps;
	TimeOut tOut;
	void Start(){
		//setup application
		Application.targetFrameRate = 70;
		sel = GameObject.Find("unit_start_point_A").transform;
		selOld = sel;
		players = Camera.mainCamera.GetComponent<RoundCounter>();
		moveMode = false;
		summonMode = false;
		selectMode = false;
		statusUI = transform.GetComponent<StatusUI>();
		mainUI = transform.GetComponent<MainUI>();
		chessUI = transform.GetComponent<MainInfoUI>();
		fxVault = transform.GetComponent<CommonFX>();
		attackerCal = transform.GetComponent<AttackCalFX>();
		npc = GameObject.Find("NpcPlayer").GetComponent<NpcPlayer>();
		rUI = transform.GetComponent<RoundUI>();
		pp = GameObject.Find("Maps").transform.GetComponent<PlacePrizes>();
		ps = GameObject.Find("Maps").transform.GetComponent<PrizeStorage>();
		nMaps = GameObject.Find("Maps").transform.GetComponent<NameMaps>();
		endGame = GameObject.Find("EndStage").transform.GetComponent<EndSummonland>();
		processPrize = true;
		pSummoner = GameObject.Find("InitStage").transform.GetComponent<PlaceSummoner>();
		sMachine = GameObject.Find("StatusMachine").GetComponent<StatusMachine>();
		sysSound = GameObject.Find("SystemSound").GetComponent<SystemSound>();
		voice = GameObject.Find("SystemSound_voice").GetComponent<SystemSound>();
		tOut = GameObject.Find("TimeOut").GetComponent<TimeOut>();
	}
	
	void Awake(){
		if(Network.connections.Length>0){
			npcMode = false;
			if(Network.peerType == NetworkPeerType.Server){
				APlaying = true;
				BPlaying = false;
				Playing = APlaying;
				playerSide = 1;
			}else if(Network.peerType == NetworkPeerType.Client){
				print("I'm client");
				APlaying = false;
				BPlaying = false;
				Playing = APlaying;
				playerSide = 2;
			}
		}else{
			npcMode = true;
			APlaying = true;
			BPlaying = false;
			Playing = APlaying;
			NpcPlaying = false;
			playerSide = 1;
		}
	}
	
	public int GetPlayerSide(){
		if(Network.connections.Length>0){
			if(Network.peerType == NetworkPeerType.Server){
				playerSide = 1;
			}else if(Network.peerType == NetworkPeerType.Client){
				playerSide = 2;
			}
		}else{
			playerSide = 1;
		}
		return playerSide;
	}
	
	public void CleanMapsMat(){
		selectMode = false;
		moveMode = false;
		attackMode = false;
		neighbors.Clear();
		skillrollOverList.Clear();
		attackableLists.Clear();
		nMaps.ToggleGrid(tOut.gridOn);
		Transform allMap = GameObject.Find("Maps").transform;
		for(int i =0; i< allMap.childCount; i++){
			
			allMap.GetChild(i).renderer.material = GetOriginalMat(allMap.GetChild(i));
			
			if(!npcMode)
				networkView.RPC("RPCUpdateMat",RPCMode.OthersBuffered,allMap.GetChild(i).name,1);
		}
	}
		
	public void CancelCmds(){
		//guiShow = false;
		//mainUI.MainGuiFade = false;
		//mainUI.SubGuiFade = false;
		if(!sMachine.TutorialMode)
			chessUI.MainFadeIn = false;
		chessUI.TargetFadeIn = false;
	    inSelect = false;
		moveMode = false;
		attackMode = false;
		reviveMode = false;	
		skillMode = false;
		selectMode = false;
		summonMode = false;

			
		if(chess != null){
			chess.gameObject.layer = 10;
			chess.GetComponent<CharacterProperty>().InSelection = false;
			MoveToLayer(chess,10);
			if(Playing && !npcMode){
				networkView.RPC("RPCCancelStatusUI",RPCMode.Others,chess.name);
				networkView.RPC("RPCUpdateChessLayers",RPCMode.Others,chess.name,10);
				networkView.RPC("RPCUpdateTargetInfoUI",RPCMode.Others,chess.name,false);
			}
		}
    	
		CleanMapsMat();

		updateTerritoryMat();
	}
	
	void alignCamera(Transform focus){
		Vector3 newCamPosition = focus.position - players.CamOffest;
		transform.position = newCamPosition;
		camMoveMode = true;
	}
	
	void translateMainCam(float timeToReach){
		movingSeg+= Time.deltaTime/timeToReach;
		Vector3 newPos = Vector3.Lerp(oldCamPosition, newCamPosition, movingSeg);
		transform.position = newPos;
		float d = Vector3.Distance(transform.position, newCamPosition);
		if(d<0.03f){
			camMoveMode = false;
			movingSeg = 0.0f;
			//screenPos = Camera.main.WorldToScreenPoint(chess.position);
			//screenPos.y = Screen.height - screenPos.y;
		}
	}
	
	public void SummonTrueCmd(Transform currentChess,Transform gf, Transform map){
		gf.position = map.position;
		CharacterProperty gfp = gf.GetComponent<CharacterProperty>();
		Vector3 relativePos = currentChess.transform.position - gf.transform.position;
		gf.Translate(new Vector3(0.0f,1.5f,0.0f));
		Transform targetLook = transform.GetComponent<MoveCharacter>().GetClosetChess(gf);
		if(targetLook!=null)
			relativePos = targetLook.transform.position - gf.transform.position;
		Quaternion rotation = Quaternion.LookRotation(relativePos);
		gf.transform.rotation = Quaternion.Euler(new Vector3(gf.transform.rotation.eulerAngles.x, rotation.eulerAngles.y, gf.transform.rotation.eulerAngles.z));
		
									
		Vector3 fxPos = new Vector3(map.transform.position.x,map.transform.position.y+0.1f,map.transform.position.z);
		GameObject flashIn = Instantiate(fxVault.SummonIn,fxPos,Quaternion.identity) as GameObject;
		Destroy(flashIn, 3.0f);
		//Summon FX
		
		SummonFX gfSFX = gf.GetComponent<SummonFX>();
		gfSFX.ActivateSummonFX();
		
		
		if(gfp.Tower){
			defenseCommand(gf);
		}
		
		gfp.death = false;
		gfp.Attacked = true;
		gfp.Activated = true;
		gfp.CmdTimes = 0;
		gfp.Hp = gfp.MaxHp;
		gfp.TurnFinished = true;
		Transform[] gfSkills = gf.GetComponent<SkillSets>().Skills;
		foreach(Transform skill in gfSkills){
			skill.GetComponent<SkillProperty>().DefaultCDRounds();
		}
		foreach(BuffType Buff in Enum.GetValues(typeof(BuffType))){
			BuffList bfList = gf.GetComponent<BuffList>();
			if(bfList != null)
				bfList.ExtraDict[Buff] = 0;
		}
		CharacterPassive chessPassive = gf.GetComponent<CharacterPassive>();
		foreach(PassiveType passive in Enum.GetValues(typeof(PassiveType))){
			chessPassive.PassiveDict[passive] = false;
		}
		if((chessPassive.PassiveAbility!=null) && (chessPassive.PassiveAbility.Length>0)){
			foreach(PassiveType p in chessPassive.PassiveAbility){
				chessPassive.PassiveDict[p] = true;
			}
		}
		//update unnormal status
		List<UnnormalStatus> keys = new List<UnnormalStatus>(gfp.UnStatusCounter.Keys);
		foreach(UnnormalStatus key in keys){
			gfp.UnStatusCounter[key] = 0;
			gfp.LastUnStatusCounter[key] = 0;
		}
		
		updateAllCharactersPowers();
		updateTerritoryMat();
	}
	
	//select map or character
	void selecting(){
		if(Input.GetMouseButtonDown(0)){
			
			Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        	//Debug.DrawRay(ray.origin, ray.direction * castLength, Color.yellow);
			RaycastHit hit;
			LayerMask switchMask = MaskCharaceter;
			if(Physics.Raycast(ray, out hit, castLength,MaskCharaceter)){
				if(playerSide == hit.transform.GetComponent<CharacterProperty>().Player && !skillMode){
					selectMode = false;
				}
			}
			
			if(selectMode)
				switchMask = MaskMap;
			if(Physics.Raycast(ray, out hit, castLength,switchMask)){
				if(hit.transform.GetComponent<Identy>() != null){
					if(!hit.transform.GetComponent<Identy>().MapUnit){
						if(selectMode)
							CancelCmds();
						chess = hit.transform;
						//update layer
						
						chess.gameObject.layer = 11;
						MoveToLayer(chess,11);
						chess.GetComponent<CharacterProperty>().InSelection = true;
						inSelect = true;
						
						
						chessUI.InsertChess(chess);
						chessUI.MainFadeIn = true;
						chessUI.Critical = false;
						if(Playing && !npcMode){
							//print(npcMode);
							networkView.RPC("RPCUpdateChessLayers",RPCMode.Others,chess.name,11);
							networkView.RPC("RPCUpdateTargetInfoUI", RPCMode.Others,chess.name,true);
						}
							
						if(chess!=oldChess && oldChess!=null){
							oldChess.gameObject.layer = 10;
							oldChess.GetComponent<CharacterProperty>().InSelection = false;
							MoveToLayer(oldChess,10);
							if(Playing && !npcMode)
								networkView.RPC("RPCUpdateChessLayers",RPCMode.Others,oldChess.name,10);
						}
						oldChess = chess;
						/*	
						newCamPosition = chess.position - players.CamOffest;
						oldCamPosition = transform.position;
						Vector3 diffCamPos = newCamPosition-oldCamPosition;
						if(Math.Abs(diffCamPos.x)>15 || Math.Abs(diffCamPos.z)>12){
							camMoveMode = true;
						}else{
							screenPos = Camera.main.WorldToScreenPoint(chess.position);
							screenPos.y = Screen.height - screenPos.y;
						}*/
						if(chess.GetComponent<CharacterProperty>().Player>1){
								player = players.playerB;
							}else{
								player = players.playerA;
						}
						
						CharacterProperty chessProperty = chess.GetComponent<CharacterProperty>();
						if(chessProperty.TurnFinished || !Playing){
							if(!selectMode){
								//guiShow = false;
								//mainUI.MainGuiFade = false;
								//mainUI.IgnoreMainUI = true;
								//mainUI.SubGuiFade = false;
								chessUI.InsertChess(chess);
								chessUI.MainFadeIn = true;
								chessUI.TargetFadeIn = true;
								//this.GetComponent<InfoUI>().infoUI = true;
							}
						}
					}
				}
			}
		}
		//play voice of selected character
		if(selectMode && Input.GetMouseButtonUp(0))	
			voice.PlayVoiceSelect(chess);
		
		if(selectMode){
			Ray ray = camera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, castLength,MaskMap)){
				if(hit.transform.GetComponent<Identy>()!=null){
					if(hit.transform.GetComponent<Identy>().MapUnit){
						sel = hit.transform;
						if(neighbors.Contains(sel)){
							if(!skillMode)
								sel.renderer.material = GetRollOverMat(sel);
							else
								sel.renderer.material = highLightSkill;
							if(MapHelper.IsMapOccupied(sel)){
								Transform targetChess = MapHelper.GetMapOccupiedObj(sel);
								//mainUI.IgnoreMainUI = true;
								chessUI.InsertChess(targetChess);
								chessUI.TargetFadeIn = true;
								if(!npcMode)
									networkView.RPC("RPCUpdateMainInfoUI",RPCMode.Others,targetChess.name,true);
							}else{
								//mainUI.IgnoreMainUI = false;
								chessUI.TargetFadeIn = false;
								if(!npcMode)
									networkView.RPC("RPCUpdateMainInfoUI",RPCMode.Others,sel.name,false);
							}
							if(!npcMode)
								networkView.RPC("RPCUpdateMat",RPCMode.Others,sel.name,2);
							
							
							if(selOld!=sel && neighbors.Contains(selOld)){
								selOld.renderer.material = GetCloseByMat(selOld);
								//mainUI.IgnoreMainUI = false;
								chessUI.TargetFadeIn = false;
								if(!npcMode){
									networkView.RPC("RPCUpdateMainInfoUI",RPCMode.Others,sel.name,false);
									networkView.RPC("RPCUpdateMat",RPCMode.Others,selOld.name,3);
								}
							}
							selOld = sel;
							if(Input.GetMouseButtonDown(0)){
								
								if(moveMode){
									
									CharacterSelect chessSelect = chess.GetComponent<CharacterSelect>();
									CharacterProperty chessProperty = chess.GetComponent<CharacterProperty>();
									Transform localUnit = chessSelect.getMapPosition();
									localUnit.renderer.material = GetOriginalMat(localUnit);
									
									//animate character moving from point to point, start point: locarUnit, end point: sel
									IList pathList = new List<Transform>();
									pathList = chessSelect.FindPathList(localUnit,GetSteps(localUnit,sel),sel);
									MoveCharacter mc = transform.GetComponent<MoveCharacter>();
									mc.SetSteps(chess,pathList);
									//set machine busy
									sMachine.InBusy = true;
									
									if(!npcMode){
										networkView.RPC("RPCUpdateMat",RPCMode.Others,localUnit.name,1);
										networkView.RPC("UpdateMoveCharacter",RPCMode.Others,chess.name, sel.name);
										networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,2,false);
									}
									
									CleanMapsMat();
									
									chessSelect.MoveRangeList.Clear();
									
									updateTerritoryMat();
									
									moveMode = false;
									attackMode = false;
									attackableLists.Clear();
								
								}
								if(summonMode){
									if(!reviveMode){
										Transform localMap = chess.GetComponent<CharacterSelect>().getMapPosition();
										localMap.renderer.material = GetOriginalMat(localMap);
										if(!npcMode)
											networkView.RPC("RPCUpdateMat", RPCMode.Others,chess.GetComponent<CharacterSelect>().getMapPosition().name,1);
									}
									
									SummonTrueCmd(chess, currentGF, sel);
									
									CleanMapsMat();
									
									Transform targetLook = transform.GetComponent<MoveCharacter>().GetClosetChess(currentGF);
									
									//update network
									if(!npcMode){
										networkView.RPC("RPCSummonIn",RPCMode.Others,currentGF.name, sel.name);
										networkView.RPC("RPCShowUp",RPCMode.Others,currentGF.name);
										networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,4,false);
										networkView.RPC("RPCUpdateChessPosition",RPCMode.Others,sel.name,currentGF.name);
										networkView.RPC("RPCUpdateRenderStatus",RPCMode.Others,currentGF.name, currentGF.renderer.enabled);
										networkView.RPC("RPCUpdateChessSummon",RPCMode.Others,currentGF.name);
										if(!currentGF.GetComponent<CharacterProperty>().Tower)
											networkView.RPC("RPCLookAtTarget",RPCMode.Others,currentGF.name,targetLook.name);
										//networkView.RPC("RPCUpdateMana",RPCMode.Others,player.GetComponent<ManaCounter>().Mana,currentGF.GetComponent<CharacterProperty>().Player );
									}
									
									// update UI
									//mainUI.SubGuiFade = false;
									if(!reviveMode){
										//mainUI.MainGuiFade = true;
									}else{
										//mainUI.MainGuiFade = false;
										selectMode = false;
									}
									summonMode = false;
									reviveMode = false;
									CancelCmds();
									if(!npcMode)
										networkView.RPC("RPCUpdateRevive", RPCMode.Others,false);
								} 
								
								if(skillMode){
									if(CurrentSkill!=null){
										
										CurrentSkill.GetComponent<SkillProperty>().GetRealSkillRate();
										CurrentSkill.GetComponent<SkillProperty>().PassSkillRate = MapHelper.Success(CurrentSkill.GetComponent<SkillProperty>().SkillRate);
										CommonSkill cSkill = CurrentSkill.GetComponent(CurrentSkill.GetComponent<SkillProperty>().ScriptName) as CommonSkill;
										cSkill.InsertSelection(sel);
										cSkill.Execute();
										skillMode = false;
										sMachine.InBusy = true;
	
										chess.GetComponent<CharacterProperty>().Activated = true;
										CurrentSkill.GetComponent<SkillProperty>().DefaultCDRounds();
										chess.GetComponent<CharacterProperty>().CmdTimes -= 1;
										//mainUI.TurnFinished(chess, true);
										
										CleanMapsMat();
										
										chessUI.DelayFadeOut = true;
										chessUI.TargetFadeIn = false;
										AnimStateNetWork(chess,AnimVault.AnimState.skill);
										// update network
										if(!npcMode){
											networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,3,false);
											networkView.RPC("RPCUpdateChessActivated",RPCMode.Others, chess.name,true);
											networkView.RPC("RPCUpdateChessSkillCmd", RPCMode.Others, chess.name,CurrentSkill.name,sel.name,CurrentSkill.GetComponent<SkillProperty>().PassSkillRate);
										}
										chessUI.StopSkillRender = true;
										updateTerritoryMat();
									}else{
										print("Where is current skill???????????");
									}
								
									
									// update UI
									//mainUI.SubGuiFade = false;
									
								}
								neighbors.Clear();
								selectMode = false;
							}
						}
						
						if(attackableLists.Contains(sel)){
							if(MapHelper.IsMapOccupied(sel)){
								Transform targetChess = MapHelper.GetMapOccupiedObj(sel);
								//mainUI.IgnoreMainUI = true;
								chessUI.InsertChess(targetChess);
								chessUI.TargetFadeIn = true;
								if(!npcMode)
									networkView.RPC("RPCUpdateMainInfoUI",RPCMode.Others,targetChess.name,true);
							}else{
								//mainUI.IgnoreMainUI = false;
								chessUI.TargetFadeIn = false;
								if(!npcMode)
									networkView.RPC("RPCUpdateMainInfoUI",RPCMode.Others,sel.name,false);
							}
							if(!npcMode)
								networkView.RPC("RPCUpdateMat",RPCMode.Others,sel.name,2);
							
							
							if(selOld!=sel && attackableLists.Contains(selOld)){
								selOld.renderer.material = GetCloseByMat(selOld);
								//mainUI.IgnoreMainUI = false;
								chessUI.TargetFadeIn = false;
								if(!npcMode){
									networkView.RPC("RPCUpdateMainInfoUI",RPCMode.Others,sel.name,false);
									networkView.RPC("RPCUpdateMat",RPCMode.Others,selOld.name,3);
								}
							}
							selOld = sel;
							if(Input.GetMouseButtonDown(0)){
								
								if(attackMode){
									// restore the original mat for the map
									Transform localMap = chess.GetComponent<CharacterSelect>().getMapPosition();
									localMap.renderer.material = GetOriginalMat(localMap);
									
									CleanMapsMat();
									
									chess.GetComponent<CharacterSelect>().AttackRangeList.Clear();
									
									// activate turning head 
									bool critiq = attackerCal.CalcriticalHit(chess,AttackType.physical);
									
									TurnHead th = transform.GetComponent<TurnHead>();  
									th.SetTurnHeadSequence(chess,sel,true,false,critiq);
									//start busy
									sMachine.InBusy = true;
									
									oldCamPosition = transform.position;
									Transform target = MapHelper.GetMapOccupiedObj(sel); 
									newCamPosition = MapHelper.GetCenterPos(chess,target) - players.CamOffest;
									camMoveMode = true;
									if(!npcMode){
										networkView.RPC("RPCUpdateMat",RPCMode.Others,chess.GetComponent<CharacterSelect>().getMapPosition().name,1);
										networkView.RPC("UpdateTurningHead",RPCMode.Others,chess.name,sel.name,critiq);
										networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,1,false);
									}
									
									attackMode = false;
									moveMode = false;
									updateTerritoryMat();
								}
								attackableLists.Clear();
								neighbors.Clear();
								selectMode = false;
							}
						}
						sel = null;
					}
				}
			}
		}
	}
	
	public void updateTerritoryMat(){
		//network update territory
		IList aTNames = new List<string>();
		IList bTNames = new List<string>();
		
		foreach(Transform map in players.AllTerritory){
			if(!players.PlayerATerritory.Contains(map) && !players.PlayerBTerritory.Contains(map)){
				map.GetComponent<PlaneShadows>().ChangeShadowMaterial(3);
				if(!npcMode)
					networkView.RPC("RPCUpdateTerritory",RPCMode.Others,map.name, 3);
			}
		}
		
		foreach(Transform territory in players.PlayerATerritory){
			territory.GetComponent<PlaneShadows>().ChangeShadowMaterial(1);
			aTNames.Add(territory.name);
			if(!npcMode)
				networkView.RPC("RPCUpdateTerritory",RPCMode.Others,territory.name, 1);
			//territory.GetComponent<Identy>().originalMat = players.TerritoryA;
			//territory.GetComponent<Identy>().ShowMap.renderer.material = players.TerritoryA;
		}
		foreach(Transform territory in players.PlayerBTerritory){
			territory.GetComponent<PlaneShadows>().ChangeShadowMaterial(2);
			bTNames.Add(territory.name);
			if(!npcMode)
				networkView.RPC("RPCUpdateTerritory",RPCMode.Others,territory.name, 2);
			//territory.GetComponent<Identy>().originalMat = players.TerritoryB;
			//territory.GetComponent<Identy>().ShowMap.renderer.material = players.TerritoryB;
		}
	}
	
	public void updateMapSteps(){
		Transform allMap = GameObject.Find("Maps").transform;
		int mapUnitNum = allMap.GetChildCount();
		for(int i=0;i<mapUnitNum;i++){
			allMap.GetChild(i).GetComponent<Identy>().step = 0;
			//print(allMap.GetChild(i).name + ".step=" + allMap.GetChild(i).GetComponent<Identy>().step);
		}
	}
	
	void updateCharacterPowers(Transform character){
		BuffCalculation buffCal = new BuffCalculation(character);
		buffCal.UpdateBuffValue();
		//update network
		if(!npcMode)
			networkView.RPC("RPCUpdateChessPower",RPCMode.Others,character.name);
	}
	
	public void updateAllCharactersPowers(){
		foreach(Transform character in players.AllChesses){
			if(!character.GetComponent<CharacterProperty>().death)
				updateCharacterPowers(character);
		}
	}
	
	public bool GetSummonMode(){
		return summonMode;
	}
	
	public void moveCommand(Transform chess){
		neighbors.Clear();
		summonMode = false;
		skillMode = false;
		if(chess!=null){
			updateMapSteps();
			neighbors.Clear();
			CharacterSelect character = chess.GetComponent<CharacterSelect>();
			Transform currentPos = character.getMapPosition();
			if(currentPos!=null){
				character.findMoveRange(currentPos,0,chess.GetComponent<CharacterProperty>().BuffMoveRange);
				neighbors = character.MoveRangeList;
				if(neighbors.Count>0){
					foreach(Transform sixGon in neighbors){
						
						sixGon.renderer.material = GetCloseByMat(sixGon);
						if(!npcMode)
							networkView.RPC("RPCUpdateMat",RPCMode.Others,sixGon.name,3);
					}
					currentPos.renderer.material = GetRollOverMat(currentPos);
					if(!npcMode)
						networkView.RPC("RPCUpdateMat",RPCMode.Others,currentPos.name,2);
					selectMode = true;
					moveMode =true;
				}else{
					chess.GetComponent<CharacterProperty>().Moved = true;
				}
			}
		}
	}
	
	public void attackCommand(Transform chess){
		summonMode = false;
		skillMode = false;
		if(!chess.GetComponent<CharacterProperty>().Attacked){
			attackableLists.Clear();
			updateAllCharactersPowers();
			if(chess!=null){
				//updateMapSteps();
				AttackCalculation attackerCal = new AttackCalculation(chess);
				CharacterSelect character = chess.GetComponent<CharacterSelect>();
				Transform currentPos = character.getMapPosition();
				if(currentPos!=null){
					attackableLists = attackerCal.GetAttableTarget(attackerCal.Attacker);
					if(attackableLists.Count>0){
						foreach(Transform sixGon in attackableLists){
							sixGon.renderer.material= GetEnemyMat(sixGon);
							if(!npcMode)
								networkView.RPC("RPCUpdateMat",RPCMode.Others,sixGon.name,4);
						}
						currentPos.renderer.material= GetRollOverMat(currentPos);
						if(!npcMode)
							networkView.RPC("RPCUpdateMat",RPCMode.Others,currentPos.name,2);
						selectMode = true;
						attackMode = true;
					}
				}
			}
		}
	}
	
	public void defenseCommand(Transform chess){
		neighbors.Clear();
		attackableLists.Clear();
		if(chess!=null){
			Transform locationMap = chess.GetComponent<CharacterSelect>().getMapPosition();
			CharacterProperty chessProperty = chess.GetComponent<CharacterProperty>();
			int playerSide = chessProperty.Player;
			int theOtherSide = 1;
			Transform ringFX = DefRingFXRed;
			if(playerSide==1){
				theOtherSide =2;
			}else if(playerSide==2){
				theOtherSide =1;
				ringFX = DefRingFXYel;
			}
			
			if(chessProperty.Summoner || chessProperty.LeadingCharacter || chessProperty.Tower){
				foreach(Transform territory in locationMap.GetComponent<Identy>().neighbor){
					//tower defense
					if(chessProperty.Tower && territory!=null){
						Identy territoryID = territory.GetComponent<Identy>();
						territoryID.FixedSide = chessProperty.Player;
						locationMap.GetComponent<Identy>().FixedSide = chessProperty.Player;
						if(!npcMode){
							networkView.RPC("UpdateMapFixedSide", RPCMode.Others, territory.name, territoryID.FixedSide);
							networkView.RPC("UpdateMapFixedSide", RPCMode.Others, locationMap.name, territoryID.FixedSide);
						}
					}
					if(territory!=null){
						Identy territoryID = territory.GetComponent<Identy>();
						if(territoryID.FixedSide==3 || territoryID.FixedSide == chessProperty.Player ){
							if(!players.IsInsideTerritory(territory,playerSide)&& territory!=null){
								players.AddTerritory(territory,playerSide);
								if(chessProperty.Tower){
									territoryID.FixedSide = chessProperty.Player;
									if(!npcMode)
										networkView.RPC("UpdateMapFixedSide", RPCMode.Others, territory.name, territoryID.FixedSide);
								}
								if(players.IsInsideTerritory(territory,theOtherSide)){
									players.RemoveTerritory(territory,theOtherSide);
								}
							}
						}
						if(!chessProperty.Tower)
							Instantiate(ringFX, new Vector3(territory.position.x, territory.position.y-0.2f, territory.position.z), Quaternion.identity);
					}
				}
			}
			if(!players.IsInsideTerritory(locationMap,playerSide)&& locationMap!=null){
				if(locationMap.GetComponent<Identy>().FixedSide == 3 || locationMap.GetComponent<Identy>().FixedSide == chessProperty.Player ){
					players.AddTerritory(locationMap,playerSide);
					if(players.IsInsideTerritory(locationMap,theOtherSide)){
						players.RemoveTerritory(locationMap,theOtherSide);
					}
					Instantiate(ringFX, new Vector3(locationMap.position.x, locationMap.position.y-0.2f, locationMap.position.z), Quaternion.identity);
				}
			}
			if(!chessProperty.Summoner)
				chess.GetComponent<CharacterPassive>().PassiveDict[PassiveType.Leader] = false;
			//play sound
			sysSound.PlaySound(SysSoundFx.Defense);
		}
		CleanMapsMat();
		updateAllCharactersPowers();
		CheckPrizes();
	}
	
	public void summonCommand(Transform chess, Transform gf){
		moveMode = false;
		attackMode = false;
		skillMode = false;
		neighbors.Clear();
		updateAllCharactersPowers();
		CharacterProperty chessP = chess.GetComponent<CharacterProperty>();
		if(chessP!=null && !chessP.death){
			CharacterSelect character = chess.GetComponent<CharacterSelect>();
			Transform currentPos = character.getMapPosition();
			if(currentPos!=null){
				//neighbors = chess.GetComponent<CharacterProperty>().GetSummonPosition();
				foreach(Transform map in players.GetTerritory(chess.GetComponent<CharacterProperty>().Player)){
					if(!MapHelper.IsMapOccupied(map))
						neighbors.Add(map);
				}
				currentGF = gf;
				if(MapHelper.CheckTowerOnSite() && gf.GetComponent<CharacterProperty>().Tower){
					IList mapsToRemove = MapHelper.GetTowerMaps();
					foreach(Transform map in mapsToRemove){
						if(neighbors.Contains(map)){
							neighbors.Remove(map);
						}
					}
				}
				if(neighbors.Count>0){
					foreach(Transform sixGon in neighbors){
						sixGon.renderer.material= GetCloseByMat(sixGon);
						if(!npcMode)
							networkView.RPC("RPCUpdateMat",RPCMode.Others,sixGon.name,3);
					}
					currentPos.renderer.material= GetRollOverMat(currentPos);
					if(!npcMode)
						networkView.RPC("RPCUpdateMat",RPCMode.Others,currentPos.name,2);
					selectMode = true;
					summonMode = true;
				}
			}
		}else if(chess == gf){
			ReviveCommand(chess);
		}
	}
	
	public void skillCommand(Transform skill){
		updateAllCharactersPowers();
		moveMode = false; 
		attackMode = false;
		summonMode = false;
		if(chess!=null){
			CharacterSelect character = chess.GetComponent<CharacterSelect>();
			Transform currentPos = character.getMapPosition();
			if(currentPos!=null){
				CommonSkill cSkill = skill.GetComponent(skill.GetComponent<SkillProperty>().ScriptName) as CommonSkill;
				neighbors = cSkill.GetSelectionRange();
				if(neighbors.Count>0){
					foreach(Transform sixGon in neighbors){
						sixGon.renderer.material= GetCloseByMat(sixGon);
						if(!npcMode)
							networkView.RPC("RPCUpdateMat",RPCMode.Others,sixGon.name,3);
					}
					currentPos.renderer.material= GetRollOverMat(currentPos);
					if(!npcMode)
						networkView.RPC("RPCUpdateMat",RPCMode.Others,currentPos.name,2);
					CurrentSkill = skill;
					selectMode = true;
					skillMode = true;
				}
			}
		}
	}
	
	public void RenderSkillRange(Transform gf){
		CleanSkillMapMat();
		CharacterSelect character = gf.GetComponent<CharacterSelect>();
		Transform currentPos = character.getMapPosition();
		Transform skill = gf.GetComponent<SkillSets>().Skills[0];
		if(currentPos!=null){
			CommonSkill cSkill = skill.GetComponent(skill.GetComponent<SkillProperty>().ScriptName) as CommonSkill;
			skillrollOverList = cSkill.GetSelectionRange();
			if(skillrollOverList.Count > 0){
				foreach(Transform sixGon in skillrollOverList){
					sixGon.renderer.material= rollOverSkill;
				}
			}
		}
	}
	
	public void CleanSkillMapMat(){
		if(skillrollOverList.Count>0){
			foreach(Transform map in skillrollOverList){
				map.renderer.material = GetOriginalMat(map);
			}
		}
		skillrollOverList.Clear();
	}
	
	
	public void ReviveCommand(Transform masterChess){
		reviveMode = true;
		neighbors.Clear();
		neighbors = GetAllEmptyMaps();
		if(neighbors.Count>0){
			foreach(Transform haxgon in neighbors){
				haxgon.renderer.material= GetCloseByMat(haxgon);
				if(!npcMode)
					networkView.RPC("RPCUpdateMat",RPCMode.Others,haxgon.name,3);
			}
			selectMode = true;
			summonMode = true;
			currentGF = masterChess;
		}
	}
	
	public void MoveToLayer(Transform character, int layer){
		if(character!=null){
			Transform models = character.FindChild("Models");
			if(models!=null && models.childCount>0){
				for(int i=0; i<models.childCount; i++){
					models.GetChild(i).gameObject.layer = layer;
				}
			}
		}
	}
	
	public IList GetAllEmptyMaps(){
		IList allMaps = new List<Transform>();
		Transform allMap = GameObject.Find("Maps").transform;
		for(int i=0; i<allMap.childCount; i++){
			if(!MapHelper.IsMapOccupied(allMap.GetChild(i))){
				allMaps.Add(allMap.GetChild(i));
			}
		}
		return allMaps;
	}
	
	public int GetSteps(Transform start, Transform dest){
		int step = 0;
		float dis = Vector3.Distance(start.transform.position, dest.transform.position);
		if(dis>13)
			step=3;
		else if(dis<13 && dis>7)
			step=2;
		else if(dis<7)
			step=1;
		return step;
	}
	
	public bool CheckIfShowBlood(){ 
		return inSelect; 
	}
	
    void Update() {
		if(sMachine.InitGame){
			mousePos.x = Input.mousePosition.x;
			mousePos.y = Screen.height-Input.mousePosition.y;
			if(!rUI.showUI && !noColArea.Contains(mousePos)){
				if(!sMachine.InBusy && !sMachine.TutorialBusy)
					selecting();
			}
			
			
			if(camMoveMode && MoveCam)
				translateMainCam(0.3f);
			if (Input.GetKeyDown(KeyCode.Return)) {  
	    		Application.LoadLevel(1);  
	  		}
			if(processPrize){
				if(!sMachine.TutorialMode){
					PlacePrizes();
				}
				processPrize = false;
			}
		}
    }
	
	public void PlacePrizes(){
		if(npcMode){
			pp.PlacePrize(pp.InitRed, pp.InitYel);
			CleanMapsMat();
		}else if(Network.peerType == NetworkPeerType.Server){
			pp.PlacePrize(pp.InitRed, pp.InitYel);
			CleanMapsMat();
			Transform allMap = GameObject.Find("Maps").transform;
			for(int i =0; i< allMap.childCount; i++){
				Identy id = allMap.GetChild(i).GetComponent<Identy>();
				networkView.RPC("RPCUpdateMapID", RPCMode.Others,allMap.GetChild(i).name,id.PrizeRed, id.PrizeYel);  
			}
			CleanMapsMat();
		}
	}
	
	public void CheckPrizes(){
		foreach(Transform map in players.PlayerATerritory){
			Identy mapID = map.GetComponent<Identy>();
			if(mapID.PrizeRed){
				mapID.PrizeRed = false;
				int num = ps.PrizeNum();
				Transform realPrize = ps.PlaceRealPrize(map, num);
				//store the prize into the map
				mapID.Prize = realPrize;
				pp.AddPrizeMap(map);
				
				AquirePrize ap = realPrize.GetComponent<AquirePrize>();
				int buffAmount = ap.GetBuffAmount(ap.PrizeType);
				ap.BuffAmount = buffAmount;
				if(!npcMode){
					networkView.RPC("RPCUpdateMapID", RPCMode.Others, map.name, mapID.PrizeRed, mapID.PrizeYel);
					networkView.RPC("UpdatePlacePrize", RPCMode.Others, map.name, num, buffAmount);
				}
			}
		}
		foreach(Transform map in players.PlayerBTerritory){
			Identy mapID = map.GetComponent<Identy>();
			if(mapID.PrizeYel){
				mapID.PrizeYel = false;
				int num = ps.PrizeNum();
				Transform realPrize = ps.PlaceRealPrize(map, num);
				//store the prize into the map
				mapID.Prize = realPrize;
				pp.AddPrizeMap(map);
				
				AquirePrize ap = realPrize.GetComponent<AquirePrize>();
				int buffAmount = ap.GetBuffAmount(ap.PrizeType);
				ap.BuffAmount = buffAmount;
				if(!npcMode){
					networkView.RPC("RPCUpdateMapID", RPCMode.Others, map.name, mapID.PrizeRed, mapID.PrizeYel);
					networkView.RPC("UpdatePlacePrize", RPCMode.Others, map.name, num, buffAmount);
				}
			}
		}
		CleanMapsMat();
	}
	
	Material GetOriginalMat(Transform map){
		Material closeMat = null;
		Identy id = map.GetComponent<Identy>();
		if(id.MapUnit){
			if(id.PrizeRed)
				closeMat = originalMat[1];
			else if(id.PrizeYel)
				closeMat = originalMat[2];
			else
				//closeMat = originalMat[0];
				closeMat = id.originalMat;
		}
		return closeMat;
	}
	
	Material GetCloseByMat(Transform map){
		Material closeMat = null;
		Identy id = map.GetComponent<Identy>();
		if(id.MapUnit){
			if(id.PrizeRed)
				closeMat = closeBy[1];
			else if(id.PrizeYel)
				closeMat = closeBy[2];
			else
				closeMat = closeBy[0];
		}
		return closeMat;
	}
	
	Material GetRollOverMat(Transform map){
		Material closeMat = null;
		Identy id = map.GetComponent<Identy>();
		if(id.MapUnit){
			if(id.PrizeRed)
				closeMat = rollOver[1];
			else if(id.PrizeYel)
				closeMat = rollOver[2];
			else
				closeMat = rollOver[0];
		}
		return closeMat;
	}
	
	Material GetEnemyMat(Transform map){
		Material closeMat = null;
		Identy id = map.GetComponent<Identy>();
		if(id.MapUnit){
			if(id.PrizeRed)
				closeMat = rollOverEnemy[1];
			else if(id.PrizeYel)
				closeMat = rollOverEnemy[2];
			else
				closeMat = rollOverEnemy[0];
		}
		return closeMat;
	}
	
	
	public void MoveCommandNetwork(){
		if(!npcMode){
			networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,2,true);
			networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,4,false);
			networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,3,false);
			networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,1,false);
		}
	}
	
	public void AttackCommandNetwork(){
		if(!npcMode){
			networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,1,true);
			networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,4,false);
			networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,3,false);
			networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,2,false);
		}
	}
	
	public void AttackActivate(Transform chess, Transform sel){
		attackerCal.CriticalHit = attackerCal.CalcriticalHit(chess,AttackType.physical);
		attackerCal.fightBack = true;
		attackerCal.SetAttackSequence(chess, sel);
		chessUI.Critical = attackerCal.CriticalHit;
		chessUI.DelayFadeOut = true;
		chessUI.TargetFadeIn = false;
		
		//update network
		if(!npcMode){
			networkView.RPC("RPCUpdateAttackResult", RPCMode.Others, chess.name, sel.name,attackerCal.CriticalHit);
			networkView.RPC("RPCUpdateInfoUIState",RPCMode.Others,true,true,attackerCal.CriticalHit);
			networkView.RPC("RPCUpdateMainInfoUI",RPCMode.Others,MapHelper.GetMapOccupiedObj(sel).name,false);
		}
	}
	
	public void DefenseNetwork(){
		CharacterProperty chessProperty = chess.GetComponent<CharacterProperty>();
		if(!npcMode){
			networkView.RPC("RPCCancelStatusUI",RPCMode.Others,chess.name);
			//networkView.RPC("RPCUpdateChessAttacked", RPCMode.Others, chess.name, true);
			//networkView.RPC("RPCUpdateChessMoved", RPCMode.Others, chess.name, true);
		}
		
		bool ifMolePassive = MapHelper.CheckPassive(PassiveType.DefenseAddOne,chess);			
		if(ifMolePassive){
			chess.GetComponent<BuffList>().ExtraDict[BuffType.Defense]+=1;
			if(!npcMode)
				networkView.RPC("RPCMolePassive",RPCMode.Others,chess.name,ifMolePassive);
			updateCharacterPowers(chess);
		}
	}
	
	public void EndTurnNetwork(){
		if(!npcMode){
			networkView.RPC("RPCCancelStatusUI",RPCMode.Others,chess.name);
			networkView.RPC("RPCUpdateChessEndTurn",RPCMode.Others,chess.name,true);
			networkView.RPC("RPCUpdateChessLayers",RPCMode.Others, chess.name,10);
			networkView.RPC("RPCUpdateTargetInfoUI",RPCMode.Others,chess.name,false);
		}
	}
	
	public void SummonNetwork(){
		if(!npcMode){
			networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,4,true);
			networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,1,false);
			networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,2,false);
			networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,3,false);
		}
	}
	
	public void SkillUINetwork(){
		if(!npcMode){
			networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,3,true);
			networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,1,false);
			networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,2,false);
			networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,4,false);
		}
	}
	
	public void SkillCmdNetwork(Transform chess, Transform skill){
		if(!npcMode){
			networkView.RPC("RPCUpdateChessSkillCmdA",RPCMode.Others,chess.name,skill.name,skill.GetComponent<SkillProperty>().PassSkillRate);
			networkView.RPC("RPCUpdateChessActivated",RPCMode.Others, chess.name,true);
			
		}
	}
	
	public bool AnimStateNetWork(Transform chess,AnimVault.AnimState state){
		Transform model = chess.FindChild("Models");
		if(model.GetComponent<AnimVault>()!=null){
			RPCUpdateAnimState(chess.name, state.ToString());
			if(!npcMode)
				networkView.RPC("RPCUpdateAnimState", RPCMode.Others, chess.name, state.ToString());
		}
		return model.GetComponent<AnimVault>()!=null; 
	}
	
	[RPC]
	void RPCUpdateChessPosition(string selName, string chessName){
		Transform selection = GameObject.Find(selName).transform;
		Transform chess = GameObject.Find(chessName).transform;
		CharacterProperty chessP = chess.GetComponent<CharacterProperty>();
		chess.position = selection.position;
		if(!chessP.Tower)
			chess.Translate(new Vector3(0.0f,1.5f,0.0f));
		else
			chess.Translate(new Vector3(0.0f,3.5f,0.0f));
	}
	
	[RPC]
	void RPCLookAtTarget(string selName, string targetName){
		Transform chess = GameObject.Find(selName).transform;
		Transform target = GameObject.Find(targetName).transform;
		chess.LookAt(target.transform.position);
	}
	
	[RPC]
	void RPCUpdateTerritory(string tANames, int side){
		if(tANames!=null){
			Transform map = GameObject.Find(tANames).transform;
			map.GetComponent<PlaneShadows>().ChangeShadowMaterial(side);
		}
		
	}
	[RPC]
	void RPCMolePassive(string chessName, bool passive){
		if(passive){
			Transform chess = GameObject.Find(chessName).transform;
			chess.GetComponent<BuffList>().ExtraDict[BuffType.Defense]+=1;
		}
	}
	
	[RPC]
	void RPCUpdateChessEndTurn(string chessName, bool endTurn){
		if(endTurn){
			Transform chess = GameObject.Find(chessName).transform;
			chess.GetComponent<CharacterProperty>().Moved = true;
			chess.GetComponent<CharacterProperty>().Attacked = true;
			chess.GetComponent<CharacterProperty>().Activated = true;
			chess.GetComponent<CharacterProperty>().CmdTimes = 0;
			chess.GetComponent<CharacterProperty>().TurnFinished = true;
		}
	}
	
	[RPC]
	void RPCUpdateRenderStatus(string chessName, bool status){
		Transform chess = GameObject.Find(chessName).transform;
		chess.renderer.enabled = status;
	}
	
	[RPC]
	void RPCUpdateChessSummon(string chessName){
		Transform chess = GameObject.Find(chessName).transform;
		CharacterProperty chessProperty = chess.GetComponent<CharacterProperty>();
		chessProperty.Hp = chessProperty.MaxHp;
		chessProperty.death = false;
		chessProperty.Attacked = true;
		chessProperty.Activated = true;
		chessProperty.Moved = true;
		chessProperty.CmdTimes = 0;
		
		Transform[] gfSkills = chess.GetComponent<SkillSets>().Skills;
		foreach(Transform skill in gfSkills){
			skill.GetComponent<SkillProperty>().DefaultCDRounds();
		}
		
		foreach(BuffType Buff in Enum.GetValues(typeof(BuffType))){
			chess.GetComponent<BuffList>().ExtraDict[Buff] = 0;
		}
		CharacterPassive chessPassive = chess.GetComponent<CharacterPassive>();
		foreach(PassiveType passive in Enum.GetValues(typeof(PassiveType))){
			chessPassive.PassiveDict[passive] = false;
		}
		if((chessPassive.PassiveAbility!=null) && (chessPassive.PassiveAbility.Length>0)){
			foreach(PassiveType p in chessPassive.PassiveAbility){
				chessPassive.PassiveDict[p] = true;
			}
		}
	}
	
	[RPC]
	void RPCUpdateChessPower(string chessName){
		Transform chess = GameObject.Find(chessName).transform;
		BuffCalculation buffCal = new BuffCalculation(chess);
		buffCal.UpdateBuffValue();
	}
	
	[RPC]
	void RPCUpdateChessAttacked(string chessName, bool attacked){
		Transform chess = GameObject.Find(chessName).transform;
		CharacterProperty chessProperty = chess.GetComponent<CharacterProperty>();
		chessProperty.Attacked = attacked;
	}
	
	[RPC]
	void RPCUpdateChessActivated(string chessName, bool activated){
		Transform chess = GameObject.Find(chessName).transform;
		CharacterProperty chessProperty = chess.GetComponent<CharacterProperty>();
		chessProperty.Activated = activated;
		chessProperty.CmdTimes -=1;
	}
	
	[RPC]
	void RPCUpdateChessMoved(string chessName, bool moved){
		Transform chess = GameObject.Find(chessName).transform;
		CharacterProperty chessProperty = chess.GetComponent<CharacterProperty>();
		chessProperty.Moved = moved;
	}
	
	[RPC]
	void RPCUpdateAttackResult(string chessName, string selName, bool state){
		Transform selection = GameObject.Find(selName).transform;
		Transform chess = GameObject.Find(chessName).transform;
		attackerCal.CriticalHit = state;
		attackerCal.fightBack = true;
		attackerCal.SetAttackSequence(chess,selection);
	}
	
	[RPC]
	void RPCUpdateChessLayers(string chessName, int index){
		Transform chess = GameObject.Find(chessName).transform;
		chess.gameObject.layer = index;
		MoveToLayer(chess,index);
	}
	
	[RPC]
	void RPCUpdateChessSkillCmd(string chessName,string skillName, string selName, bool state){
		Transform selection = GameObject.Find(selName).transform;
		Transform chess = GameObject.Find(chessName).transform;
		Transform currentSkill = null;
		Transform[] skills = chess.GetComponent<SkillSets>().Skills;
		foreach(Transform skill in skills){
			if(skill.name == skillName)
				currentSkill = skill;
		}
		if(currentSkill!=null){
			CommonSkill cSkill = currentSkill.GetComponent(currentSkill.GetComponent<SkillProperty>().ScriptName) as CommonSkill;
			currentSkill.GetComponent<SkillProperty>().PassSkillRate = state;
			cSkill.InsertSelection(selection);
			cSkill.Execute();
			currentSkill.GetComponent<SkillProperty>().DefaultCDRounds();
		}
	}
	
	[RPC]
	void RPCUpdateChessSkillCmdA(string chessName,string skillName, bool state){
		Transform chess = GameObject.Find(chessName).transform;
		Transform currentSkill = null;
		Transform[] skills = chess.GetComponent<SkillSets>().Skills;
		foreach(Transform skill in skills){
			if(skill.name == skillName)
				currentSkill = skill;
		}
		if(currentSkill!=null){
			currentSkill.GetComponent<SkillProperty>().PassSkillRate = state;
			currentSkill.GetComponent<SkillProperty>().ActivateSkill();
			currentSkill.GetComponent<SkillProperty>().DefaultCDRounds();
		}
	}
	
	[RPC]
	void RPCUpdateMat(string selName, int mat){
		Transform selection = GameObject.Find(selName).transform;
		if(mat==1){
			selection.renderer.material = GetOriginalMat(selection);
		}else if(mat==2){
			selection.renderer.material = GetRollOverMat(selection);
		}else if(mat==3){
			selection.renderer.material = GetCloseByMat(selection);
		}else if(mat==4){
			selection.renderer.material = GetEnemyMat(selection);
		}
	}
	
	[RPC]
	void RPCUpdateMapID(string selName, bool red, bool yel){
		Transform selection = GameObject.Find(selName).transform;
		Identy selID = selection.GetComponent<Identy>();
		selID.PrizeRed = red; 
		selID.PrizeYel = yel;
	}
	
	[RPC]
	void RPCCancelStatusUI(string chessName){
		Transform selection = GameObject.Find(chessName).transform;
		statusUI.Chess = selection;
		statusUI.Attacking = false;
		statusUI.Moving = false;
		statusUI.Summoning = false;
		statusUI.Skilling = false;
	}
	
	[RPC]
	void RPCUpdateStatusUI(string chessName, int mode,bool state ){
		Transform selection = GameObject.Find(chessName).transform;
		statusUI.Chess = selection;
		switch(mode){
			case 1:
				statusUI.Attacking = state;
				break;
			case 2:
				statusUI.Moving = state;
				break;
			case 3:
				statusUI.Skilling = state;
				break;
			case 4:
				statusUI.Summoning = state;
				break;
		}
	}
	
	[RPC]
	void RPCUpdateChessStatus(string chessName, int mode, int val){
		Transform selection = GameObject.Find(chessName).transform;
		if(selection!=null){
			CharacterProperty selProperty = selection.GetComponent<CharacterProperty>();
			switch(mode){
				case 0:
					selProperty.BuffMoveRange = val;
					break;
				case 1:
					selProperty.BuffAtkRange = val;
					break;
				case 2:
					selProperty.Damage = val;
					break;
				case 3:
					selProperty.Hp = val;
					break;
				case 4:
					selProperty.BuffCriticalHit = val;
					break;
				case 5:
					selProperty.BuffSkillRate = val;
					break;
				case 6:
					selProperty.WaitRounds = val;
					break;
			}
		}
	}
	
	[RPC]
	void RPCUpdateChessUnnormal(string chessName, int mode, int val){
		Transform selection = GameObject.Find(chessName).transform;
		if(selection!=null){
			CharacterProperty selProperty = selection.GetComponent<CharacterProperty>();
			switch(mode){
				case 0:
					selProperty.UnStatusCounter[UnnormalStatus.Burned] = val;
					break;
				case 1:
					selProperty.UnStatusCounter[UnnormalStatus.Chaos] = val;
					break;
				case 2:
					selProperty.UnStatusCounter[UnnormalStatus.Freezed] = val;
					break;
				case 3:
					selProperty.UnStatusCounter[UnnormalStatus.Poisoned] = val;
					break;
				case 4:
					selProperty.UnStatusCounter[UnnormalStatus.Sleeping] = val;
					break;
				case 5:
					selProperty.UnStatusCounter[UnnormalStatus.Wounded] = val;
					break;
			}
		}
	}
	
	[RPC]
	void RPCUpdateChessLastUnnormal(string chessName, int mode, int val){
		Transform selection = GameObject.Find(chessName).transform;
		if(selection!=null){
			CharacterProperty selProperty = selection.GetComponent<CharacterProperty>();
			switch(mode){
				case 0:
					selProperty.LastUnStatusCounter[UnnormalStatus.Burned] = val;
					break;
				case 1:
					selProperty.LastUnStatusCounter[UnnormalStatus.Chaos] = val;
					break;
				case 2:
					selProperty.LastUnStatusCounter[UnnormalStatus.Freezed] = val;
					break;
				case 3:
					selProperty.LastUnStatusCounter[UnnormalStatus.Poisoned] = val;
					break;
				case 4:
					selProperty.LastUnStatusCounter[UnnormalStatus.Sleeping] = val;
					break;
				case 5:
					selProperty.LastUnStatusCounter[UnnormalStatus.Wounded] = val;
					break;
			}
		}
	}
	
	[RPC]
	void RPCUpdateMainInfoUI(string chessName, bool show){
		if(show){
			Transform chess = GameObject.Find(chessName).transform;
			chessUI.InsertChess(chess);
			mainUI.IgnoreMainUI = true;
		}
		chessUI.MainFadeIn = show;
	}
	
	[RPC]
	void RPCUpdateTargetInfoUI(string chessName, bool show){
		if(show){
			Transform chess = GameObject.Find(chessName).transform;
			if(chessUI.playerSide != chess.GetComponent<CharacterProperty>().Player){
				chessUI.InsertTargetChess(chess);
				mainUI.IgnoreMainUI = true;
				chessUI.TargetFadeIn = show;
			}
		}
		if(!show)
			chessUI.TargetFadeIn = show;
	}
	
	[RPC]
	void RPCUpdateInfoUIState(bool target, bool delay, bool critiq){
		chessUI.DelayFadeOut = delay; 
		if(!target)
			chessUI.Critical = critiq;
		else
			chessUI.CriticalRight = critiq;
	}
	
	[RPC]
	void RPCShowUp(string chessName){
		Transform chess = GameObject.Find(chessName).transform;
		currentGF = chess;
	}
	
	[RPC]
	void RPCSummonIn(string chessName, string selName){
		Transform chess = GameObject.Find(chessName).transform;
		Transform sel = GameObject.Find(selName).transform;
		Vector3 fxPos = new Vector3(sel.transform.position.x,sel.transform.position.y+0.1f,sel.transform.position.z);
		Transform flashIn = Instantiate(fxVault.SummonIn,fxPos,Quaternion.identity) as Transform;
		Destroy(GameObject.Find(flashIn.name).gameObject, 3.0f);
		if(chess.GetComponent<CharacterProperty>().Player==1)
			MapHelper.SetObjTransparent(chess,red,0.0f);
		else
			MapHelper.SetObjTransparent(chess,yellow,0.0f);
		SummonIn = true;
		t = 0;
	}
	
	[RPC]
	void RPCUpdateRevive(bool state){
		reviveMode = state;
	}
	
	[RPC]
	void RPCUpdateAnimState(string chessName, string state){
		Transform chess = GameObject.Find(chessName).transform;
		Transform model = chess.FindChild("Models");
		AnimVault.AnimState newState = AnimVault.AnimState.attack;
		switch(state){
			case "attack":
				newState = AnimVault.AnimState.attack;
				break;
			case "idle":
				newState = AnimVault.AnimState.idle;
				break;
			case "run":
				newState = AnimVault.AnimState.run;
				break;
			case "skill":
				newState = AnimVault.AnimState.skill;
				break;
		}
		model.GetComponent<AnimVault>().CurrentState = newState;
	}
	
	[RPC]
	void UpdateTurningHead(string chessName, string selName, bool critiq){
		Transform chess = GameObject.Find(chessName).transform;
		Transform sel = GameObject.Find(selName).transform;
		TurnHead th = Camera.mainCamera.GetComponent<TurnHead>();
		th.SetTurnHeadSequence(chess, sel, true, false, critiq);
	}
	
	[RPC]
	void UpdateMoveCharacter(string chessName, string selName){
		Transform chess = GameObject.Find(chessName).transform;
		Transform sel = GameObject.Find(selName).transform;
		
		CharacterSelect chessSelect = chess.GetComponent<CharacterSelect>();
		Transform localUnit = chessSelect.getMapPosition();
		IList pathList = new List<Transform>();
		pathList = chessSelect.FindPathList(localUnit,GetSteps(localUnit,sel),sel);
		MoveCharacter mc = Camera.mainCamera.GetComponent<MoveCharacter>();
		mc.SetSteps(chess,pathList);
	}
	
	[RPC]
	void UpdatePlacePrize(string mapName, int prizeNum, int buffAmount){
		Transform map = GameObject.Find(mapName).transform;
		Transform realPrize = ps.PlaceRealPrize(map, prizeNum);
		realPrize.GetComponent<AquirePrize>().BuffAmount = buffAmount;
	}
	
	[RPC]
	void UpdateMapFixedSide(string mapName, int fixedSide){
		Transform map = GameObject.Find(mapName).transform;
		Identy mapID = map.GetComponent<Identy>();
		mapID.FixedSide = fixedSide;
	}
}
