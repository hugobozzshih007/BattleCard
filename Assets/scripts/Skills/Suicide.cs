using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;

public class Suicide : MonoBehaviour, SkillInterface {
	int dmg = 5;
	Transform attacker;
	public Transform BombFX; 
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
		IList selectionRange = new List<Transform>();
		Transform thisPosition = attacker.GetComponent<CharacterSelect>().getMapPosition();  
		selectionRange.Add(thisPosition);  
		foreach(Transform map in thisPosition.GetComponent<Identity>().Neighbor){
			if(map!=null){
				selectionRange.Add(map);
			}
		}
		
		return selectionRange;
	}
	
	public void Execute ()
	{
		IList atkList = new List<Transform>();
		Transform localMap = attacker.GetComponent<CharacterSelect>().getMapPosition();
		Transform[] attackableMaps = localMap.GetComponent<Identity>().Neighbor;
		if(audio.clip != null)
			audio.Play();
		MapHelper.SetFX(attacker, BombFX, 2.5f);
		
		foreach(Transform unit in attackableMaps){
			if((unit!=null) && MapHelper.IsMapOccupied(unit)){
				atkList.Add(MapHelper.GetMapOccupiedObj(unit));
			}
		}
		int d = attacker.GetComponent<CharacterProperty>().Hp+1;
		attacker.GetComponent<CharacterProperty>().Hp -= d;
		DeathUI atkDUI = new DeathUI(attacker, attacker);
		
		if(atkList.Count>0){
			foreach(Transform target in atkList){
				target.GetComponent<CharacterProperty>().Hp -= dmg;
				int trueDmg = 5;
				if(dmg > target.GetComponent<CharacterProperty>().Hp)
					trueDmg = target.GetComponent<CharacterProperty>().Hp;
				target.GetComponent<DamageSlidingFX>().ActivateSlidingFX(attacker, trueDmg);
				if(target.GetComponent<CharacterProperty>().Hp <= 0){
					DeathUI dUI = new DeathUI(target, attacker);
				}
			}
		}
	}
}
