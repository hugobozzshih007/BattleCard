using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MapUtility;
using BuffUtility;

public class selection : MonoBehaviour {
	bool npcMode;
	public Transform sel;
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
	public bool NpcPlaying = false;
	public Material rollOver;
	public Material closeBy;
	public Material rollOverFlag;
	public Material closeByFlag;
	public Material FlagMat;
	public float castLength = 80.0f;
	public bool MoveCam;
	public int steps = 2;
	private bool viewScreenPanning = false;
	private float viewScreenOldMouseX;
	private float viewScreenOldMouseY;
	private float viewScreenZoomHeight = 50.0f;
	private float viewScreenPanSpeed = 2000.0f;
	private const float viewOffsetZ = 30.0f;
	private Vector3 oldCamPosition;
	private Vector3 newCamPosition;
	public Transform currentGF;
	//bool summonList = false;
	//bool skillList = false;
	//bool summoner = false;
	bool camMoveMode = false;
	//bool infoUI = false;
	public bool SummonIn = false;
	//int guiSegX = 30;
	//int guiSegY = 20;
	//int guiSeg = 10;
	int camStep = 0;
	float t = 0;
	float fadeInTime = 2.0f;
	Vector3 screenPos;
	public Transform player, CurrentSkill;
	MainUI mainUI;
	bool tower;
	StatusUI statusUI;
	RoundCounter players;
	MainInfoUI chessUI;
	CommonFX fxVault; 
	AttackCalFX attackerCal;
	Color red = new Color(1.0f,155.0f/255.0f,155.0f/255.0f,1.0f);
	Color yellow = new Color(1.0f,245.0f/255.0f,90.0f/255.0f,1.0f);
	Color rollOverBlue = new Color(0.3f,0.3f,0.3f,1.0f);
	Color rollOverRed = new Color(0.7f,0.7f,0.7f,1.0f);
	public bool reviveMode = false;
	NpcPlayer npc;
	
	
	void Start(){
		viewScreenZoomHeight = transform.position.y;
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
	}
	
	void Awake(){
		Application.targetFrameRate = 1000;
		if(Network.connections.Length>0){
			npcMode = false;
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
		}else{
			npcMode = true;
			APlaying = true;
			BPlaying = false;
			Playing = APlaying;
			NpcPlaying = false;
		}
	}
	void CleanMapsMat(){
		Transform allMap = GameObject.Find("Maps").transform;
		for(int i =0; i< allMap.GetChildCount(); i++){
			allMap.GetChild(i).renderer.material = originalMat;
			if(!npcMode)
				networkView.RPC("RPCUpdateMat",RPCMode.OthersBuffered,allMap.GetChild(i).name,1);
		}
	}
		
	void panning(){
		if(mainUI.Cancel){
			selectMode = false;
			moveMode = false;
			attackMode = false;
			summonMode = false;
			skillMode = false;
			
			CleanMapsMat();
			updateTerritoryMat();
		}
		
		if(Input.GetMouseButtonDown(1)) {
			//guiShow = false;
			mainUI.MainGuiFade = false;
			mainUI.SubGuiFade = false;
			chessUI.MainFadeIn = false;
			chessUI.TargetFadeIn = false;
			
			moveMode = false;
			attackMode = false;
			
			//summonList = false;
			//skillList = false;
			skillMode = false;
			if(!reviveMode){
				selectMode = false;
				summonMode = false;
			}
			
			if(chess != null){
				chess.gameObject.layer = 10;
				MoveToLayer(chess,10);
				if(Playing && !npcMode){
					networkView.RPC("RPCCancelStatusUI",RPCMode.Others,chess.name);
					networkView.RPC("RPCUpdateChessLayers",RPCMode.Others,chess.name,10);
					networkView.RPC("RPCUpdateTargetInfoUI",RPCMode.Others,chess.name,false);
				}
			}
    		viewScreenPanning = true;
    		viewScreenOldMouseX = Input.mousePosition.x;
    		viewScreenOldMouseY = Input.mousePosition.y;
			if(!reviveMode){
				CleanMapsMat();
			}
			updateTerritoryMat();
			//neighbors.Clear();
		}
		if(viewScreenPanning==true) {
		    if(Input.GetMouseButtonUp(1)) {
		        viewScreenPanning = false;
		    }
			if(MoveCam){
			    Vector3 viewScreenPanVector = transform.TransformPoint(new Vector3((viewScreenOldMouseX - Input.mousePosition.x) / viewScreenPanSpeed, 0.0f, (viewScreenOldMouseY - Input.mousePosition.y) / viewScreenPanSpeed));
			    // since we use a quick and dirty transform, reset the camera height to what it was
			    viewScreenPanVector.y = viewScreenZoomHeight; 
			    transform.position = viewScreenPanVector;
			}
		}
		float zoomRate = 100.0f;
		float distance = Camera.main.fov;
		if(Input.GetAxis("Mouse ScrollWheel") != 0.0){
    		distance -= (Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime) * zoomRate * Mathf.Abs(distance);
			foreach(Camera t in Camera.allCameras){
				t.fov = distance;
			}
		}
	}
	
