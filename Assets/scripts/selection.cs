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
	private float viewScreenPanSpeed = 200.0f;
	private Transform currentGF;
	private Transform attackTarget;
	bool guiShow = false;
	bool summonList = false;
	bool summoner = false;
	int guiSegX = 30;
	int guiSegY = 20;
	int guiSeg = 10;
	Vector3 screenPos;
	
	void Start(){
		sel = GameObject.Find("unit").transform;
		selOld = sel;
		moveMode = false;
		summonMode = false;
		selectMode = false;
	}
	
	
	void panning(){
		if(Input.GetMouseButtonDown(1)) {
  			//print ("Panning...");
    		viewScreenPanning = true;
    		viewScreenOldMouseX = Input.mousePosition.x;
    		viewScreenOldMouseY = Input.mousePosition.y;
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
	
	//select map or character
	void selecting(){
		if(Input.GetMouseButtonDown(0) && !guiShow && !selectMode){
			Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        	Debug.DrawRay(ray.origin, ray.direction * castLength, Color.yellow);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, castLength)){
				if(!hit.transform.GetComponent<Identy>().MapUnit){
					screenPos = Camera.main.WorldToScreenPoint(hit.transform.position);
					screenPos.y = Screen.height - screenPos.y;
					chess = hit.transform;
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
								chess.GetComponent<CharacterSelect>().getMapPosition().renderer.material = originalMat;
								chess.position = sel.position;
								chess.Translate(new Vector3(0.0f,1.5f,0.0f));
								foreach(Transform sixGons in neighbors){
									if(sixGons.GetComponent<Identy>().Flag){
										sixGons.renderer.material = FlagMat;
									}else{
										sixGons.renderer.material = originalMat;}
								}
								foreach(Transform s in chess.GetComponent<CharacterSelect>().MoveRangeList){
									s.GetComponent<Identy>().step = 0;
								}
								chess.GetComponent<CharacterProperty>().Moved = true;
								chess.GetComponent<CharacterSelect>().MoveRangeList.Clear();
								moveMode = false;
							} 
							if(summonMode){
								chess.GetComponent<CharacterSelect>().getMapPosition().renderer.material = originalMat;
								currentGF.position = sel.position;
								currentGF.Translate(new Vector3(0.0f,1.5f,0.0f));
								currentGF.renderer.enabled = true;
								currentGF.GetComponent<CharacterProperty>().Hp = currentGF.GetComponent<CharacterProperty>().defPower;
								currentGF.GetComponent<CharacterProperty>().death = false;
								foreach(Transform sixGons in neighbors){
									if(sixGons.GetComponent<Identy>().Flag){
										sixGons.renderer.material = FlagMat;
									}else{
										sixGons.renderer.material = originalMat;
									}
								}
								summonMode = false;
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
								attackMode = false;
							}
							neighbors.Clear();
							selectMode = false;
						}
					}
				}
			}
		}
	}
	
	void moveCommand(Transform chess){
		if(chess!=null){
			CharacterSelect character = chess.GetComponent<CharacterSelect>();
			Transform currentPos = character.getMapPosition();
			//Transform mainCh = MapHelper.GetMapOccupiedObj(currentPos);
			//print(mainCh.name);
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
				print(neighbors.Count);
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
		target.GetComponent<CharacterProperty>().Hp -= attacker.GetComponent<CharacterProperty>().Damage;
	}
	
	void summonCommand(Transform chess, Transform gf){
		if(chess!=null){
			CharacterSelect character = chess.GetComponent<CharacterSelect>();
			Transform currentPos = character.getMapPosition();
			if(currentPos!=null){
				neighbors = chess.GetComponent<CharacterProperty>().GetSummonPosition();
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
		if(guiShow){
			
			GUI.Box(new Rect(screenPos.x+guiSegX,screenPos.y+guiSegY,100,130), "Menu");
			if(summoner && !chess.GetComponent<CharacterProperty>().Activated){
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
			if(!chess.GetComponent<CharacterProperty>().Attacked){
				if(GUI.Button(new Rect(screenPos.x+guiSegX+guiSeg, screenPos.y+guiSegY+guiSeg*6,80,20),"Attack")){
					attackCommand(chess);
					guiShow = false;
					summonList = false;
					chess.GetComponent<CharacterProperty>().Attacked = true;
				}
			}
			if(!chess.GetComponent<CharacterProperty>().Attacked){
				if(GUI.Button(new Rect(screenPos.x+guiSegX+guiSeg, screenPos.y+guiSegY+guiSeg*8,80,20),"Defence")){
					guiShow = false;
					summonList = false;
					chess.GetComponent<CharacterProperty>().Hp+=1;
					chess.GetComponent<CharacterProperty>().Attacked = true;
					chess.GetComponent<CharacterProperty>().Moved = true;
				}
			}
			if(!chess.GetComponent<CharacterProperty>().Activated){
				if(GUI.Button(new Rect(screenPos.x+guiSegX+guiSeg, screenPos.y+guiSegY+guiSeg*10,80,20),"Skills")){
					guiShow = false;
					summonList = false;
					chess.GetComponent<CharacterProperty>().Activated = true;
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
								gf.GetComponent<CharacterProperty>().death = false;
								guiShow = false;
								chess.GetComponent<CharacterProperty>().Activated = true;
							}
							summonList = false;
						}	
					}
				}
			}
		}
	}
	
    void Update() {
		panning();
		selecting();
    }
	
}
