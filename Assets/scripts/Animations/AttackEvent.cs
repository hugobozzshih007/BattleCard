using UnityEngine;
using System.Collections;

public class AttackEvent : MonoBehaviour {
	public Transform Chess, Sel;
	public bool FightBackMode = false;
	selection currentSelect; 
	public void SetTarget(Transform chess, Transform sel){
		Chess = chess;
		Sel = sel;
	}
	void ActivateAtk(){
		if(FightBackMode){
			AttackCalFX atkCal = Camera.mainCamera.GetComponent<AttackCalFX>();
			atkCal.fightBack = false;
			atkCal.CriticalHit = false;
			atkCal.SetAttackSequence(Chess,Sel);
			FightBackMode = false;
		}else{
			currentSelect = Camera.mainCamera.GetComponent<selection>();
			currentSelect.AttackActivate(Chess, Sel);
		}
	}
}
