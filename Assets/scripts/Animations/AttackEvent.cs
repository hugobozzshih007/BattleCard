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
			atkCal.FightBack = false;
			atkCal.CriticalHit = false;
			atkCal.SetAttackSequence(Chess,Sel);
		}else{
			atkCal.FightBack = true;
			atkCal.CriticalHit = critiqHit;
			atkCal.SetAttackSequence(Chess,Sel);
			if(currentSelect.npcMode && playerSide == 2){
				//NGUI
				chessUI.ShowHitStatus(critiqHit, 2);
			}else{
				//NGUI
				chessUI.ShowHitStatus(critiqHit, 1);
			}
			//NGUI
			chessUI.DelayDeactivateInfoUI(2);
		}
	}
}
