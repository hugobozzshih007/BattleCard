using UnityEngine;
using System.Collections;

public class FightBack : MonoBehaviour {
	AttackCalFX atkCal; 
	Transform Attacker, Target;
	bool attackable = false;
	// Use this for initialization
	void Start () {
		atkCal = transform.GetComponent<AttackCalFX>();
	}
	
	public void SetFightBack(Transform atk, Transform target){
		Attacker = atk;
		Target = target;
		attackable = atkCal.Attackable(Attacker,target);
		// init fight back
		if(attackable){
			atkCal.CriticalHit = false;
			atkCal.fightBack = false;
			if(Attacker.FindChild("Models").GetComponent<AnimVault>()!=null){
				Attacker.FindChild("Models").GetComponent<AnimVault>().CurrentState = AnimVault.AnimState.attack;
				Attacker.FindChild("Models").GetComponent<AttackEvent>().FightBackMode = true;
				Attacker.FindChild("Models").GetComponent<AttackEvent>().SetTarget(Attacker, Target.GetComponent<CharacterSelect>().getMapPosition());
			}else{
				atkCal.SetAttackSequence(Attacker,Target.GetComponent<CharacterSelect>().getMapPosition());
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
