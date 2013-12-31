using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;

public class InitStage : MonoBehaviour {
	public Texture2D InitImg, TutorBut, SelCmd, MoveCmd, ToMap, BuffExplan, DefCmd, NextBut, SecondMove, AtkCmd, FinalKill;
	public bool ShowGUI = true;
	Rect initImgRect, butRect, overRect, selCmdRect, moveCmdRect, mapRect, nextRect, defRect, buffRect, 
	next2Rect ,secMoveRect, atkRect, finalRect; 
	Vector2 mousePos;
	Transform playerA, playerB;
	float _mainAlpha = 1.0f;
	public bool ShowSelCmd, ShowMoveCmd, ShowMap, ShowBuff, ShowDef, ShowSecMove, ShowAtk, ShowFinal; 
	public int stage;
	// Use this for initialization
	void Start () {
		initImgRect = new Rect(.0f, .0f, Screen.width, Screen.height);
		butRect = new Rect(300.0f, 450.0f, 208.0f, 49.0f);
		overRect = new Rect(296.0f, 449.0f, 216.0f, 51.0f);
		selCmdRect = new Rect(490.0f, 150.0f, 286.0f, 153.0f);
		moveCmdRect = new Rect(80.0f, 37.0f, 314.0f, 82.0f);
		mapRect = new Rect(540.0f, 165.0f, 280.0f, 155.0f);
		buffRect = new Rect(625.0f, 45.0f, 571.0f, 321.0f);
		defRect = new Rect(80.0f, 263.0f, 368.0f, 74.0f);
		nextRect = new Rect(1055.0f, 295.0f, 122.0f, 52.0f);
		secMoveRect = new Rect(770.0f,335.0f,318.0f,116.0f);
		atkRect = new Rect(80.0f,165.0f,343.0f,53.0f);
		finalRect = new Rect(355.0f,246.0f,606.0f,171.0f);
		next2Rect = new Rect(842.0f,363.0f,97.0f, 42.0f);
		mousePos = new Vector2();
		ShowSelCmd = false;
		ShowMoveCmd = false;
		ShowMap = false;
		ShowBuff = false;
		ShowDef = false;
		ShowSecMove = false;
		ShowAtk = false;
		ShowFinal = false;
		playerA = Camera.mainCamera.GetComponent<RoundCounter>().playerA;
		playerB = Camera.mainCamera.GetComponent<RoundCounter>().playerB;
	}
	
	// Update is called once per frame
	void Update () {
		mousePos.x = Input.mousePosition.x;
		mousePos.y = Screen.height-Input.mousePosition.y;
	}
	
	void OnGUI(){
		GUI.depth = 0;
		GUI.backgroundColor = Color.clear;
		GUI.color = new Color(1.0f,1.0f,1.0f,_mainAlpha);
		if(ShowGUI){
			GUI.DrawTexture(initImgRect,InitImg);
			if(GUI.Button(butRect, TutorBut)){
				ShowGUI = false;
				transform.GetComponent<ShowSummoner>().ShowTower();
				transform.GetComponent<ShowSummoner>().ShowSummonerA();
			} 
			if(butRect.Contains(mousePos)){
				GUI.DrawTexture(overRect, TutorBut);
			}
		}
		if(ShowSelCmd){
			GUI.DrawTexture(selCmdRect, SelCmd);
		}
		if(ShowMoveCmd){
			GUI.DrawTexture(moveCmdRect, MoveCmd);
		}
		if(ShowMap){
			GUI.DrawTexture(mapRect, ToMap);
		}
		if(ShowBuff){
			GUI.DrawTexture(buffRect, BuffExplan);
			if(GUI.Button(nextRect, NextBut)){
				ShowBuff = false;
				ShowDef = true;
			}
			if(nextRect.Contains(mousePos)){
				Rect over = new Rect(1053.0f, 294.0f, 126.0f, 54.0f);
				GUI.DrawTexture(over, NextBut);
			}
		}
		if(ShowDef){
			GUI.DrawTexture(defRect, DefCmd);
		}
		if(ShowSecMove){
			GUI.DrawTexture(secMoveRect, SecondMove);
			
			Transform theMap = playerB.GetComponent<CharacterSelect>().getMapPosition();
			if(theMap!=null){
				IList maps = new List<Transform>();
				foreach(Transform m in theMap.GetComponent<Identity>().Neighbor){
					if(m!=null)
						maps.Add(m);
				}
				foreach(Transform m in maps){
					if(playerA == MapHelper.GetMapOccupiedObj(m)){
						ShowSecMove = false;
						ShowAtk = true;
						break;
					}
				}
			}
		}
		if(ShowAtk){
			GUI.DrawTexture(atkRect, AtkCmd);
		}
		if(ShowFinal){
			GUI.DrawTexture(finalRect, FinalKill);
			if(GUI.Button(next2Rect, NextBut)){
				ShowFinal = false; 
				//Camera.mainCamera.GetComponent<MainUI>().InSecondTutor = false;
			}
		}
	}
}
