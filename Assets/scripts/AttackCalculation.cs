using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;

public class AttackCalculation{
	public Transform Attacker;
	public Transform Target;
	Transform attackerLocation; 
	Transform targetLocation;
	CharacterProperty attackerProperty, targetProperty;
	CharacterSelect attackerSelect, targetSelect;
	
	public AttackCalculation(Transform attacker){
		Attacker = attacker;
		attackerProperty = Attacker.GetComponent<CharacterProperty>();
		attackerSelect = Attacker.GetComponent<CharacterSelect>();
		attackerLocation = attackerSelect.getMapPosition();
	}
	
	public void InsertTarget(Transform targetMap){
		targetLocation = targetMap;
		Target = MapHelper.GetMapOccupiedObj(targetLocation);
		targetProperty = Target.GetComponent<CharacterProperty>();
		targetSelect = Target.GetComponent<CharacterSelect>();
	}
	
	public bool Attackable(){
		bool attackable = false;
		return attackable;
	}
	
	public IList GetAttableTarget(){
		IList ableTargets = new List<Transform>();
		IList targets = new List<Transform>();
		targets = attackerProperty.GetAttackPosition();
		bool flyHitable = MapHelper.CheckPassive(PassiveType.FlyingHit, Attacker);
		foreach(Transform map in targets){
			Transform enemy = MapHelper.GetMapOccupiedObj(map);
			bool flyable = MapHelper.CheckPassive(PassiveType.Flying, enemy);
			if(!flyHitable){
				if(!flyable)
					ableTargets.Add(map);
			}else{
				ableTargets.Add(map);
			}
		}
		return targets;
	}
	
	public void UpdateAttackResult(){
	}
}
