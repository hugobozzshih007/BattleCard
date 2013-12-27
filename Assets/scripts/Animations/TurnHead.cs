using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;

public class TurnHead : MonoBehaviour {
	
	public Transform Attacker;
	CharacterProperty akProperty;
	public Transform Target;
	Transform attackerModel, targetLocation;
	float t = 0.0f;
	float s = 0.0f;
	float timeToTurn = 0.5f;
	bool attackMode = false;
	bool turningHead = false;
	bool fightBackMode;
	bool critiqHit;
	MainInfoUI chessUI;
	Quaternion oldRotation;
	GeneralSelection currentSel;
	StatusMachine sMachine;
	// Use this for initialization
	void Start () {
		currentSel = transform.GetComponent<GeneralSelection>();
		chessUI = transform.GetComponent<MainInfoUI>();
		sMachine = GameObject.Find("StatusMachine").GetComponent<StatusMachine>();
	}
	
	public void SetTurnHeadSequence(Transform atk, Transform targetMap, bool doAttack, bool fightBack, bool critiq){
		Attacker = atk; 
		akProperty = Attacker.GetComponent<CharacterProperty>();
		targetLocation = targetMap;
		Target = MapHelper.GetMapOccupiedObj(targetMap);
		oldRotation = Attacker.transform.rotation;
		turningHead = true;
		attackMode = doAttack;
		fightBackMode = fightBack;
		critiqHit = critiq;
		//walking till turning ends
		attackerModel = Attacker.FindChild("Models");
		bool npc = false;
		if(currentSel.npcMode && akProperty.Player == 2)
			npc = true;
		if(!sMachine.TutorialMode)
			chessUI.TalkingShit(Attacker.GetComponent<TalkingContent>().GetAtkWords(), npc);
		if(attackerModel.GetComponent<AnimVault>()!=null)
			attackerModel.GetComponent<AnimVault>().CurrentState = AnimVault.AnimState.run;
	}
	
	// Update is called once per frame
	void Update () {
		if(turningHead){
			t+= Time.deltaTime/timeToTurn;
			Vector3 relativePos = Target.transform.position - Attacker.transform.position;
			Quaternion rotation = Quaternion.LookRotation(relativePos);
			float yAngle = Mathf.LerpAngle(oldRotation.eulerAngles.y, rotation.eulerAngles.y, t);
			//if(!akProperty.Tower)
			Attacker.transform.rotation = Quaternion.Euler(new Vector3(oldRotation.eulerAngles.x, yAngle, oldRotation.eulerAngles.z));
			float angle = Mathf.Abs(rotation.eulerAngles.y - yAngle);
			if(Mathf.RoundToInt(angle) == 360){
				angle -= 360.0f;
				angle = Mathf.Abs(angle);
			}
			if(angle<=1.0f){
				t = 0.0f;
				turningHead = false;
				//start animating attacking
				if(attackMode){
					
					if(attackerModel.GetComponent<AnimVault>()!=null){
						attackerModel.GetComponent<AnimVault>().CurrentState = AnimVault.AnimState.attack;
						if(attackerModel.GetComponent<AttackEvent>()!=null){
							attackerModel.GetComponent<AttackEvent>().SetTarget(Attacker, targetLocation, fightBackMode, critiqHit);
						}else if(attackerModel.GetComponent<ShowArrow>()!=null){
							attackerModel.GetComponent<ShowArrow>().SetTarget(Target,fightBackMode,critiqHit);
						}else if(attackerModel.GetComponent<ShowFlamingArrow>()!=null){
							attackerModel.GetComponent<ShowFlamingArrow>().SetTarget(Target,fightBackMode,critiqHit);
						}
						
				    }else{
						AttackCalFX aCal = transform.GetComponent<AttackCalFX>();
						if(fightBackMode){
							aCal.FightBack = false;
							aCal.CriticalHit = false;
							aCal.SetAttackSequence(Attacker, targetLocation);
						}else{
							aCal.FightBack = true;
							aCal.CriticalHit = critiqHit;
							aCal.SetAttackSequence(Attacker, targetLocation);
							chessUI.Critical = critiqHit;
							chessUI.DelayFadeOut = true;
							chessUI.TargetFadeIn = false;
						}
					}
				}
			}
		}
	}
	/*
	[RPC]
	void UpdateRotation(string chessName, Quaternion old, Quaternion rotation, float t){
		Transform chess = GameObject.Find(chessName).transform;
		chess.transform.rotation = Quaternion.Lerp(old, rotation, t);
	}*/
}