	void alignCamera(Transform focus){
		Vector3 newCamPosition = focus.position - players.CamOffest;
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
	
	public void SummonTrueCmd(Transform currentChess,Transform gf, Transform map){
		gf.position = map.position;
		gf.Translate(new Vector3(0.0f,1.5f,0.0f));
		Transform targetLook = transform.GetComponent<MoveCharacter>().GetClosetChess(gf);
		Vector3 relativePos = targetLook.transform.position - gf.transform.position;
		Quaternion rotation = Quaternion.LookRotation(relativePos);
		gf.transform.rotation = Quaternion.Euler(new Vector3(gf.transform.rotation.eulerAngles.x, rotation.eulerAngles.y, gf.transform.rotation.eulerAngles.z));
									
		Vector3 fxPos = new Vector3(map.transform.position.x,map.transform.position.y+0.1f,map.transform.position.z);
		Transform flashIn = Instantiate(fxVault.SummonIn,fxPos,Quaternion.identity) as Transform;
		GameObject insFlash = GameObject.Find(flashIn.name).gameObject;
		Destroy(insFlash, 3.0f);
		if(currentChess.GetComponent<CharacterProperty>().Player==1)
			MapHelper.SetObjTransparent(gf,red,0.0f);
		else
			MapHelper.SetObjTransparent(gf,yellow,0.0f);
		SummonIn = true;
		t = 0;
									
		gf.renderer.enabled = true;
		//currentGF.GetComponent<CharacterProperty>().Hp = currentGF.GetComponent<CharacterProperty>().defPower;
		gf.GetComponent<CharacterProperty>().death = false;
		gf.GetComponent<CharacterProperty>().Attacked = true;
		gf.GetComponent<CharacterProperty>().Activated = true;
		gf.GetComponent<CharacterProperty>().Moved = true;
		gf.GetComponent<CharacterProperty>().TurnFinished = true;
		player.GetComponent<ManaCounter>().Mana-=gf.GetComponent<CharacterProperty>().summonCost;
		
		updateAllCharactersPowers();
		updateTerritoryMat();
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
						if(Playing && !npcMode){
							print(npcMode);
							networkView.RPC("RPCUpdateChessLayers",RPCMode.Others,chess.name,11);
							networkView.RPC("RPCUpdateTargetInfoUI", RPCMode.Others,chess.name,true);
						}
						
						if(chess!=oldChess && oldChess!=null){
							oldChess.gameObject.layer = 10;
							MoveToLayer(oldChess,10);
							if(Playing && !npcMode)
								networkView.RPC("RPCUpdateChessLayers",RPCMode.Others,oldChess.name,10);
						}
						oldChess = chess;
						
						newCamPosition = chess.position - players.CamOffest;
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
						if(chessProperty.TurnFinished || !Playing){
							//guiShow = false;
							mainUI.MainGuiFade = false;
							mainUI.IgnoreMainUI = true;
							mainUI.SubGuiFade = false;
							chessUI.InsertChess(chess);
							chessUI.MainFadeIn = true;
							chessUI.TargetFadeIn = true;
							//this.GetComponent<InfoUI>().infoUI = true;
						}else if(Playing){
							//guiShow = true;
							if(!chessProperty.TurnFinished){
								mainUI.MainGuiFade = true;
								if(mainUI.InTutorial){
									GameObject.Find("InitStage").transform.GetComponent<InitStage>().ShowSelCmd = false;
									GameObject.Find("InitStage").transform.GetComponent<InitStage>().ShowMoveCmd = true;
								}
							}else{
								mainUI.MainGuiFade = false;
							}
						}
						//summoner = chess.GetComponent<CharacterProperty>().Summoner;
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
							sel.renderer.material = rollOver;
							if(MapHelper.IsMapOccupied(sel)){
								Transform targetChess = MapHelper.GetMapOccupiedObj(sel);
								mainUI.IgnoreMainUI = true;
								chessUI.InsertChess(targetChess);
								chessUI.TargetFadeIn = true;
								if(!npcMode)
									networkView.RPC("RPCUpdateMainInfoUI",RPCMode.Others,targetChess.name,true);
							}else{
								mainUI.IgnoreMainUI = false;
								chessUI.TargetFadeIn = false;
								if(!npcMode)
									networkView.RPC("RPCUpdateMainInfoUI",RPCMode.Others,sel.name,false);
							}
							if(!npcMode)
								networkView.RPC("RPCUpdateMat",RPCMode.Others,sel.name,2);
							
							
							if(selOld!=sel && neighbors.Contains(selOld)){
								selOld.renderer.material = closeBy;
								mainUI.IgnoreMainUI = false;
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
									localUnit.renderer.material = originalMat;
									
									//animate character moving from point to point, start point: locarUnit, end point: sel
									IList pathList = new List<Transform>();
									pathList = chessSelect.FindPathList(localUnit,GetSteps(localUnit,sel),sel);
									MoveCharacter mc = transform.GetComponent<MoveCharacter>();
									mc.SetSteps(chess,pathList);
									
									if(!npcMode){
										networkView.RPC("RPCUpdateMat",RPCMode.Others,localUnit.name,1);
										networkView.RPC("UpdateMoveCharacter",RPCMode.Others,chess.name, sel.name);
										networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,2,false);
									}
									
									foreach(Transform sixGons in neighbors){
										sixGons.renderer.material = originalMat;
										if(!npcMode)
											networkView.RPC("RPCUpdateMat",RPCMode.Others,sixGons.name,1);
									}
									
									//chessProperty.Moved = true;
									chessSelect.MoveRangeList.Clear();
									
									updateTerritoryMat();
									
									moveMode = false;
									
									//update UI
									mainUI.SubGuiFade = false;
									if(mainUI.InTutorial){
										GameObject.Find("InitStage").transform.GetComponent<InitStage>().ShowMap = false;
									}
									if(chessProperty.Attacked && chessProperty.Activated && chessProperty.Moved){
										chessProperty.TurnFinished = true;
										chess.gameObject.layer = 10;
										MoveToLayer(chess,10);
										EndTurnNetwork();
									}
								}
								if(summonMode){
									if(!reviveMode){
										chess.GetComponent<CharacterSelect>().getMapPosition().renderer.material = originalMat;
										if(!npcMode)
											networkView.RPC("RPCUpdateMat", RPCMode.Others,chess.GetComponent<CharacterSelect>().getMapPosition().name,1);
									}
									
									SummonTrueCmd(chess, currentGF, sel);
									
									
									foreach(Transform sixGons in neighbors){
										sixGons.renderer.material =originalMat;
										if(!npcMode)
											networkView.RPC("RPCUpdateMat",RPCMode.Others,sixGons.name,1);
									}
									
									Transform targetLook = transform.GetComponent<MoveCharacter>().GetClosetChess(currentGF);
									
									//update network
									if(!npcMode){
										networkView.RPC("RPCSummonIn",RPCMode.Others,currentGF.name, sel.name);
										networkView.RPC("RPCShowUp",RPCMode.Others,currentGF.name);
										networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,4,false);
										networkView.RPC("RPCUpdateChessPosition",RPCMode.Others,sel.name,currentGF.name);
										networkView.RPC("RPCUpdateRenderStatus",RPCMode.Others,currentGF.name, currentGF.renderer.enabled);
										networkView.RPC("RPCUpdateChessSummon",RPCMode.Others,currentGF.name);
										networkView.RPC("RPCLookAtTarget",RPCMode.Others,currentGF.name,targetLook.name);
										networkView.RPC("RPCUpdateMana",RPCMode.Others,player.GetComponent<ManaCounter>().Mana,currentGF.GetComponent<CharacterProperty>().Player );
									}
									
									//chess.gameObject.layer = 10;
									//networkView.RPC("RPCUpdateChessLayers",RPCMode.Others, chess.name,10);
									
									// update UI
									mainUI.SubGuiFade = false;
									if(!reviveMode)
										mainUI.MainGuiFade = true;
									else{
										mainUI.MainGuiFade = false;
										selectMode = false;
									}
									summonMode = false;
									reviveMode = false;
									if(!npcMode)
										networkView.RPC("RPCUpdateRevive", RPCMode.Others,false);
								} 
								if(attackMode){
									// restore the original mat for the map
									chess.GetComponent<CharacterSelect>().getMapPosition().renderer.material = originalMat;
									
									foreach(Transform sixGons in neighbors){
										sixGons.renderer.material =originalMat;
										if(!npcMode)
											networkView.RPC("RPCUpdateMat",RPCMode.Others,sixGons.name,1);
									}
									chess.GetComponent<CharacterSelect>().AttackRangeList.Clear();
									
									// activate turning head 
									bool critiq = attackerCal.CalcriticalHit(chess,AttackType.physical);
									//print("critiq: "+critiq);
									TurnHead th = transform.GetComponent<TurnHead>();  
									th.SetTurnHeadSequence(chess,sel,true,false,critiq);
									
									if(!npcMode){
										networkView.RPC("RPCUpdateMat",RPCMode.Others,chess.GetComponent<CharacterSelect>().getMapPosition().name,1);
										networkView.RPC("UpdateTurningHead",RPCMode.Others,chess.name,sel.name,critiq);
										networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,1,false);
									}
									//chess.GetComponent<CharacterProperty>().Attacked = true;
									
