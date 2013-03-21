using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;

public class AttackCalFX : MonoBehaviour {
	
	public Transform Attacker;
	public Transform Target;
	public bool CriticalHit;
	public bool fightBack = true;
	Transform attackerLocation; 
	Transform targetLocation;
	CharacterProperty attackerProperty, targetProperty;
	CharacterSelect attackerSelect, targetSelect;
	DamageSlidingUI sUI;
	DeathFX dFX;
	
	bool wait = false;
	
	MainUI mUI;
	MainInfoUI chessUI;
	CommonFX cFX;
	
	FightBack fb;
	// Use this for initialization
	void Start () {
		mUI = transform.GetComponent<MainUI>();
		sUI = transform.GetComponent<DamageSlidingUI>();
		fb = transform.GetComponent<FightBack>();
		chessUI = transform.GetComponent<MainInfoUI>();
		cFX =  transform.GetComponent<CommonFX>();
		//currentSel = transform.GetComponent<selection>();
	}
	
	public void SetAttackSequence(Transform attacker, Transform targetMap){
		Attacker = attacker;
		//CriticalHit = false;
		attackerProperty = Attacker.GetComponent<CharacterProperty>();
		attackerSelect = Attacker.GetComponent<CharacterSelect>();
		attackerLocation = attackerSelect.getMapPosition();
		targetLocation = targetMap;
		Target = MapHelper.GetMapOccupiedObj(targetLocation);
		targetProperty = Target.GetComponent<CharacterProperty>();
		targetSelect = Target.GetComponent<CharacterSelect>();
		
		UpdateAttackResult(AttackType.physical);
		Vector3 pos = new Vector3(Target.transform.position.x,Target.transform.position.y,Target.transform.position.z);
		Transform blood = Instantiate(cFX.NormalAttack,pos,Quaternion.identity) as Transform;
		Destroy(GameObject.Find(blood.name).gameObject,3.0f);
		mUI.MainGuiFade = true;
		mUI.SubGuiFade = false;	
	}
	
	public bool Attackable(Transform atk, Transform newTarget){
		IList targetList = new List<Transform>();
		targetList = GetAttackableTarget(atk);
		bool attackable = false;
		Transform map = newTarget.GetComponent<CharacterSelect>().getMapPosition();
		attackable = targetList.Contains(map);
		return attackable;
	}
	
	public bool CalcriticalHit(Transform attacker, AttackType mode){
		bool hit = false;
		CharacterProperty aProperty = attacker.GetComponent<CharacterProperty>();
		int realNum = Random.Range(1,100);
		if(mode == AttackType.physical){
			if(realNum <= aProperty.BuffCriticalHit){
				hit = true;
			}else{
				hit = false;
			}
		}else if(mode == AttackType.magical){
			if(realNum <= aProperty.BuffSkillRate){
				hit = true;
			}else{
				hit = false;
			}
		}
		return hit;
	}
	
	public IList GetAttackableTarget(Transform attacker){
		updateMapSteps();
		IList ableTargets = new List<Transform>();
		IList targets = new List<Transform>();
		targets = attacker.GetComponent<CharacterProperty>().GetAttackPosition();
		bool flyHitable = MapHelper.CheckPassive(PassiveType.FlyingHit, attacker)|| MapHelper.CheckPassive(PassiveType.Flying, attacker);
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
		return ableTargets;
	}
	
	void updateMapSteps(){
		Transform allMap = GameObject.Find("Maps").transform;
		int mapUnitNum = allMap.GetChildCount();
		for(int i=0;i<mapUnitNum;i++){
			allMap.GetChild(i).GetComponent<Identy>().step = 0;
			//print(allMap.GetChild(i).name + ".step=" + allMap.GetChild(i).GetComponent<Identy>().step);
		}
	}
	
	public IList GetMagicTarget(Transform attacker){
		IList ableTargets = new List<Transform>();
		IList targets = new List<Transform>();
		targets = attacker.GetComponent<CharacterProperty>().GetAttackPosition();
		foreach(Transform map in targets){
			ableTargets.Add(map);
		}
		return ableTargets;
	}
	
