using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;

public class AttackCalFX : MonoBehaviour {
	
	public Transform Attacker;
	public Transform Target;
	public bool CriticalHit;
	public bool FightBack = true;
	Transform attackerLocation; 
	Transform targetLocation;
	CharacterProperty attackerProperty, targetProperty;
	CharacterSelect attackerSelect, targetSelect;
	//DamageSlidingUI sUI;
	DeathFX dFX;
	
	bool wait = false;
	
	MainUI mUI;
	MainInfoUI chessUI;
	CommonFX cFX;
	
	FightBack fb;
	
	StatusMachine sMachine; 
	// Use this for initialization
	void Start () {
		mUI = transform.GetComponent<MainUI>();
		fb = transform.GetComponent<FightBack>();
		chessUI = transform.GetComponent<MainInfoUI>();
		cFX =  transform.GetComponent<CommonFX>();
		sMachine = GameObject.Find("StatusMachine").GetComponent<StatusMachine>();
		//currentSel = transform.GetComponent<selection>();
	}
	
	public void SetAttackSequence(Transform attacker, Transform targetMap){
		this.Attacker = attacker;
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
		GameObject blood = Instantiate(cFX.NormalAttack,pos,Quaternion.identity) as GameObject;
		Destroy(blood,3.0f);
		if(!sMachine.TutorialMode){
			chessUI.Talking = false;
			chessUI.TalkingRight = false;
		}
		//mUI.MainGuiFade = true;
		//mUI.SubGuiFade = false;	
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
		bool RengeAtker = MapHelper.CheckPassive(PassiveType.FlyingHit, attacker);
		bool flyAtker = MapHelper.CheckPassive(PassiveType.Flying, attacker);
		foreach(Transform map in targets){
			Transform enemy = MapHelper.GetMapOccupiedObj(map);
			bool flyable = MapHelper.CheckPassive(PassiveType.Flying, enemy);
			
			if(!RengeAtker && !flyAtker){
				if(!flyable)
					ableTargets.Add(map);
			}else{
				ableTargets.Add(map);
			}
		}
		return ableTargets;
	}
	