									attackMode = false;
									updateTerritoryMat();
								
									//mainUI.SubGuiFade = false;
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
										
										player.GetComponent<ManaCounter>().Mana -= CurrentSkill.GetComponent<SkillProperty>().SkillCost;
										chess.GetComponent<CharacterProperty>().Activated = true;
										
										foreach(Transform sixGons in neighbors){
											sixGons.renderer.material = originalMat;
											if(!npcMode)
												networkView.RPC("RPCUpdateMat",RPCMode.Others,sixGons.name,1);
										}
										
										chessUI.DelayFadeOut = true;
										chessUI.TargetFadeIn = false;
										AnimStateNetWork(chess,AnimVault.AnimState.skill);
										// update network
										if(!npcMode){
											networkView.RPC("RPCUpdateStatusUI",RPCMode.Others,chess.name,3,false);
											networkView.RPC("RPCUpdateChessActivated",RPCMode.Others, chess.name,true);
											networkView.RPC("RPCUpdateChessSkillCmd", RPCMode.Others, chess.name,CurrentSkill.name,sel.name,CurrentSkill.GetComponent<SkillProperty>().PassSkillRate);
											networkView.RPC("RPCUpdateMana",RPCMode.Others,player.GetComponent<ManaCounter>().Mana,player.GetComponent<CharacterProperty>().Player);
										}
									
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
		
