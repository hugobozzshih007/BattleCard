using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;

public class ChaosAid : MonoBehaviour, CommonSkill {
	
	int skillRate;
	RoundCounter chessStorage;
	Transform attacker, target; 
	SkillSlidingUI sUI;
	// Use this for initialization
	void Start () {
		attacker = transform.parent.parent;
		chessStorage = Camera.main.GetComponent<RoundCounter>();
	}
	
	public void InsertSelection (Transform map)
	{
		target = MapHelper.GetMapOccupiedObj(map);
	}
	
	public IList GetSelectionRange ()
	{
		IList selectionRange = new List<Transform>();
		AttackCalculation atkCal = new AttackCalculation(attacker);
		foreach(Transform map in atkCal.GetMagicTarget(atkCal.Attacker)){
			Transform chess = MapHelper.GetMapOccupiedObj(map);
			if(!chess.GetComponent<CharacterProperty>().Summoner)
				selectionRange.Add(map);
		}
		return selectionRange;
	}
	
	public void Execute ()
	{
		sUI = Camera.mainCamera.GetComponent<SkillSlidingUI>();
		int Player = transform.parent.GetComponent<CharacterProperty>().Player; 
		CharacterProperty targetProperty = target.GetComponent<CharacterProperty>();
		if(transform.GetComponent<SkillProperty>().PassSkillRate){
			if(Player == 1){
				chessStorage.PlayerAChesses.Add(target);
				chessStorage.PlayerBChesses.Remove(target);
				targetProperty.Player = 1;
			}else if(Player == 2){
				chessStorage.PlayerBChesses.Add(target);
				chessStorage.PlayerAChesses.Remove(target);
				targetProperty.Player = 2;
			}
			targetProperty.UnStatusCounter[UnnormalStatus.Chaos] = 2;
			targetProperty.LastUnStatusCounter[UnnormalStatus.Chaos] = 2;
			targetProperty.UnStatus = true;
			SkillUI sui = new SkillUI(target, true, "Chaos");
			sUI.UIItems.Add(sui);
			sUI.FadeInUI = true;
			print("Target get chaos!!");
		}else{
			SkillUI sui = new SkillUI(attacker, false, "");
			sUI.UIItems.Add(sui);
			sUI.FadeInUI = true;
			print("Chaos failed");
		}
	}
}
