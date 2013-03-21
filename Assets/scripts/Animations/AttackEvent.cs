using UnityEngine;
using System.Collections;

public class AttackEvent : MonoBehaviour {
	Transform Chess, Sel;
	bool fightBackMode = false;
	bool critiqHit;
	selection currentSelect;
	MainInfoUI chessUI;
	
	void Start(){
		chessUI = Camera.mainCamera.GetComponent<MainInfoUI>();
	}
	
	public void SetTarget(Transform chess, Transform sel, bool fightBack, bool critiq){
		Chess = chess;
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
			chessUI.Critical = critiqHit;
			chessUI.DelayFadeOut = true;
			chessUI.TargetFadeIn = false;
		}
	}
}