		foreach(Transform territory in players.PlayerATerritory){
			aTNames.Add(territory.name);
			if(!npcMode)
				networkView.RPC("RPCUpdateTerritory",RPCMode.Others,territory.name, 1);
			//territory.GetComponent<Identy>().originalMat = players.TerritoryA;
			territory.GetComponent<Identy>().ShowMap.renderer.material = players.TerritoryA;
		}
		foreach(Transform territory in players.PlayerBTerritory){
			bTNames.Add(territory.name);
			if(!npcMode)
				networkView.RPC("RPCUpdateTerritory",RPCMode.Others,territory.name, 2);
			//territory.GetComponent<Identy>().originalMat = players.TerritoryB;
			territory.GetComponent<Identy>().ShowMap.renderer.material = players.TerritoryB;
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
	
	public void moveCommand(Transform chess){
		neighbors.Clear();
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
						sixGon.renderer.material = closeBy;
						if(!npcMode)
							networkView.RPC("RPCUpdateMat",RPCMode.Others,sixGon.name,3);
					}
					currentPos.renderer.material = rollOver;
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
		neighbors.Clear();
		updateAllCharactersPowers();
		if(chess!=null){
			//updateMapSteps();
			AttackCalculation attackerCal = new AttackCalculation(chess);
			CharacterSelect character = chess.GetComponent<CharacterSelect>();
			Transform currentPos = character.getMapPosition();
			if(currentPos!=null){
				neighbors = attackerCal.GetAttableTarget(attackerCal.Attacker);
				if(neighbors.Count>0){
					foreach(Transform sixGon in neighbors){
						sixGon.renderer.material= closeBy;
						if(!npcMode)
							networkView.RPC("RPCUpdateMat",RPCMode.Others,sixGon.name,3);
					}
					currentPos.renderer.material= rollOver;
					if(!npcMode)
						networkView.RPC("RPCUpdateMat",RPCMode.Others,currentPos.name,2);
					selectMode = true;
					attackMode = true;
				}
			}
		}
	}
	
