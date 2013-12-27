using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;

public class DarkBallAttack : MonoBehaviour, SkillInterface{
	Transform attacker;
	Transform model; 
	int damage = 2;
	DamageSlidingUI sUI;
	// Use this for initialization
	void Start () {
		attacker = transform.parent.parent; 
		model = attacker.Find("Models");
		sUI = Camera.mainCamera.GetComponent<DamageSlidingUI>();
	}
	public void InsertSelection (Transform map)
	{
		throw new System.NotImplementedException ();
	}
	
	IList GetTargets(){
		IList atkList = new List<Transform>();
		Transform localMap = attacker.GetComponent<CharacterSelect>().getMapPosition();
		Transform[] targetMaps = localMap.GetComponent<Identy>().neighbor;
		foreach(Transform unit in targetMaps){
			if((unit!=null) && MapHelper.IsMapOccupied(unit)){
				Transform character = MapHelper.GetMapOccupiedObj(unit);
				if(character.GetComponent<CharacterProperty>().Player != attacker.GetComponent<CharacterProperty>().Player){
					atkList.Add(character);
				}
			}
		}
		return atkList;
	}
	
	public IList GetSelectionRange ()
	{
		IList atkList = new List<Transform>();
		atkList = GetTargets();
		IList atkMapList = new List<Transform>();
		if(atkList.Count>0){
			foreach(Transform target in atkList){
				atkMapList.Add(target.GetComponent<CharacterSelect>().getMapPosition());
			}
		}
		return atkMapList;
	}
	
	public void Execute (){
		IList atkList = new List<Transform>();
		atkList = GetTargets();
		model.GetComponent<DarkBallFX>().InsertTargets(atkList);
	}
	
	public void CalculateDamge(){
		IList atkList = new List<Transform>();
		atkList = GetTargets();
		if(atkList.Count>0){
			foreach(Transform gf in atkList){
				CharacterProperty gfp = gf.GetComponent<CharacterProperty>();
				gfp.Hp -= damage;
				if(gfp.Hp < 0)
					gfp.Hp = 0; 
				
				//print("Bombed critical hit!");
			}
		}
	}
	
	public void ShowDamge(){
		IList atkList = new List<Transform>();
		atkList = GetTargets();
		if(atkList.Count>0){
			foreach(Transform gf in atkList){
				DamageSlidingFX gfDFX = gf.GetComponent<DamageSlidingFX>();
				gfDFX.ActivateSlidingFX(attacker, damage);
			}
		}
	}
	// Update is called once per frame
	void Update () {
	
	}
}
