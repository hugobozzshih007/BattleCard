using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;

public class PoisonAid : MonoBehaviour, CommonSkill {
	
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
			selectionRange.Add(map);
		}
		return selectionRange;
	}
	
	public void Execute ()
	{
		sUI = Camera.mainCamera.GetComponent<SkillSlidingUI>();
		CharacterProperty targetProperty = target.GetComponent<CharacterProperty>();
		if(transform.GetComponent<SkillProperty>().PassSkillRate){
			targetProperty.UnStatusCounter[UnnormalStatus.Poisoned] = 2;
			targetProperty.LastUnStatusCounter[UnnormalStatus.Poisoned] = 3;
			targetProperty.UnStatus = true;
			print("Poisoned target");
			SkillUI sui = new SkillUI(target, true, "poisoned");
			sUI.UIItems.Add(sui);
			sUI.FadeInUI = true;
		}else{
			SkillUI sui = new SkillUI(attacker, false, "");
			sUI.UIItems.Add(sui);
			sUI.FadeInUI = true;
			print("Poison failed");
		}
	}
}
