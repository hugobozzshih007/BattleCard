using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MapUtility;

public class selection : MonoBehaviour {
	private Transform sel;
	private Transform chess = null;
	IList neighbors;
	Transform[] summonArea;
	private Transform selOld;
	private SixGonRays unit;
	public Material originalMat;
	private bool moveMode = false;
	private bool summonMode = false;
	private bool selectMode = false;
	private bool attackMode = false;
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
	private Transform attackTarget;
	bool guiShow = false;
	bool summonList = false;
	bool summoner = false;
	bool camMoveMode = false;
	int guiSegX = 30;
	int guiSegY = 20;
	int guiSeg = 10;
	int camStep = 0;
	Vector3 screenPos;
	Transform player;
	bool tower;
	RoundCounter players;
	
	void Start(){
		sel = GameObject.Find("unit_start_point_A").transform;
		selOld = sel;
		players = Camera.mainCamera.GetComponent<RoundCounter>();
		moveMode = false;
		summonMode = false;
		selectMode = false;
	}
	
	
	void panning(){
		if(Input.GetMouseButtonDown(1)) {
			guiShow = false;
			selectMode = false;
			moveMode = false;
			attackMode = false;
			summonMode = false;
			summonList = false;
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
		if(Input.GetMouseButtonDown(0) && !guiShow && !selectMode){
			Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        	Debug.DrawRay(ray.origin, ray.direction * castLength, Color.yellow);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, castLength)){
				if(!hit.transform.GetComponent<Identy>().MapUnit && !hit.transform.GetComponent<CharacterProperty>().Tower){
					chess = hit.transform;
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
					if(chessProperty.TurnFinished){
						guiShow = false;
					}else{
						guiShow = true;
					}
					summoner = chess.GetComponent<CharacterProperty>().Summoner;
				}
			}
		}
		
