using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;

public class BombAttack : MonoBehaviour, CommonSkill {
	
	int damage;
	int skillRate;
	Transform attacker, target, targetMap;
	DamageSlidingUI sUI;
	
	// Use this for initialization
	void Start () {
		damage = 1;
		attacker = transform.parent.parent;
	}
	
	public void InsertSelection (Transform map)
	{
		target = MapHelper.GetMapOccupiedObj(map);
		targetMap = map;
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
		IList targetList = new List<Transform>();
		targetList.Add(target);
		sUI = Camera.mainCamera.GetComponent<DamageSlidingUI>();
		Transform[] mapArray = targetMap.GetComponent<Identy>().neighbor;
		foreach(Transform map in mapArray){
			if(map!=null){
				if(MapHelper.IsMapOccupied(map)){
					targetList.Add(MapHelper.GetMapOccupiedObj(map));
				}
			}
		}
		foreach(Transform unit in targetList){
			if(transform.GetComponent<SkillProperty>().PassSkillRate){
				damage = 2;
				unit.GetComponent<CharacterProperty>().Hp -= damage;
				DamageUI dUI = new DamageUI(unit,damage);
				sUI.UIItems.Add(dUI);
				sUI.FadeInUI = true;
				print("Bombed critical hit!");
			}else{
				damage = 1;
				unit.GetComponent<CharacterProperty>().Hp -= damage;
				DamageUI dUI = new DamageUI(unit,damage);
				sUI.UIItems.Add(dUI);
				sUI.FadeInUI = true;
				print("Bombed!");
			}
		}
	}
}