	public IList GetAttackableTarget(Transform root, Transform attacker){
		updateMapSteps();
		IList ableTargets = new List<Transform>();
		IList targets = new List<Transform>();
		targets = attacker.GetComponent<CharacterProperty>().GetAttackPosition(root);
		bool RengeAtker = MapHelper.CheckPassive(PassiveType.FlyingHit, attacker);
		bool flyAtker = MapHelper.CheckPassive(PassiveType.Flying, attacker);
		foreach(Transform map in targets){
			Transform enemy = MapHelper.GetMapOccupiedObj(map);
			bool flyable = MapHelper.CheckPassive(PassiveType.Flying, enemy);
			if(!RengeAtker && !flyAtker){
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
			allMap.GetChild(i).GetComponent<Identity>().step = 0;
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
		DamageSlidingFX targetSFX = target.GetComponent<DamageSlidingFX>();
		targetSFX.ActivateSlidingFX(atk, damage);
	}
	
	// Update is called once per frame
	void Update () {
	}
	int GetTrueDamage(float damage, int def){
		int newDamage = Mathf.RoundToInt(damage);
		int trueDamage = (newDamage - def);
		if(trueDamage < 0)
			trueDamage = 0; 
		return trueDamage;
	}
	
	public void UpdateAttackResult(AttackType mode){
		dFX = Camera.mainCamera.GetComponent<DeathFX>();
		if(mode == AttackType.physical){
			if(Attacker.GetComponent<CharacterPassive>().PassiveDict[PassiveType.SuddenDeath]){
				if(!targetProperty.Tower && MapHelper.Success(10)){
					targetProperty.Hp = 1;
					ShowDamageUI(Target, targetProperty.MaxHp-1, Attacker);
				}
				else{
					int trueDamage = GetTrueDamage((float)attackerProperty.Damage, targetProperty.ModifiedDefPow);
					targetProperty.Hp -= trueDamage;
					Target.GetComponent<BuffList>().ExtraDict[BuffType.Defense] = 0;
					targetProperty.Damaged = true;
					ShowDamageUI(Target, trueDamage, Attacker);
				}
				
			}else if(CriticalHit){
				int trueDamage = GetTrueDamage((float)attackerProperty.Damage*1.5f,targetProperty.ModifiedDefPow);
				targetProperty.Hp -= trueDamage;
				Target.GetComponent<BuffList>().ExtraDict[BuffType.Defense] = 0;
				targetProperty.Damaged = true;
				ShowDamageUI(Target, trueDamage, Attacker);
				Debug.Log("Critical Hit!");
			}else{
				int trueDamage = GetTrueDamage((float)attackerProperty.Damage, targetProperty.ModifiedDefPow);
				targetProperty.Hp -= trueDamage;
				Target.GetComponent<BuffList>().ExtraDict[BuffType.Defense] = 0;
				targetProperty.Damaged = true;
				ShowDamageUI(Target, trueDamage, Attacker);
			}
			
			if(Attacker.GetComponent<CharacterPassive>().PassiveDict[PassiveType.WoundBite]){
				targetProperty.AbleDef = false;
				targetProperty.ModifiedDefPow = 0;
				targetProperty.UnStatusCounter[UnnormalStatus.Wounded] = 1;
				targetProperty.LastUnStatusCounter[UnnormalStatus.Wounded] = 2;
			}
			
		}else if(mode == AttackType.magical){
			if(CriticalHit){
				targetProperty.Hp -= attackerProperty.Damage*2;
				Target.GetComponent<BuffList>().ExtraDict[BuffType.Defense] = 0;
				targetProperty.Damaged = true;
				Debug.Log("Critical Hit!");
			}else{
				print("damage!!");
				int trueDamage = GetTrueDamage((float)attackerProperty.Damage, targetProperty.ModifiedDefPow);
				targetProperty.Hp -= trueDamage;
				Target.GetComponent<BuffList>().ExtraDict[BuffType.Defense] = 0;
				targetProperty.Damaged = true;
				ShowDamageUI(Target, trueDamage, Attacker);
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
			targetProperty.Hp = 0;
			
				//dFX.SetDeathSequence(Target);
				//targetProperty.Ready = false;
				//targetProperty.WaitRounds = targetProperty.StandByRounds;
				chessUI.DelayFadeOut = true;
				targetProperty.Attacked = true;
				//cancel attack buff
				Target.GetComponent<BuffList>().ExtraDict[BuffType.Attack] = 0;
				Target.GetComponent<BuffList>().ExtraDict[BuffType.CriticalHit] = 0;
				targetProperty.CmdTimes -= 1;
				mUI.TurnFinished(Target, false);
				if(chessUI.playerSide ==1)
					chessUI.MainFadeIn = false;
				else
					chessUI.TargetFadeIn = false;
		
		}else{
			if(FightBack){
				fb.SetFightBack(Target,Attacker);
				targetProperty.Attacked = true;
				//cancel attack buff
				Target.GetComponent<BuffList>().ExtraDict[BuffType.Attack] = 0;
				Target.GetComponent<BuffList>().ExtraDict[BuffType.CriticalHit] = 0;
				targetProperty.CmdTimes -= 1;
				mUI.TurnFinished(Target, false);
			}else{
				sMachine.InBusy = false;
			}
		}
		attackerProperty.Attacked = true;
		//cancel attack buff
		Attacker.GetComponent<BuffList>().ExtraDict[BuffType.Attack] = 0;
		Attacker.GetComponent<BuffList>().ExtraDict[BuffType.CriticalHit] = 0;
		attackerProperty.CmdTimes -= 1;
		mUI.TurnFinished(Attacker, false);
	}
}
