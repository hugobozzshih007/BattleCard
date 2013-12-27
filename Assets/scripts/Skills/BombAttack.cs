using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;

public class BombAttack : MonoBehaviour, SkillInterface {
	Transform models; 
	int damage;
	int skillRate;
	Transform attacker, target, targetMap;
	DamageSlidingUI sUI;
	
	// Use this for initialization
	void Start () {
		damage = 1;
		sUI = Camera.mainCamera.GetComponent<DamageSlidingUI>();
		attacker = transform.parent.parent;
		models = attacker.Find("Models");
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
		models.GetComponent<FireBombFX>().InsertBombLocation(target);
	}
	
	void ShowDamageUI(Transform target,int damage, Transform atk){
		DamageSlidingFX targetSFX = target.GetComponent<DamageSlidingFX>();
		targetSFX.ActivateSlidingFX(atk, damage);
	}
	
	public void CalculateDamage(){
		IList targetList = new List<Transform>();
		targetList = MapHelper.GetAroundGFs(target.GetComponent<CharacterSelect>().getMapPosition());
		targetList.Add(target);
		CharacterProperty atkP = attacker.GetComponent<CharacterProperty>();
		foreach(Transform unit in targetList){
			CharacterProperty unitP = unit.GetComponent<CharacterProperty>();
			if(transform.GetComponent<SkillProperty>().PassSkillRate){
				damage = 2;
				if(atkP.Player != unitP.Player){
					unitP.Hp -= damage;
					ShowDamageUI(unit,2,attacker);
					print("Bombed critical hit!");
				}
			}else{
				damage = 1;
				if(atkP.Player != unitP.Player){
					unitP.Hp -= damage;
					ShowDamageUI(unit,1,attacker);
				print("Bombed!");
				}
			}
		}
	}
}
