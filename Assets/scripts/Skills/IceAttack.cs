using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;

public class IceAttack : MonoBehaviour, SkillInterface {
	
	int skillRate;
	//RoundCounter chessStorage;
	Transform attacker, target;
	
	// Use this for initialization
	void Start () {
		attacker = transform.parent.parent;
		//chessStorage = Camera.main.GetComponent<RoundCounter>();
	}
	
	public void InsertSelection (Transform map)
	{
		target = MapHelper.GetMapOccupiedObj(map);
	}
	
	public IList GetSelectionRange ()
	{
		IList selectionRange = new List<Transform>();
		AttackCalFX atkCal = Camera.main.GetComponent<AttackCalFX>();
		foreach(Transform map in atkCal.GetMagicTarget(attacker)){
			selectionRange.Add(map);
		}
		return selectionRange;
	}
	
	public void Execute ()
	{
		CharacterProperty targetProperty = target.GetComponent<CharacterProperty>();
		if(transform.GetComponent<SkillProperty>().PassSkillRate){
			targetProperty.UnStatusCounter[UnnormalStatus.Freezed] = 4;
			targetProperty.LastUnStatusCounter[UnnormalStatus.Freezed] = 5;
			targetProperty.UnStatus = true;
			print("Freezed target");
		}else{
			print("Freez failed");
		}
	}
}
