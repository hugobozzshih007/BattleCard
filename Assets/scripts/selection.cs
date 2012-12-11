using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MapUtility;
using BuffUtility;

public class selection : MonoBehaviour {
	Transform sel;
	public LayerMask MaskMap;
	public LayerMask MaskCharaceter;
	public Transform chess = null;
	Transform oldChess = null;
	IList neighbors = new List<Transform>();
	Transform[] summonArea;
	private Transform selOld;
	private SixGonRays unit;
	public Material originalMat;
	private bool moveMode = false;
	private bool summonMode = false;
	public bool selectMode = false;
	private bool attackMode = false;
	private bool skillMode = false;
	public bool APlaying = false;
	public bool BPlaying = false;
	public bool Playing = false;
	public Material rollOver;
	public Material closeBy;
	public Material rollOverFlag;
	public Material closeByFlag;
	public Material FlagMat;
	public float castLength = 80.0f;
	public int steps = 2;
	private bool viewScreenPanning = false;
	private float viewScreenOldMouseX;
	private float viewScreenOldMouseY;
	private float viewScreenZoomHeight = 50.0f;
	private float viewScreenPanSpeed = 2000.0f;
	private const float viewOffsetZ = 30.0f;
	private Vector3 oldCamPosition;
	private Vector3 newCamPosition;
	private Transform currentGF;
	//public Transform AttackTarget;
	public bool guiShow = false;
	bool summonList = false;
	bool skillList = false;
	bool summoner = false;
	bool camMoveMode = false;
	bool infoUI = false;
	int guiSegX = 30;
	int guiSegY = 20;
	int guiSeg = 10;
	int camStep = 0;
	Vector3 screenPos;
	public Transform player, CurrentSkill;
	MainUI mainUI;
	bool tower;
	StatusUI statusUI;
	RoundCounter players;
	MainInfoUI chessUI;
	
	void Start(){
		sel = GameObject.Find("unit_start_point_A").transform;
		selOld = sel;
		players = Camera.mainCamera.GetComponent<RoundCounter>();
		moveMode = false;
		summonMode = false;
		selectMode = false;
		statusUI = transform.GetComponent<StatusUI>();
		mainUI = transform.GetComponent<MainUI>();
		chessUI = transform.GetComponent<MainInfoUI>();
	}
	
	void Awake(){
		 Application.targetFrameRate = 1000;
		if(Network.peerType == NetworkPeerType.Server){
			APlaying = true;
			BPlaying = false;
			Playing = APlaying;
		}else if(Network.peerType == NetworkPeerType.Client){
			print("I'm client");
			APlaying = false;
			BPlaying = false;
			Playing = APlaying;
		}
	}
	
	void panning(){
		if(mainUI.Cancel){
			selectMode = false;
			moveMode = false;
			attackMode = false;
			summonMode = false;
			skillMode = false;
			if(neighbors!=null){
				foreach(Transform map in neighbors){
					if(map!=null)
						map.renderer.material = originalMat;
				}
			}
			updateTerritoryMat();
		}
		
		if(Input.GetMouseButtonDown(1)) {
			//guiShow = false;
			mainUI.MainGuiFade = false;
			mainUI.SubGuiFade = false;
			chessUI.MainFadeIn = false;
			chessUI.TargetFadeIn = false;
			//chessUI.SetChessesNull();
			selectMode = false;
			moveMode = false;
			attackMode = false;
			summonMode = false;
			summonList = false;
			skillList = false;
			skillMode = false;
			
			if(chess != null){
				chess.gameObject.layer = 10;
				MoveToLayer(chess,10);
				if(Playing){
					networkView.RPC("RPCCancelStatusUI",RPCMode.Others,chess.name);
					networkView.RPC("RPCUpdateChessLayers",RPCMode.Others,chess.name,10);
					networkView.RPC("RPCUpdateTargetInfoUI",RPCMode.Others,chess.name,false);
				}
			}
    		viewScreenPanning = true;
    		viewScreenOldMouseX = Input.mousePosition.x;
    		viewScreenOldMouseY = Input.mousePosition.y;
			if(neighbors!=null){
				foreach(Transform map in neighbors){
					if(map!=null)
						map.renderer.material = originalMat;
				}
			}
			updateTerritoryMat();
			//neighbors.Clear();
		}
		if(viewScreenPanning==true) {
		    if(Input.GetMouseButtonUp(1)) {
		        viewScreenPanning = false;
		    }
		    Vector3 viewScreenPanVector = transform.TransformPoint(new Vector3((viewScreenOldMouseX - Input.mousePosition.x) / viewScreenPanSpeed, 0.0f, (viewScreenOldMouseY - Input.mousePosition.y) / viewScreenPanSpeed));
		    // since we use a quick and dirty transform, reset the camera height to what it was
		    viewScreenPanVector.y = viewScreenZoomHeight; 
		    transform.position = viewScreenPanVector;
		}
	}
	
	void alignCamera(Transform focus){
		Vector3 newCamPosition = new Vector3(focus.position.x, viewScreenZoomHeight, focus.position.z-viewOffsetZ);
		transform.position = newCamPosition;
		camMoveMode = true;
	}
	
	void translateMainCam(int segment){
		float segX = (oldCamPosition.x-newCamPosition.x)/segment;
		float segZ = (oldCamPosition.z-newCamPosition.z)/segment;
		transform.position = new Vector3(transform.position.x-segX,viewScreenZoomHeight,transform.position.z-segZ);
		camStep+=1;
		if(camStep==segment){
			camMoveMode = false;
			camStep = 0;
			screenPos = Camera.main.WorldToScreenPoint(chess.position);
			screenPos.y = Screen.height - screenPos.y;
		}
	}
	
