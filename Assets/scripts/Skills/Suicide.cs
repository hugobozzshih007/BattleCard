using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;

public class Suicide : MonoBehaviour, CommonSkill {
	
	Transform attacker;
	DamageSlidingUI sUI;
	
	// Use this for initialization
	void Start () {
		attacker = transform.parent.parent;
	}
	
	public void InsertSelection (Transform map)
	{
		throw new System.NotImplementedException ();
	}
	
	public IList GetSelectionRange ()
	{
		throw new System.NotImplementedException ();
	}
	
	public void Execute ()
	{
		sUI = Camera.mainCamera.GetComponent<DamageSlidingUI>();
		IList atkList = new List<Transform>();
		Transform localMap = attacker.GetComponent<CharacterSelect>().getMapPosition();
		Transform[] attackableMaps = localMap.GetComponent<Identy>().neighbor;
		foreach(Transform unit in attackableMaps){
			if((unit!=null) && MapHelper.IsMapOccupied(unit)){
				atkList.Add(MapHelper.GetMapOccupiedObj(unit));
			}
		}
		int d = attacker.GetComponent<CharacterProperty>().Hp+1;
		attacker.GetComponent<CharacterProperty>().Hp -= (attacker.GetComponent<CharacterProperty>().Hp+1);
		DamageUI aDieUI = new DamageUI(attacker, d, attacker);
		sUI.UIItems.Add(aDieUI);
		
		if(atkList.Count>0){
			foreach(Transform target in atkList){
				target.GetComponent<CharacterProperty>().Hp -= attacker.GetComponent<CharacterProperty>().Damage;
				DamageUI dUI = new DamageUI(target,attacker.GetComponent<CharacterProperty>().Damage, attacker);
				sUI.UIItems.Add(dUI);
			}
			sUI.FadeInUI = true;
		}
	}
}
