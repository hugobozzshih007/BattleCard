using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;

public class MapStealing : MonoBehaviour, CommonSkill {
	
	int Player;
	IList aTerritory, bTerritory;
	int skillRate;
	RoundCounter mapStorage;
	Transform selectedMap, target;
	SkillSlidingUI sUI;
	
	// Use this for initialization
	void Start () {
		mapStorage = Camera.main.GetComponent<RoundCounter>();
	}
	
	public void InsertSelection(Transform map){
		selectedMap = map;
		target = MapHelper.GetMapOccupiedObj(map);
	}
	
	public IList GetSelectionRange(){
		IList selectionRange = new List<Transform>();
		Player = transform.parent.parent.GetComponent<CharacterProperty>().Player;
		if(Player==1){
			foreach(Transform map in mapStorage.PlayerBTerritory){
				selectionRange.Add(map);
			}
		}else if(Player==2){
			foreach(Transform map in mapStorage.PlayerATerritory){
				selectionRange.Add(map);
			}
		}
		return selectionRange;
	}
	
	public void Execute(){
		sUI = Camera.mainCamera.GetComponent<SkillSlidingUI>();
		
			Player = transform.parent.parent.GetComponent<CharacterProperty>().Player;
			if(selectedMap!=null){
				if(Player == 1){
					mapStorage.PlayerATerritory.Add(selectedMap);
					mapStorage.PlayerBTerritory.Remove(selectedMap);
					print("Stealed playerB's map!");
				}else if(Player == 2){
					mapStorage.PlayerBTerritory.Add(selectedMap);
					mapStorage.PlayerATerritory.Remove(selectedMap);
					print("Stealed playerA's map!");
				}
				SystemSound sSound = GameObject.Find("SystemSound").transform.GetComponent<SystemSound>();
				sSound.PlaySound(SysSoundFx.Defense);
			}
			SkillUI sui = new SkillUI(selectedMap, true, "map stealed");
			sUI.UIItems.Add(sui);
			GameObject.Find("StatusMachine").GetComponent<StatusMachine>().InBusy = false;
			sUI.FadeInUI = true;
		
	}
}