		if(selectMode){
			Ray ray = camera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, castLength)){
				if(hit.transform.GetComponent<Identy>().MapUnit){
					sel = hit.transform;
					if(neighbors.Contains(sel)){
						
						if(sel.GetComponent<Identy>().Flag)
							sel.renderer.material = rollOverFlag;
						else
							sel.renderer.material = rollOver;
						
						if(selOld!=sel){
							if(selOld.GetComponent<Identy>().Flag)
								selOld.renderer.material = closeByFlag;
							else
								selOld.renderer.material = closeBy;
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
										sixGons.renderer.material = originalMat;}
								}
								/*
								foreach(Transform s in chess.GetComponent<CharacterSelect>().MoveRangeList){
									s.GetComponent<Identy>().step = 0;
								}*/
								chess.GetComponent<CharacterProperty>().Moved = true;
								chess.GetComponent<CharacterSelect>().MoveRangeList.Clear();
								updateTerritoryMat();
								updateCharacterPowers(chess);
								updateMapSteps();
								moveMode = false;
							} 
							if(summonMode){
								chess.GetComponent<CharacterSelect>().getMapPosition().renderer.material = originalMat;
								currentGF.position = sel.position;
								currentGF.Translate(new Vector3(0.0f,1.5f,0.0f));
								currentGF.renderer.enabled = true;
								currentGF.GetComponent<CharacterProperty>().Hp = currentGF.GetComponent<CharacterProperty>().defPower;
								currentGF.GetComponent<CharacterProperty>().death = false;
								player.GetComponent<ManaCounter>().Mana-=2;
								foreach(Transform sixGons in neighbors){
									if(sixGons.GetComponent<Identy>().Flag){
										sixGons.renderer.material = FlagMat;
									}else{
										sixGons.renderer.material = originalMat;
									}
								}
								summonMode = false;
								updateCharacterPowers(currentGF);
								updateTerritoryMat();
							}
							if(attackMode){
								chess.GetComponent<CharacterSelect>().getMapPosition().renderer.material = originalMat;
								attack(chess,sel);
								foreach(Transform sixGons in neighbors){
									if(sixGons.GetComponent<Identy>().Flag){
										sixGons.renderer.material = FlagMat;
									}else{
										sixGons.renderer.material = originalMat;
									}
								}
								/*
								foreach(Transform s in chess.GetComponent<CharacterSelect>().AttackRangeList){
									s.GetComponent<Identy>().step = 0;
								}*/
								chess.GetComponent<CharacterSelect>().AttackRangeList.Clear();
								chess.GetComponent<CharacterProperty>().Attacked = true;
								attackMode = false;
								updateMapSteps();
								updateTerritoryMat();
							}
							neighbors.Clear();
							selectMode = false;
						}
					}
				}
			}
		}
	}
	
	void updateTerritoryMat(){
		foreach(Transform territory in players.GetTerritory(1)){
			territory.GetComponent<Identy>().originalMat = players.TerritoryA;
			territory.renderer.material = territory.GetComponent<Identy>().originalMat;
		}
		foreach(Transform territory in players.GetTerritory(2)){
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
		CharacterProperty property = character.GetComponent<CharacterProperty>();
		int addedAtk = players.GetIntensifiedPower(character,"atk");
		int addedDef = players.GetIntensifiedPower(character,"def");
		int oldDefPower = property.ModifiedDefPow;
		property.ModifiedDefPow = property.defPower+addedDef;
		property.Damage = property.atkPower+addedAtk;
		if(property.Hp == oldDefPower)
			property.Hp = property.ModifiedDefPow;
	}
	
	void updateAllCharactersPowers(){
		foreach(Transform character in players.PlayerAChesses){
			CharacterProperty property = character.GetComponent<CharacterProperty>();
			int addedAtk = players.GetIntensifiedPower(character,"atk");
			int addedDef = players.GetIntensifiedPower(character,"def");
			int oldDefPower = property.ModifiedDefPow;
			property.ModifiedDefPow = property.defPower+addedDef;
			property.Damage = property.atkPower+addedAtk;
			if(property.Hp == oldDefPower)
				property.Hp = property.ModifiedDefPow;
		}
		foreach(Transform character in players.PlayerBChesses){
			CharacterProperty property = character.GetComponent<CharacterProperty>();
			int addedAtk = players.GetIntensifiedPower(character,"atk");
			int addedDef = players.GetIntensifiedPower(character,"def");
			int oldDefPower = property.ModifiedDefPow;
			property.ModifiedDefPow = property.defPower+addedDef;
			property.Damage = property.atkPower+addedAtk;
			if(property.Hp == oldDefPower)
				property.Hp = property.ModifiedDefPow;
		}
	}
	
	void moveCommand(Transform chess){
		if(chess!=null){
			CharacterSelect character = chess.GetComponent<CharacterSelect>();
			Transform currentPos = character.getMapPosition();
			if(currentPos!=null){
				character.findMoveRange(currentPos,0,chess.GetComponent<CharacterProperty>().moveRange);
				neighbors = character.MoveRangeList;
				if(neighbors.Count>0){
					foreach(Transform sixGon in neighbors){
						if(sixGon.GetComponent<Identy>().Flag){
							sixGon.renderer.material = closeByFlag;
						}else{
							sixGon.renderer.material = closeBy;
						}
					}
					currentPos.renderer.material = rollOver;
					selectMode = true;
					moveMode =true;
				}else{
					chess.GetComponent<CharacterProperty>().Moved = true;
				}
			}
		}
	}
	
	void attackCommand(Transform chess){
		if(chess!=null){
			CharacterSelect character = chess.GetComponent<CharacterSelect>();
			Transform currentPos = character.getMapPosition();
			if(currentPos!=null){
				neighbors = chess.GetComponent<CharacterProperty>().GetAttackPosition();
				if(neighbors.Count>0){
					foreach(Transform sixGon in neighbors){
						if(sixGon.GetComponent<Identy>().Flag){
							sixGon.renderer.material = closeByFlag;
						}else{
							sixGon.renderer.material = closeBy;
						}
					}
					currentPos.renderer.material = rollOver;
					selectMode = true;
					attackMode = true;
				}
			}
		}
	}
	
	void attack(Transform attacker, Transform targetLocation){
		Transform target = MapHelper.GetMapOccupiedObj(targetLocation);
		Transform attackerLocation = attacker.GetComponent<CharacterSelect>().getMapPosition();
		if(attacker.GetComponent<CharacterProperty>().criticalHit()){
			target.GetComponent<CharacterProperty>().Hp -= attacker.GetComponent<CharacterProperty>().Damage*2;
			print("Critical Hit!");
		}else{
			target.GetComponent<CharacterProperty>().Hp -= attacker.GetComponent<CharacterProperty>().Damage;
		}
		IList targetAtkableMaps = target.GetComponent<CharacterProperty>().GetAttackPosition();
		if(targetAtkableMaps.Contains(attackerLocation))
			attacker.GetComponent<CharacterProperty>().Hp -= target.GetComponent<CharacterProperty>().Damage;
		
		if(attacker.GetComponent<CharacterProperty>().Hp<=0){
			attacker.GetComponent<CharacterProperty>().death = true;
			attacker.GetComponent<CharacterProperty>().Ready = false;
			attacker.GetComponent<CharacterProperty>().WaitRounds = attacker.GetComponent<CharacterProperty>().StandByRounds;
		}
		
		if(target.GetComponent<CharacterProperty>().Hp<=0){
			if(!target.GetComponent<CharacterProperty>().Tower){
				target.GetComponent<CharacterProperty>().death = true;
				target.GetComponent<CharacterProperty>().Ready = false;
				target.GetComponent<CharacterProperty>().WaitRounds = target.GetComponent<CharacterProperty>().StandByRounds;
			}else{
				target.GetComponent<CharacterProperty>().death = true;
			}
		}
	}
	
	void summonCommand(Transform chess, Transform gf){
		if(chess!=null){
			CharacterSelect character = chess.GetComponent<CharacterSelect>();
			Transform currentPos = character.getMapPosition();
			if(currentPos!=null){
				//neighbors = chess.GetComponent<CharacterProperty>().GetSummonPosition();
				foreach(Transform map in players.GetTerritory(chess.GetComponent<CharacterProperty>().Player)){
					neighbors.Add(map);
				}
				currentGF = gf;
				if(neighbors.Count>0){
					foreach(Transform sixGon in neighbors){
						if(sixGon.GetComponent<Identy>().Flag){
							sixGon.renderer.material = closeByFlag;
						}else{
							sixGon.renderer.material = closeBy;
						}
					}
					currentPos.renderer.material = rollOver;
					selectMode = true;
					summonMode = true;
				}
			}
		}
	}
	
	void OnGUI(){
		if(player==players.playerA)
			GUI.backgroundColor = Color.red;
		else
			GUI.backgroundColor = Color.yellow;
		if(guiShow){
			GUI.Box(new Rect(screenPos.x+guiSegX,screenPos.y+guiSegY,100,150), "Mana:"+player.GetComponent<ManaCounter>().Mana);
			if(summoner && player.GetComponent<ManaCounter>().Mana>1){
				if(GUI.Button(new Rect(screenPos.x+guiSegX+guiSeg, screenPos.y+guiSegY+guiSeg*2,80,20),"Summon")){
					summonList = true;
				}
			}
			if(!chess.GetComponent<CharacterProperty>().Moved){
				if(GUI.Button(new Rect(screenPos.x+guiSegX+guiSeg, screenPos.y+guiSegY+guiSeg*4,80,20),"Move")){
					moveCommand(chess);
					guiShow = false;
					summonList = false;
				}
			}
			if(!chess.GetComponent<CharacterProperty>().Attacked && chess.GetComponent<CharacterProperty>().GetAttackPosition().Count>0){
				if(GUI.Button(new Rect(screenPos.x+guiSegX+guiSeg, screenPos.y+guiSegY+guiSeg*6,80,20),"Attack")){
					attackCommand(chess);
					guiShow = false;
					summonList = false;
				}
			}
			if(!chess.GetComponent<CharacterProperty>().Attacked){
				if(GUI.Button(new Rect(screenPos.x+guiSegX+guiSeg, screenPos.y+guiSegY+guiSeg*8,80,20),"Defence")){
					guiShow = false;
					summonList = false;
					//chess.GetComponent<CharacterProperty>().Hp+=1;
					chess.GetComponent<CharacterProperty>().Attacked = true;
					chess.GetComponent<CharacterProperty>().Moved = true;
					Transform locationMap = chess.GetComponent<CharacterSelect>().getMapPosition();
					int playerSide = chess.GetComponent<CharacterProperty>().Player;
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
					updateAllCharactersPowers();
					updateTerritoryMat();
				}
			}
			if(!chess.GetComponent<CharacterProperty>().Activated){
				if(GUI.Button(new Rect(screenPos.x+guiSegX+guiSeg, screenPos.y+guiSegY+guiSeg*10,80,20),"Skills")){
					guiShow = false;
					summonList = false;
					chess.GetComponent<CharacterProperty>().Activated = true;
				}
			}
			if(!chess.GetComponent<CharacterProperty>().TurnFinished){
				if(GUI.Button(new Rect(screenPos.x+guiSegX+guiSeg, screenPos.y+guiSegY+guiSeg*12,80,20),"End Turn")){
					guiShow = false;
					summonList = false;
					chess.GetComponent<CharacterProperty>().Activated = true;
					chess.GetComponent<CharacterProperty>().Attacked = true;
					chess.GetComponent<CharacterProperty>().Moved = true;
					chess.GetComponent<CharacterProperty>().TurnFinished = true;
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
							if(gf.GetComponent<CharacterProperty>().Ready){
								summonCommand(chess,gf); 
								//gf.GetComponent<CharacterProperty>().death = false;
								guiShow = false;
								//chess.GetComponent<CharacterProperty>().Activated = true;
							}
							summonList = false;
						}	
					}
				}
			}
		}//else if(!chess.GetComponent<CharacterProperty>().Ready){
			//waiting command here 
		//}
	}
	
    void Update() {
		panning();
		selecting();
		if(camMoveMode)
			translateMainCam(80);
		if (Input.GetKeyDown(KeyCode.Return)) {  
    		Application.LoadLevel(1);  
  		}  
    }
	
}