	void ShowDamageUI(Transform target,int damage, Transform atk){
		DamageUI dUI = new DamageUI(target,damage, atk);
		sUI.UIItems.Add(dUI);
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	
	public void UpdateAttackResult(AttackType mode){
		dFX = Camera.mainCamera.GetComponent<DeathFX>();
		if(mode == AttackType.physical){
			if(Attacker.GetComponent<CharacterPassive>().PassiveDict[PassiveType.SuddenDeath]){
				if(!targetProperty.Tower && MapHelper.Success(10)){
					targetProperty.Hp = 0;
				}
				else{
					targetProperty.Hp -= attackerProperty.Damage;
					targetProperty.Damaged = true;
					ShowDamageUI(Target, attackerProperty.Damage, Attacker);
				}
				
			}/*else if(Attacker.GetComponent<CharacterPassive>().PassiveDict[PassiveType.MultiArrow]){
				IList targetList = new List<Transform>();
				foreach(Transform unit in GetAttackableTarget(Attacker)){
					targetList.Add(MapHelper.GetMapOccupiedObj(unit));
				}
				if(targetList.Contains(Target))
					targetList.Remove(Target);
				if(CriticalHit){
					targetProperty.Hp -= attackerProperty.Damage*2;
					ShowDamageUI(Target, attackerProperty.Damage*2, Attacker);
					Debug.Log("Critical Hit!");
				}else{
					targetProperty.Hp -= attackerProperty.Damage;
					ShowDamageUI(Target, attackerProperty.Damage, Attacker);
				}
				
				Transform[] tArray = new Transform[targetList.Count];
				targetList.CopyTo(tArray,0);
				if(tArray.Length > 1){
					for(int i=0;i<2;i++){
						tArray[i].GetComponent<CharacterProperty>().Hp -= 1;
						ShowDamageUI(tArray[i], 1, Attacker);
					}
				}else if(tArray.Length < 2){
					for(int i=0;i<tArray.Length;i++){
						tArray[i].GetComponent<CharacterProperty>().Hp -= 1;
						ShowDamageUI(tArray[i], 1, Attacker);
					}
				}
				
			}*/else if(CriticalHit){
				targetProperty.Hp -= attackerProperty.Damage*2;
				targetProperty.Damaged = true;
				ShowDamageUI(Target, attackerProperty.Damage*2, Attacker);
				Debug.Log("Critical Hit!");
			}else{
				targetProperty.Hp -= attackerProperty.Damage;
				targetProperty.Damaged = true;
				ShowDamageUI(Target, attackerProperty.Damage, Attacker);
			}
			
			if(Attacker.GetComponent<CharacterPassive>().PassiveDict[PassiveType.WoundBite]){
				targetProperty.UnStatusCounter[UnnormalStatus.Wounded] = 1;
				targetProperty.LastUnStatusCounter[UnnormalStatus.Wounded] = 2;
			}
			
		}else if(mode == AttackType.magical){
			if(CriticalHit){
				targetProperty.Hp -= attackerProperty.Damage*2;
				targetProperty.Damaged = true;
				Debug.Log("Critical Hit!");
			}else{
				print("damage!!");
				targetProperty.Hp -= attackerProperty.Damage;
				targetProperty.Damaged = true;
				ShowDamageUI(Target, attackerProperty.Damage, Attacker);
			}
		}
		/*
		if(Attackable()){
			attackerProperty.Hp -= targetProperty.Damage;
		}*/
		
		if(attackerProperty.Hp<=0){
			//attackerProperty.death = true;
			//dFX.SetDeathSequence(Attacker);
			attackerProperty.Ready = false;
			attackerProperty.WaitRounds = attackerProperty.StandByRounds;
		}
		
		if(targetProperty.Hp<=0){
			if(!targetProperty.Tower){
				//dFX.SetDeathSequence(Target);
				//targetProperty.Ready = false;
				//targetProperty.WaitRounds = targetProperty.StandByRounds;
				chessUI.DelayFadeOut = true;
				targetProperty.Attacked = true;
				if(chessUI.playerSide ==1)
					chessUI.MainFadeIn = false;
				else
					chessUI.TargetFadeIn = false;
			}else{
				targetProperty.death = true;
			}
		}else{
			if(fightBack){
				fb.SetFightBack(Target,Attacker);
				targetProperty.Attacked = true;
			}
		}
		attackerProperty.Attacked = true;
		sUI.FadeInUI = true;
	}
	
	
	
}
