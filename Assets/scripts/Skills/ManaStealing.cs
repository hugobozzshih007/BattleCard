using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;

public class ManaStealing :MonoBehaviour, SkillInterface{
	
	int Player;
	SkillProperty property;
	int skillRate;
	Transform playerA, playerB;
	SkillSlidingUI sUI;
	
	void Start(){
		playerA = GameObject.Find("pSummonerA").transform;
		playerB = GameObject.Find("pSummonerB").transform;
		property = transform.GetComponent<SkillProperty>();
		
	}
	
	public void InsertSelection(Transform map){
	}
	
	public IList GetSelectionRange(){
		IList nothing = new List<Transform>();
		return nothing;
	}
	
	public void Execute(){
		sUI = Camera.mainCamera.GetComponent<SkillSlidingUI>();
		if(transform.GetComponent<SkillProperty>().PassSkillRate){
			Player = transform.parent.parent.GetComponent<CharacterProperty>().Player;
			Transform target = null;
			Transform stealer = null;
			if(Player == 1){
				stealer = playerA;
				target = playerB;
			}else{
				stealer = playerB;
				target = playerA;
			}
			
			ManaCounter manaA = stealer.GetComponent<ManaCounter>();
			ManaCounter manaB = target.GetComponent<ManaCounter>();
			
			if(manaB.Mana>0){
				manaA.Mana += 1;
				manaB.Mana -= 1;
				SkillUI sui = new SkillUI(transform.parent.parent, true, "Mana +1");
				sUI.UIItems.Add(sui);
				sUI.FadeInUI = true;
				print("Mana stealed!");
			}else{
				SkillUI sui = new SkillUI(transform.parent.parent, true, "");
				sUI.UIItems.Add(sui);
				sUI.FadeInUI = true;
				print("No mana to steal!");
			}
		}else{
			print("Stealed nothing!");
		}
	}
}
