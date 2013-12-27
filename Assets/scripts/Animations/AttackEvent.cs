using UnityEngine;
using System.Collections;

public class AttackEvent : MonoBehaviour {
	Transform Chess, Sel;
	bool fightBackMode = false;
	bool critiqHit;
	int playerSide;
	GeneralSelection currentSelect;
	MainInfoUI chessUI;
	//StatusMachine sMachine; 
	//CalculateTactics cTatic;
	
	void Start(){
		chessUI = Camera.mainCamera.GetComponent<MainInfoUI>();
		currentSelect = Camera.mainCamera.GetComponent<GeneralSelection>();
		//sMachine = GameObject.Find("StatusMachine").GetComponent<StatusMachine>();
		//cTatic = GameObject.Find("NpcPlayer").GetComponent<CalculateTactics>();
	}
	
	public void SetTarget(Transform chess, Transform sel, bool fightBack, bool critiq){
		Chess = chess;
		playerSide = chess.GetComponent<CharacterProperty>().Player;
		Sel = sel;
		fightBackMode = fightBack;
		critiqHit = critiq;
	}
	void ActivateAtk(){
		AttackCalFX atkCal = Camera.mainCamera.GetComponent<AttackCalFX>();
		if(fightBackMode){
			atkCal.fightBack = false;
			atkCal.CriticalHit = false;
			atkCal.SetAttackSequence(Chess,Sel);
		}else{
			atkCal.fightBack = true;
			atkCal.CriticalHit = critiqHit;
			atkCal.SetAttackSequence(Chess,Sel);
			if(currentSelect.npcMode && playerSide == 2)
				chessUI.CriticalRight = critiqHit;
			else
				chessUI.Critical = critiqHit;
			chessUI.DelayFadeOut = true;
			chessUI.TargetFadeIn = false;
		}
	}
}