	//select map or character
	void selecting(){
		if(Input.GetMouseButtonDown(0) && !selectMode){
			//if(chess != null){
			//	chess.gameObject.layer = 10;
			//}
			Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        	Debug.DrawRay(ray.origin, ray.direction * castLength, Color.yellow);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, castLength,MaskCharaceter)){
				if(hit.transform.GetComponent<Identy>() != null){
					if(!hit.transform.GetComponent<Identy>().MapUnit){
						chess = hit.transform;
						//update layer
						chess.gameObject.layer = 11;
						MoveToLayer(chess,11);
						mainUI.IgnoreMainUI = false;
						chessUI.InsertChess(chess);
						chessUI.MainFadeIn = true;
						chessUI.Critical = false;
						if(Playing){
							networkView.RPC("RPCUpdateChessLayers",RPCMode.Others,chess.name,11);
							networkView.RPC("RPCUpdateTargetInfoUI", RPCMode.Others,chess.name,true);
						}
						
						if(chess!=oldChess && oldChess!=null){
							oldChess.gameObject.layer = 10;
							MoveToLayer(oldChess,10);
							if(Playing)
								networkView.RPC("RPCUpdateChessLayers",RPCMode.Others,oldChess.name,10);
						}
						oldChess = chess;
						
						newCamPosition = new Vector3(chess.position.x, viewScreenZoomHeight, chess.position.z-viewOffsetZ);
						oldCamPosition = transform.position;
						Vector3 diffCamPos = newCamPosition-oldCamPosition;
						if(Math.Abs(diffCamPos.x)>25 || Math.Abs(diffCamPos.z)>20){
							camMoveMode = true;
						}else{
							screenPos = Camera.main.WorldToScreenPoint(chess.position);
							screenPos.y = Screen.height - screenPos.y;
						}
						if(chess.GetComponent<CharacterProperty>().Player>1){
								player = players.playerB;
							}else{
								player = players.playerA;
						}
						CharacterProperty chessProperty = chess.GetComponent<CharacterProperty>();
						if(chessProperty.TurnFinished /*|| !Playing*/){
							//guiShow = false;
							mainUI.MainGuiFade = false;
							mainUI.IgnoreMainUI = true;
							mainUI.SubGuiFade = false;
							chessUI.InsertChess(chess);
							chessUI.MainFadeIn = true;
							chessUI.TargetFadeIn = true;
							//this.GetComponent<InfoUI>().infoUI = true;
						}else /*if(Playing)*/{
							//guiShow = true;
							if(!chessProperty.TurnFinished){
								mainUI.MainGuiFade = true;
							}else{
								mainUI.MainGuiFade = false;
							}
						}
						summoner = chess.GetComponent<CharacterProperty>().Summoner;
					}
				}
			}
		}
		
		if(selectMode){
			Ray ray = camera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, castLength,MaskMap)){
				if(hit.transform.GetComponent<Identy>()!=null){
					if(hit.transform.GetComponent<Identy>().MapUnit){
						sel = hit.transform;
						if(neighbors.Contains(sel)){
							
							if(sel.GetComponent<Identy>().Flag){
								sel.renderer.material = rollOverFlag;
							}else{
								sel.renderer.material = rollOver;
								if(MapHelper.IsMapOccupied(sel)){
									Transform targetChess = MapHelper.GetMapOccupiedObj(sel);
									mainUI.IgnoreMainUI = true;
									chessUI.InsertChess(targetChess);
									chessUI.TargetFadeIn = true;
									networkView.RPC("RPCUpdateMainInfoUI",RPCMode.Others,targetChess.name,true);
								}else{
									mainUI.IgnoreMainUI = false;
									chessUI.TargetFadeIn = false;
									networkView.RPC("RPCUpdateMainInfoUI",RPCMode.Others,sel.name,false);
								}
								networkView.RPC("RPCUpdateMat",RPCMode.Others,sel.name,3);
							}
							
							if(selOld!=sel && neighbors.Contains(selOld)){
								if(selOld.GetComponent<Identy>().Flag){
									selOld.renderer.material = closeByFlag;
								}else{
									selOld.renderer.material = closeBy;
									mainUI.IgnoreMainUI = false;
									chessUI.TargetFadeIn = false;
									networkView.RPC("RPCUpdateMainInfoUI",RPCMode.Others,sel.name,false);
									networkView.RPC("RPCUpdateMat",RPCMode.Others,selOld.name,2);
								}
							}
							selOld = sel;
							if(Input.GetMouseButtonDown(0)){
								if(moveMode){
									Transform localUnit = chess.GetComponent<CharacterSelect>().getMapPosition();
									localUnit.renderer.material = localUnit.GetComponent<Identy>().originalMat;
									chess.position = sel.position;
									chess.Translate(new Vector3(0.0f,1.5f,0.0f));
									
									foreach(Transform sixGons in neighbors){
										if(sixGons.GetComponent<Identy>().Flag){
											sixGons.renderer.material = FlagMat;
										}else{
											sixGons.renderer.material = originalMat;
											networkView.RPC("RPCUpdateMat",RPCMode.Others,sixGons.name,1);
										}
									}
									
									chess.GetComponent<CharacterProperty>().Moved = true;
									chess.GetComponent<CharacterSelect>().MoveRangeList.Clear();
									
									/*
									selOld.renderer.material = originalMat;
									networkView.RPC("RPCUpdateMat",RPCMode.Others,selOld.name,1);
									sel.renderer.material = originalMat;
									networkView.RPC("RPCUpdateMat",RPCMode.Others,sel.name,1);
									*/
									
									updateTerritoryMat();
									
									updateCharacterPowers(chess);
									//updateMapSteps(); 
									
									//update network
									networkView.RPC("RPCUpdateChessPosition",RPCMode.Others,sel.name,chess.name);
									networkView.RPC("RPCUpdateChessMoved",RPCMode.Others,chess.name,true);
									moveMode = false;
									networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,2,false);
									//chess.gameObject.layer = 10;
									//networkView.RPC("RPCUpdateChessLayers",RPCMode.Others ,chess.name,10);
									
									//update UI
									mainUI.SubGuiFade = false;
									if(chess.GetComponent<CharacterProperty>().Attacked && chess.GetComponent<CharacterProperty>().Activated && chess.GetComponent<CharacterProperty>().Moved){
										chess.GetComponent<CharacterProperty>().TurnFinished = true;
										chess.gameObject.layer = 10;
										MoveToLayer(chess,10);
										EndTurnNetwork();
									}
								} 
								if(summonMode){
									chess.GetComponent<CharacterSelect>().getMapPosition().renderer.material = originalMat;
									currentGF.position = sel.position;
									currentGF.Translate(new Vector3(0.0f,1.5f,0.0f));
									currentGF.renderer.enabled = true;
									currentGF.GetComponent<CharacterProperty>().Hp = currentGF.GetComponent<CharacterProperty>().defPower;
									currentGF.GetComponent<CharacterProperty>().death = false;
									currentGF.GetComponent<CharacterProperty>().Attacked = true;
									currentGF.GetComponent<CharacterProperty>().Activated = true;
									currentGF.GetComponent<CharacterProperty>().Moved = true;
									currentGF.GetComponent<CharacterProperty>().TurnFinished = true;
									player.GetComponent<ManaCounter>().Mana-=currentGF.GetComponent<CharacterProperty>().summonCost;
									foreach(Transform sixGons in neighbors){
										if(sixGons.GetComponent<Identy>().Flag){
											sixGons.renderer.material = FlagMat;
										}else{
											sixGons.renderer.material = originalMat;
											networkView.RPC("RPCUpdateMat",RPCMode.Others,sixGons.name,1);
										}
									}
									summonMode = false;
									
									//update network
									networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,4,false);
									networkView.RPC("RPCUpdateChessPosition",RPCMode.Others,sel.name,currentGF.name);
									networkView.RPC("RPCUpdateRenderStatus",RPCMode.Others,currentGF.name, currentGF.renderer.enabled);
									networkView.RPC("RPCUpdateChessSummon",RPCMode.Others,currentGF.name);
									networkView.RPC("RPCUpdateMana",RPCMode.Others,player.GetComponent<ManaCounter>().Mana,currentGF.GetComponent<CharacterProperty>().Player );
									
									updateCharacterPowers(currentGF);
									
									/*
									selOld.renderer.material = originalMat;
									networkView.RPC("RPCUpdateMat",RPCMode.Others,selOld.name,1);
									sel.renderer.material = originalMat;
									networkView.RPC("RPCUpdateMat",RPCMode.Others,sel.name,1);
									*/
									updateTerritoryMat();
									
									//chess.gameObject.layer = 10;
									//networkView.RPC("RPCUpdateChessLayers",RPCMode.Others, chess.name,10);
									
									// update UI
									mainUI.SubGuiFade = false;
									mainUI.MainGuiFade = true;
								}
								if(attackMode){
									chess.GetComponent<CharacterSelect>().getMapPosition().renderer.material = originalMat;
									AttackCalculation attackerCal = new AttackCalculation(chess);
									attackerCal.InsertTarget(sel);
									attackerCal.CriticalHit = attackerCal.CalcriticalHit(AttackType.physical);
									attackerCal.UpdateAttackResult(AttackType.physical);
									chessUI.Critical = attackerCal.CriticalHit;
									chessUI.DelayFadeOut = true;
									chessUI.TargetFadeIn = false;
									
									//update network
									networkView.RPC("RPCUpdateAttackResult", RPCMode.Others, chess.name, sel.name,attackerCal.CriticalHit);
									networkView.RPC("RPCUpdateChessAttacked", RPCMode.Others, chess.name, true);
									//networkView.RPC("RPCUpdateMainInfoUI",RPCMode.Others,MapHelper.GetMapOccupiedObj(sel).name,true);
									networkView.RPC("RPCUpdateInfoUIState",RPCMode.Others,true,true,attackerCal.CriticalHit);
									networkView.RPC("RPCUpdateMainInfoUI",RPCMode.Others,MapHelper.GetMapOccupiedObj(sel).name,false);
									
									
									//default map steps before get new attackpositions
									//updateMapSteps();
									Transform currentTarget = MapHelper.GetMapOccupiedObj(sel);

									if(!currentTarget.GetComponent<CharacterProperty>().death){
										//fight back
										AttackCalculation currentTargetAtk = new AttackCalculation(currentTarget);
										bool atkable = currentTargetAtk.GetAttableTarget(currentTarget).Contains(chess.GetComponent<CharacterSelect>().getMapPosition());
										
										if(atkable){
											currentTargetAtk.InsertTarget(chess.GetComponent<CharacterSelect>().getMapPosition());
											currentTargetAtk.CriticalHit = currentTargetAtk.CalcriticalHit(AttackType.physical);
											chessUI.CriticalRight = currentTargetAtk.CriticalHit;
											currentTargetAtk.UpdateAttackResult(AttackType.physical);
											if(chess.GetComponent<CharacterProperty>().Hp<=0){
												chessUI.DelayFadeOut = true;
												chessUI.MainFadeIn = false;
											}
											networkView.RPC("RPCUpdateAttackResult", RPCMode.Others, currentTarget.name, chess.name,currentTargetAtk.CriticalHit);
											networkView.RPC("RPCUpdateInfoUIState",RPCMode.Others,false,true,currentTargetAtk.CriticalHit);
										}
									}
									
									
									
									foreach(Transform sixGons in neighbors){
										if(sixGons.GetComponent<Identy>().Flag){
											sixGons.renderer.material = FlagMat;
										}else{
											sixGons.renderer.material = originalMat;
											networkView.RPC("RPCUpdateMat",RPCMode.Others,sixGons.name,1);
										}
									}
									
									chess.GetComponent<CharacterSelect>().AttackRangeList.Clear();
									chess.GetComponent<CharacterProperty>().Attacked = true;
									
									attackMode = false;
									networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,1,false);
									//updateMapSteps();
									/*
									selOld.renderer.material = originalMat;
									networkView.RPC("RPCUpdateMat",RPCMode.Others,selOld.name,1);
									sel.renderer.material = originalMat;
									networkView.RPC("RPCUpdateMat",RPCMode.Others,sel.name,1);
									*/
									updateTerritoryMat();
									
									//chess.gameObject.layer = 10;
									//networkView.RPC("RPCUpdateChessLayers",RPCMode.Others, chess.name,10);
									// update UI
									mainUI.SubGuiFade = false;
									if(chess.GetComponent<CharacterProperty>().Attacked && chess.GetComponent<CharacterProperty>().Activated && chess.GetComponent<CharacterProperty>().Moved){
										chess.GetComponent<CharacterProperty>().TurnFinished = true;
										chess.gameObject.layer = 10;
										MoveToLayer(chess,10);
										chessUI.DelayFadeOut = true;
										chessUI.MainFadeIn = false;
										EndTurnNetwork();
									}
									
								}
								if(skillMode){
									if(CurrentSkill!=null){
										CurrentSkill.GetComponent<SkillProperty>().GetRealSkillRate();
										CurrentSkill.GetComponent<SkillProperty>().PassSkillRate = MapHelper.Success(CurrentSkill.GetComponent<SkillProperty>().SkillRate);
										CommonSkill cSkill = CurrentSkill.GetComponent(CurrentSkill.GetComponent<SkillProperty>().ScriptName) as CommonSkill;
										cSkill.InsertSelection(sel);
										cSkill.Execute();
										skillMode = false;
										networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,3,false);
										player.GetComponent<ManaCounter>().Mana -= CurrentSkill.GetComponent<SkillProperty>().SkillCost;
										chess.GetComponent<CharacterProperty>().Activated = true;
										
										foreach(Transform sixGons in neighbors){
											if(sixGons.GetComponent<Identy>().Flag){
												sixGons.renderer.material = FlagMat;
											}else{
												sixGons.renderer.material = originalMat;
												networkView.RPC("RPCUpdateMat",RPCMode.Others,sixGons.name,1);
											}
										}
										
										chessUI.DelayFadeOut = true;
										chessUI.TargetFadeIn = false;
										
										// update network
										networkView.RPC("RPCUpdateChessActivated",RPCMode.Others, chess.name,true);
										networkView.RPC("RPCUpdateChessSkillCmd", RPCMode.Others, chess.name,CurrentSkill.name,sel.name,CurrentSkill.GetComponent<SkillProperty>().PassSkillRate);
										networkView.RPC("RPCUpdateMana",RPCMode.Others,player.GetComponent<ManaCounter>().Mana,player.GetComponent<CharacterProperty>().Player);
										skillList = false;
										/*
										selOld.renderer.material = originalMat;
										networkView.RPC("RPCUpdateMat",RPCMode.Others,selOld.name,1);
										sel.renderer.material = originalMat;
										networkView.RPC("RPCUpdateMat",RPCMode.Others,sel.name,1);
										*/
										updateTerritoryMat();
									}else{
										print("Where is current skill???????????");
									}
									//chess.gameObject.layer = 10;
									//networkView.RPC("RPCUpdateChessLayers",RPCMode.Others, chess.name,10);
									
									// update UI
									mainUI.SubGuiFade = false;
									if(chess.GetComponent<CharacterProperty>().Attacked && chess.GetComponent<CharacterProperty>().Activated && chess.GetComponent<CharacterProperty>().Moved){
										chess.GetComponent<CharacterProperty>().TurnFinished = true;
										chess.gameObject.layer = 10;
										MoveToLayer(chess,10);
										EndTurnNetwork();
									}
								}
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
		
		foreach(Transform territory in players.GetTerritory(1)){
			aTNames.Add(territory.name);
			networkView.RPC("RPCUpdateTerritory",RPCMode.Others,territory.name, 1);
			territory.GetComponent<Identy>().originalMat = players.TerritoryA;
			territory.renderer.material = territory.GetComponent<Identy>().originalMat;
		}
		foreach(Transform territory in players.GetTerritory(2)){
			bTNames.Add(territory.name);
			networkView.RPC("RPCUpdateTerritory",RPCMode.Others,territory.name, 2);
			territory.GetComponent<Identy>().originalMat = players.TerritoryB;
			territory.renderer.material = territory.GetComponent<Identy>().originalMat;
		}
	}
	
	void updateMapSteps(){
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
		networkView.RPC("RPCUpdateChessPower",RPCMode.Others,character.name);
	}
	
	public void updateAllCharactersPowers(){
		foreach(Transform character in players.AllChesses){
			updateCharacterPowers(character);
		}
	}
	
	public void moveCommand(Transform chess){
		if(chess!=null){
			updateMapSteps();
			CharacterSelect character = chess.GetComponent<CharacterSelect>();
			Transform currentPos = character.getMapPosition();
			if(currentPos!=null){
				character.findMoveRange(currentPos,0,chess.GetComponent<CharacterProperty>().BuffMoveRange);
				neighbors = character.MoveRangeList;
				if(neighbors.Count>0){
					foreach(Transform sixGon in neighbors){
						if(sixGon.GetComponent<Identy>().Flag){
							sixGon.renderer.material = closeByFlag;
						}else{
							sixGon.renderer.material = closeBy;
							networkView.RPC("RPCUpdateMat",RPCMode.Others,sixGon.name,2);
							
						}
					}
					currentPos.renderer.material = rollOver;
					networkView.RPC("RPCUpdateMat",RPCMode.Others,currentPos.name,3);
					selectMode = true;
					moveMode =true;
				}else{
					chess.GetComponent<CharacterProperty>().Moved = true;
				}
			}
		}
	}
	
	public void attackCommand(Transform chess){
		if(chess!=null){
			//updateMapSteps();
			AttackCalculation attackerCal = new AttackCalculation(chess);
			CharacterSelect character = chess.GetComponent<CharacterSelect>();
			Transform currentPos = character.getMapPosition();
			if(currentPos!=null){
				neighbors = attackerCal.GetAttableTarget(attackerCal.Attacker);
				if(neighbors.Count>0){
					foreach(Transform sixGon in neighbors){
						if(sixGon.GetComponent<Identy>().Flag){
							sixGon.renderer.material = closeByFlag;
						}else{
							sixGon.renderer.material = closeBy;
							networkView.RPC("RPCUpdateMat",RPCMode.Others,sixGon.name,2);
						}
					}
					currentPos.renderer.material = rollOver;
					networkView.RPC("RPCUpdateMat",RPCMode.Others,currentPos.name,3);
					selectMode = true;
					attackMode = true;
				}
			}
		}
	}
	
	public void defenseCommand(Transform chess){
		if(chess!=null){
			Transform locationMap = chess.GetComponent<CharacterSelect>().getMapPosition();
			CharacterProperty chessProperty = chess.GetComponent<CharacterProperty>();
			int playerSide = chessProperty.Player;
			int theOtherSide = 1;
			if(playerSide==1)
				theOtherSide =2;
			else if(playerSide==2)
				theOtherSide =1;
			
			RoundCounter players = Camera.mainCamera.GetComponent<RoundCounter>();
			
			if(chessProperty.Summoner){
				foreach(Transform territory in locationMap.GetComponent<Identy>().neighbor){
					if(!players.IsInsideTerritory(territory,playerSide)&& territory!=null){
						players.AddTerritory(territory,playerSide);
						if(players.IsInsideTerritory(territory,theOtherSide)){
							players.RemoveTerritory(territory,theOtherSide);
						}
					}
				}
			}
			if(!players.IsInsideTerritory(locationMap,playerSide)&& locationMap!=null){
				players.AddTerritory(locationMap,playerSide);
				if(players.IsInsideTerritory(locationMap,theOtherSide)){
					players.RemoveTerritory(locationMap,theOtherSide);
				}
			}
		}
	}
	
	public void summonCommand(Transform chess, Transform gf){
		if(chess!=null){
			CharacterSelect character = chess.GetComponent<CharacterSelect>();
			Transform currentPos = character.getMapPosition();
			if(currentPos!=null){
				//neighbors = chess.GetComponent<CharacterProperty>().GetSummonPosition();
				foreach(Transform map in players.GetTerritory(chess.GetComponent<CharacterProperty>().Player)){
					if(!MapHelper.IsMapOccupied(map))
						neighbors.Add(map);
				}
				currentGF = gf;
				if(neighbors.Count>0){
					foreach(Transform sixGon in neighbors){
						if(sixGon.GetComponent<Identy>().Flag){
							sixGon.renderer.material = closeByFlag;
						}else{
							sixGon.renderer.material = closeBy;
							networkView.RPC("RPCUpdateMat",RPCMode.Others,sixGon.name,2);
						}
					}
					currentPos.renderer.material = rollOver;
					networkView.RPC("RPCUpdateMat",RPCMode.Others,currentPos.name,3);
					selectMode = true;
					summonMode = true;
				}
			}
		}
	}
	
	public void skillCommand(Transform skill){
		if(chess!=null){
			CharacterSelect character = chess.GetComponent<CharacterSelect>();
			Transform currentPos = character.getMapPosition();
			if(currentPos!=null){
				CommonSkill cSkill = skill.GetComponent(skill.GetComponent<SkillProperty>().ScriptName) as CommonSkill;
				neighbors = cSkill.GetSelectionRange();
				if(neighbors.Count>0){
					foreach(Transform sixGon in neighbors){
						if(sixGon.GetComponent<Identy>().Flag){
							sixGon.renderer.material = closeByFlag;
						}else{
							sixGon.renderer.material = closeBy;
							networkView.RPC("RPCUpdateMat",RPCMode.Others,sixGon.name,2);
						}
					}
					currentPos.renderer.material = rollOver;
					networkView.RPC("RPCUpdateMat",RPCMode.Others,currentPos.name,3);
					CurrentSkill = skill;
					selectMode = true;
					skillMode = true;
				}
			}
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
	/*
	void OnGUI(){
		GUI.depth = 3;
		if(player==players.playerA)
			GUI.backgroundColor = Color.red;
		else
			GUI.backgroundColor = Color.yellow;
		if(guiShow){
			GUI.Box(new Rect(screenPos.x+guiSegX,screenPos.y+guiSegY,100,160), "Mana:"+player.GetComponent<ManaCounter>().Mana);
			if(summoner && player.GetComponent<ManaCounter>().Mana>=1){
				if(GUI.Button(new Rect(screenPos.x+guiSegX+guiSeg, screenPos.y+guiSegY+guiSeg*2,80,20),"Summon")){
					summonList = true;
					this.GetComponent<InfoUI>().infoUI = false;
				}
			}
			if(!chess.GetComponent<CharacterProperty>().Moved){
				if(GUI.Button(new Rect(screenPos.x+guiSegX+guiSeg, screenPos.y+guiSegY+guiSeg*4,80,20),"Move")){
					moveCommand(chess);
					
					guiShow = false;
					summonList = false;
					skillList = false;
					this.GetComponent<InfoUI>().infoUI = false;
					networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,2,true);
					networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,4,false);
					networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,3,false);
					networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,1,false);
				}
			}
			if(!chess.GetComponent<CharacterProperty>().Attacked){
				AttackCalculation atc = new AttackCalculation(chess);
				if(atc.GetAttableTarget(atc.Attacker).Count>0){
					if(GUI.Button(new Rect(screenPos.x+guiSegX+guiSeg, screenPos.y+guiSegY+guiSeg*6,80,20),"Attack")){
						attackCommand(chess);
						
						guiShow = false;
						summonList = false;
						skillList = false;
						this.GetComponent<InfoUI>().infoUI = false;
						networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,1,true);
						networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,4,false);
						networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,3,false);
						networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,2,false);
					}
				}
			}
			if(!chess.GetComponent<CharacterProperty>().Attacked){
				if(GUI.Button(new Rect(screenPos.x+guiSegX+guiSeg, screenPos.y+guiSegY+guiSeg*8,80,20),"Defense")){
					guiShow = false;
					summonList = false;
					skillList = false;
					
					CharacterProperty chessProperty = chess.GetComponent<CharacterProperty>();
					
					chessProperty.Attacked = true;
					chessProperty.Moved = true;
					
					//update network
					this.GetComponent<InfoUI>().infoUI = false;
					networkView.RPC("RPCCancelStatusUI",RPCMode.Others,chess.name);
					networkView.RPC("RPCUpdateChessAttacked", RPCMode.Others, chess.name, true);
					networkView.RPC("RPCUpdateChessMoved", RPCMode.Others, chess.name, true);
					
					Transform locationMap = chess.GetComponent<CharacterSelect>().getMapPosition();
					int playerSide = chessProperty.Player;
					int theOtherSide = 1;
					if(playerSide==1)
						theOtherSide =2;
					else if(playerSide==2)
						theOtherSide =1;
					
					if(chess.GetComponent<CharacterProperty>().Summoner){
						foreach(Transform territory in locationMap.GetComponent<Identy>().neighbor){
							if(!players.IsInsideTerritory(territory,playerSide)&& territory!=null){
								players.AddTerritory(territory,playerSide);
								if(players.IsInsideTerritory(territory,theOtherSide)){
									players.RemoveTerritory(territory,theOtherSide);
								}
							}
						}
					}
					if(!players.IsInsideTerritory(locationMap,playerSide)&& locationMap!=null){
						players.AddTerritory(locationMap,playerSide);
						if(players.IsInsideTerritory(locationMap,theOtherSide)){
							players.RemoveTerritory(locationMap,theOtherSide);
						}
					}
					//chess.gameObject.layer = 10;
					// network update
					//networkView.RPC("RPCUpdateChessEndTurn",RPCMode.Others,chess.name,chessProperty.TurnFinished);
					updateAllCharactersPowers();
					updateTerritoryMat();
					
					bool ifMolePassive = MapHelper.CheckPassive(PassiveType.DefenseAddOne,chess);
					
					if(ifMolePassive){
						chessProperty.Hp +=1;
						networkView.RPC("RPCMolePassive",RPCMode.Others,chess.name,ifMolePassive);
					}
					//networkView.RPC("RPCUpdateChessLayers",RPCMode.Others, chess.name,10);
				}
			}
			if(!chess.GetComponent<CharacterProperty>().Activated){
				if(GUI.Button(new Rect(screenPos.x+guiSegX+guiSeg, screenPos.y+guiSegY+guiSeg*10,80,20),"Skills")){
					skillList = true;
					summonList = false;
					this.GetComponent<InfoUI>().infoUI = false;
					//chess.GetComponent<CharacterProperty>().Activated = true;
				}
			}
			if(GUI.Button(new Rect(screenPos.x+guiSegX+guiSeg, screenPos.y+guiSegY+guiSeg*12,80,20),"Info")){
				Texture2D textureInfo = chess.GetComponent<CharacterProperty>().cardInfo;
				guiShow = false;
				skillList = false;
				summonList = false;
				this.GetComponent<InfoUI>().infoUI = true;
				this.GetComponent<InfoUI>().TextureInfo = textureInfo;
			}
			
			if(!chess.GetComponent<CharacterProperty>().TurnFinished){
				if(GUI.Button(new Rect(screenPos.x+guiSegX+guiSeg, screenPos.y+guiSegY+guiSeg*14,80,20),"End Turn")){
					
					guiShow = false;
					skillList = false;
					summonList = false;
					this.GetComponent<InfoUI>().infoUI = false;
					chess.GetComponent<CharacterProperty>().Activated = true;
					chess.GetComponent<CharacterProperty>().Attacked = true;
					chess.GetComponent<CharacterProperty>().Moved = true;
					chess.GetComponent<CharacterProperty>().TurnFinished = true;
					chess.gameObject.layer = 10;
					//update netwrok
					networkView.RPC("RPCCancelStatusUI",RPCMode.Others,chess.name);
					networkView.RPC("RPCUpdateChessEndTurn",RPCMode.Others,chess.name,true);
					networkView.RPC("RPCUpdateChessLayers",RPCMode.Others, chess.name,10);
				}
			}
			if(summonList){
				GUI.Box(new Rect(screenPos.x+guiSegX*4,screenPos.y+guiSegY,150,150),"GFs");
				IList soldiers = null;
				if(chess.GetComponent<CharacterProperty>().Player == 1){
					soldiers = Camera.mainCamera.GetComponent<RoundCounter>().PlayerAChesses;
				}else if(chess.GetComponent<CharacterProperty>().Player == 2){
					soldiers = Camera.mainCamera.GetComponent<RoundCounter>().PlayerBChesses;
				}
				int seg = 0;
				foreach(Transform gf in soldiers){
					if(gf.GetComponent<CharacterProperty>().death && !gf.GetComponent<CharacterProperty>().Summoner){
						seg+=1;
						if(GUI.Button(new Rect(screenPos.x+guiSegX*4+10, screenPos.y+guiSegY+20*seg,150,20),gf.GetComponent<CharacterProperty>().NameString + " ("+gf.GetComponent<CharacterProperty>().WaitRounds+") ")){
							if(gf.GetComponent<CharacterProperty>().Ready && player.GetComponent<ManaCounter>().Mana >= gf.GetComponent<CharacterProperty>().summonCost){
								summonCommand(chess,gf); 
								//gf.GetComponent<CharacterProperty>().death = false;
								guiShow = false;
								networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,4,true);
								networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,1,false);
								networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,2,false);
								networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,3,false);
								//chess.GetComponent<CharacterProperty>().Activated = true;
							}
							summonList = false;
						}	
					}
				}
			}
			if(skillList){
				GUI.Box(new Rect(screenPos.x+guiSegX*4,screenPos.y+guiSegY,150,150),"Skills");
				Transform[] skills = chess.GetComponent<SkillSets>().Skills;
				int seg =0;
				foreach(Transform skill in skills){
					if(player.GetComponent<ManaCounter>().Mana>=skill.GetComponent<SkillProperty>().SkillCost){
						seg+=1;
						if(GUI.Button(new Rect(screenPos.x+guiSegX*4+10, screenPos.y+guiSegY+20*seg,150,20),skill.GetComponent<SkillProperty>().SkillName + " ("+skill.GetComponent<SkillProperty>().SkillCost+" M) ")){
							networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,3,true);
							networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,1,false);
							networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,2,false);
							networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,4,false);
							if(!skill.GetComponent<SkillProperty>().NeedToSelect){
								skill.GetComponent<SkillProperty>().GetRealSkillRate();
								skill.GetComponent<SkillProperty>().PassSkillRate = MapHelper.Success(skill.GetComponent<SkillProperty>().SkillRate);
								skill.GetComponent<SkillProperty>().ActivateSkill();
								player.GetComponent<ManaCounter>().Mana -= skill.GetComponent<SkillProperty>().SkillCost;
								chess.GetComponent<CharacterProperty>().Activated = true;
								//chess.gameObject.layer = 10;
								//update network
								networkView.RPC("RPCUpdateChessSkillCmdA",RPCMode.Others,chess.name,skill.name,skill.GetComponent<SkillProperty>().PassSkillRate);
								networkView.RPC("RPCUpdateChessActivated",RPCMode.Others, chess.name,true);
								networkView.RPC("RPCUpdateMana",RPCMode.Others,player.GetComponent<ManaCounter>().Mana,player.GetComponent<CharacterProperty>().Player);
								
							}else{
								skillCommand(skill);
							}
							skillList = false;
							guiShow = false;
						}
					}
				}
			}
		}
	}
	*/
	/*
	void RPCUpdateProperty(Transform chess){
		if(chess!=null){
			CharacterProperty cp = chess.GetComponent<CharacterProperty>();
			networkView.RPC("RPCUpdateChessUnnormal",RPCMode.Others,chess.name,0,cp.UnStatusCounter[UnnormalStatus.Burned]);
			networkView.RPC("RPCUpdateChessLastUnnormal",RPCMode.Others,chess.name,0,cp.LastUnStatusCounter[UnnormalStatus.Burned]);
			networkView.RPC("RPCUpdateChessUnnormal",RPCMode.Others,chess.name,1,cp.UnStatusCounter[UnnormalStatus.Chaos]);
			networkView.RPC("RPCUpdateChessLastUnnormal",RPCMode.Others,chess.name,1,cp.LastUnStatusCounter[UnnormalStatus.Chaos]);
			networkView.RPC("RPCUpdateChessUnnormal",RPCMode.Others,chess.name,2,cp.UnStatusCounter[UnnormalStatus.Freezed]);
			networkView.RPC("RPCUpdateChessLastUnnormal",RPCMode.Others,chess.name,2,cp.LastUnStatusCounter[UnnormalStatus.Freezed]);
			networkView.RPC("RPCUpdateChessUnnormal",RPCMode.Others,chess.name,3,cp.UnStatusCounter[UnnormalStatus.Poisoned]);
			networkView.RPC("RPCUpdateChessLastUnnormal",RPCMode.Others,chess.name,3,cp.LastUnStatusCounter[UnnormalStatus.Poisoned]);
			networkView.RPC("RPCUpdateChessUnnormal",RPCMode.Others,chess.name,4,cp.UnStatusCounter[UnnormalStatus.Sleeping]);
			networkView.RPC("RPCUpdateChessLastUnnormal",RPCMode.Others,chess.name,4,cp.LastUnStatusCounter[UnnormalStatus.Sleeping]);
			networkView.RPC("RPCUpdateChessUnnormal",RPCMode.Others,chess.name,5,cp.UnStatusCounter[UnnormalStatus.Wounded]);
			networkView.RPC("RPCUpdateChessLastUnnormal",RPCMode.Others,chess.name,5,cp.LastUnStatusCounter[UnnormalStatus.Wounded]);
			
			networkView.RPC("RPCUpdateChessStatus",RPCMode.Others,chess.name,0,cp.BuffMoveRange);
			networkView.RPC("RPCUpdateChessStatus",RPCMode.Others,chess.name,1,cp.BuffAtkRange);
			networkView.RPC("RPCUpdateChessStatus",RPCMode.Others,chess.name,2,cp.Damage);
			networkView.RPC("RPCUpdateChessStatus",RPCMode.Others,chess.name,3,cp.Hp);
			networkView.RPC("RPCUpdateChessStatus",RPCMode.Others,chess.name,4,cp.BuffCriticalHit);
			networkView.RPC("RPCUpdateChessStatus",RPCMode.Others,chess.name,5,cp.BuffSkillRate);
			networkView.RPC("RPCUpdateChessStatus",RPCMode.Others,chess.name,6,cp.StandByRounds);
		}
	}
	*/
    void Update() {
		panning();	
		selecting();
		if(camMoveMode)
			translateMainCam(80);
		if (Input.GetKeyDown(KeyCode.Return)) {  
    		Application.LoadLevel(1);  
  		}  
    }
	
	public void MoveCommandNetwork(){
		networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,2,true);
		networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,4,false);
		networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,3,false);
		networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,1,false);
	}
	
	public void AttackCommandNetwork(){
		networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,1,true);
		networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,4,false);
		networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,3,false);
		networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,2,false);
	}
	
	public void DefenseNetwork(){
		CharacterProperty chessProperty = chess.GetComponent<CharacterProperty>();
		networkView.RPC("RPCCancelStatusUI",RPCMode.Others,chess.name);
		networkView.RPC("RPCUpdateChessAttacked", RPCMode.Others, chess.name, true);
		networkView.RPC("RPCUpdateChessMoved", RPCMode.Others, chess.name, true);
		
		bool ifMolePassive = MapHelper.CheckPassive(PassiveType.DefenseAddOne,chess);			
		if(ifMolePassive){
			chessProperty.Hp +=1;
			networkView.RPC("RPCMolePassive",RPCMode.Others,chess.name,ifMolePassive);
		}
	}
	
	public void EndTurnNetwork(){
		networkView.RPC("RPCCancelStatusUI",RPCMode.Others,chess.name);
		networkView.RPC("RPCUpdateChessEndTurn",RPCMode.Others,chess.name,true);
		networkView.RPC("RPCUpdateChessLayers",RPCMode.Others, chess.name,10);
		networkView.RPC("RPCUpdateTargetInfoUI",RPCMode.Others,chess.name,false);
	}
	
	public void SummonNetwork(){
		networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,4,true);
		networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,1,false);
		networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,2,false);
		networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,3,false);
	}
	
	public void SkillUINetwork(){
		networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,3,true);
		networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,1,false);
		networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,2,false);
		networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,4,false);
	}
	
	public void SkillCmdNetwork(Transform chess, Transform skill){
		networkView.RPC("RPCUpdateChessSkillCmdA",RPCMode.Others,chess.name,skill.name,skill.GetComponent<SkillProperty>().PassSkillRate);
		networkView.RPC("RPCUpdateChessActivated",RPCMode.Others, chess.name,true);
		networkView.RPC("RPCUpdateMana",RPCMode.Others,player.GetComponent<ManaCounter>().Mana,player.GetComponent<CharacterProperty>().Player);				
	}
	
	[RPC]
	void RPCUpdateChessPosition(string selName, string chessName){
		Transform selection = GameObject.Find(selName).transform;
		Transform chess = GameObject.Find(chessName).transform;
		chess.position = selection.position;
		chess.Translate(new Vector3(0.0f,1.5f,0.0f));
	}
	
	[RPC]
	void RPCUpdateTerritory(string tANames, int side){
		if(tANames!=null){
			Transform map = GameObject.Find(tANames).transform;
			//if(!players.IsInsideTerritory(map,side))
			//	players.AddTerritory(map,side);
			if(side==1)
				map.renderer.material = players.TerritoryA;
			else if(side==2)
				map.renderer.material = players.TerritoryB;
		}
		
	}
	[RPC]
	void RPCMolePassive(string chessName, bool passive){
		if(passive){
			Transform chess = GameObject.Find(chessName).transform;
			chess.GetComponent<CharacterProperty>().Hp+=1;
		}
	}
	
	
	[RPC]
	void RPCUpdateChessEndTurn(string chessName, bool endTurn){
		if(endTurn){
			Transform chess = GameObject.Find(chessName).transform;
			chess.GetComponent<CharacterProperty>().Moved = true;
			chess.GetComponent<CharacterProperty>().Attacked = true;
			chess.GetComponent<CharacterProperty>().Activated = true;
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
		chessProperty.Hp = chessProperty.defPower;
		chessProperty.death = false;
		chessProperty.Attacked = true;
		chessProperty.Activated = true;
		chessProperty.Moved = true;
	}
	[RPC]
	void RPCUpdateMana(int mana, int side){
		if(side==1)
			players.playerA.GetComponent<ManaCounter>().Mana = mana;
		else if(side == 2)
			players.playerB.GetComponent<ManaCounter>().Mana = mana;
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
		AttackCalculation attackerCal = new AttackCalculation(chess);
		attackerCal.InsertTarget(selection);
		attackerCal.CriticalHit = state;
		attackerCal.UpdateAttackResult(AttackType.physical);
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
		}
	}
	
	[RPC]
	void RPCUpdateMat(string selName, int mat){
		Transform selection = GameObject.Find(selName).transform;
		if(mat==1){
			selection.renderer.material = originalMat;
		}else if(mat==2){
			selection.renderer.material = closeBy;
		}else if(mat==3){
			selection.renderer.material = rollOver;
		}
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
}
