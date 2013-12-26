using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;

public class CtrlStealing : MonoBehaviour, CommonSkill{
	public Transform stealFX;
	int Player;
	IList aTerritory, bTerritory;
	int skillRate;
	RoundCounter chessStorage;
	selection currentSel; 
	Transform selectedMap, target;
	SkillSlidingUI sUI;
	NpcPlayer npc;
	
	// Use this for initialization
	void Start () {
		chessStorage = Camera.main.GetComponent<RoundCounter>();
		currentSel = Camera.main.GetComponent<selection>();
		npc = GameObject.Find("NpcPlayer").transform.GetComponent<NpcPlayer>();
	}
	
	public void InsertSelection(Transform map){
		selectedMap = map;
		target = MapHelper.GetMapOccupiedObj(map);
	}
	
	public IList GetSelectionRange(){
		IList selectionRange = new List<Transform>();
		Player = transform.parent.parent.GetComponent<CharacterProperty>().Player;
		if(Player == 1){
			foreach(Transform chess in chessStorage.PlayerBChesses){
				CharacterProperty property = chess.GetComponent<CharacterProperty>();
				if(!property.death && !property.Summoner){
					selectionRange.Add(chess.GetComponent<CharacterSelect>().getMapPosition());
				}
			}
		}else if(Player == 2){
			foreach(Transform chess in chessStorage.PlayerAChesses){
				CharacterProperty property = chess.GetComponent<CharacterProperty>();
				if(!property.death && !property.Summoner){
					selectionRange.Add(chess.GetComponent<CharacterSelect>().getMapPosition());
				}
			}
		}
		
		return selectionRange;
	}
	
	public void ActivateControl(){
		Player = transform.parent.parent.GetComponent<CharacterProperty>().Player;
		CharacterProperty targetProperty = target.GetComponent<CharacterProperty>();
		if(Player == 1){
			chessStorage.PlayerAChesses.Add(target);
			chessStorage.PlayerBChesses.Remove(target);
			targetProperty.Player = 1;
		}else if(Player == 2){
			/*
			if(currentSel.npcMode){
				npc.InsertNewGf(target);
			}*/
			chessStorage.PlayerBChesses.Add(target);
			chessStorage.PlayerAChesses.Remove(target);
			targetProperty.Player = 2;
		}
		targetProperty.Attacked = false;
		targetProperty.TurnFinished = false;
		targetProperty.CmdTimes = 3; 
	}
	
	public void Execute(){
		sUI = Camera.mainCamera.GetComponent<SkillSlidingUI>();
		SkillUI sui = new SkillUI(target, true, "Controlled");
		sUI.UIItems.Add(sui);
		sUI.FadeInUI = true;
		MapHelper.SetFX(transform.parent.parent, stealFX, 4.0f);
		MapHelper.SetFX(target, stealFX, 4.0f);
		
	}
	
}