	public void defenseCommand(Transform chess){
		neighbors.Clear();
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
		updateAllCharactersPowers();
	}
	
	public void summonCommand(Transform chess, Transform gf){
		neighbors.Clear();
		updateAllCharactersPowers();
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
						sixGon.renderer.material= closeBy;
						if(!npcMode)
							networkView.RPC("RPCUpdateMat",RPCMode.Others,sixGon.name,3);
					}
					currentPos.renderer.material= rollOver;
					if(!npcMode)
						networkView.RPC("RPCUpdateMat",RPCMode.Others,currentPos.name,2);
					selectMode = true;
					summonMode = true;
				}
			}
		}
	}
	
	public void skillCommand(Transform skill){
		updateAllCharactersPowers();
		if(chess!=null){
			CharacterSelect character = chess.GetComponent<CharacterSelect>();
			Transform currentPos = character.getMapPosition();
			if(currentPos!=null){
				CommonSkill cSkill = skill.GetComponent(skill.GetComponent<SkillProperty>().ScriptName) as CommonSkill;
				neighbors = cSkill.GetSelectionRange();
				if(neighbors.Count>0){
					foreach(Transform sixGon in neighbors){
						sixGon.renderer.material= closeBy;
						if(!npcMode)
							networkView.RPC("RPCUpdateMat",RPCMode.Others,sixGon.name,3);
					}
					currentPos.renderer.material= rollOver;
					if(!npcMode)
						networkView.RPC("RPCUpdateMat",RPCMode.Others,currentPos.name,2);
					CurrentSkill = skill;
					selectMode = true;
					skillMode = true;
				}
			}
		}
	}
	
	public void ReviveCommand(Transform masterChess){
		if(Playing){
			reviveMode = true;
			neighbors.Clear();
			neighbors = GetAllEmptyMaps();
			if(neighbors.Count>0){
				foreach(Transform haxgon in neighbors){
					haxgon.renderer.material= closeBy;
					if(!npcMode)
						networkView.RPC("RPCUpdateMat",RPCMode.Others,haxgon.name,3);
				}
				selectMode = true;
				summonMode = true;
				currentGF = masterChess;
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
	
	
    void Update() {
		panning();	
		selecting();
		if(SummonIn){
			if(currentGF!=null){
				t+=Time.deltaTime/fadeInTime;
				float alpha = Mathf.Lerp(0.0f,1.0f,t);
				Color oldCol = currentGF.renderer.material.color;
				oldCol.a = alpha;
				currentGF.renderer.material.color = oldCol;
				List<Transform> models = new List<Transform>();
				Transform model = currentGF.FindChild("Models");
				if(model.childCount>0){
					for(int i=0; i<model.childCount; i++){
						if(model.GetChild(i).GetComponent<SkinnedMeshRenderer>()!=null){
							models.Add(model.GetChild(i));
						}
					}
				}
				if(models.Count>0){
					foreach(Transform m in models){
						m.renderer.material.color = oldCol;
					}
				}
					
				
				if(alpha>=0.95){
					if(MapHelper.SetObjOldShader(currentGF,1.0f)){
						SummonIn = false;
						npc.InPause = true;
					}
					if(mainUI.InSecondTutor){
						//start Second Tutorial; 
						GameObject.Find("InitStage").transform.GetComponent<InitStage>().ShowSecMove = true;
					}
				}
			}
		}
		
		if(camMoveMode && MoveCam)
			translateMainCam(80);
		if (Input.GetKeyDown(KeyCode.Return)) {  
    		Application.LoadLevel(1);  
  		}  
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
			networkView.RPC("RPCUpdateChessAttacked", RPCMode.Others, chess.name, true);
			networkView.RPC("RPCUpdateChessMoved", RPCMode.Others, chess.name, true);
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
			networkView.RPC("RPCUpdateMana",RPCMode.Others,player.GetComponent<ManaCounter>().Mana,player.GetComponent<CharacterProperty>().Player);
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
		chess.position = selection.position;
		chess.Translate(new Vector3(0.0f,1.5f,0.0f));
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
			
			if(side==1)
				map.GetComponent<Identy>().ShowMap.renderer.material = players.TerritoryA;
			else if(side==2)
				map.GetComponent<Identy>().ShowMap.renderer.material = players.TerritoryB;
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
			selection.renderer.material = rollOver;
		}else if(mat==3){
			selection.renderer.material = closeBy;
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
}
