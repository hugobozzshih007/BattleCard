using UnityEngine;
using System.Collections;

public class FightBack : MonoBehaviour {
	AttackCalFX atkCal;
	TurnHead turningHead;
	Transform Attacker, Target;
	bool attackable = false;
	// Use this for initialization
	void Start () {
		atkCal = transform.GetComponent<AttackCalFX>();
		turningHead = transform.GetComponent<TurnHead>();
	}
	
	public void SetFightBack(Transform atk, Transform target){
		Attacker = atk;
		Target = target;
		attackable = atkCal.Attackable(atk, target);
		// init fight back
		if(attackable){
			turningHead.SetTurnHeadSequence(Attacker, target.GetComponent<CharacterSelect>().getMapPosition(),true,true,false);
			print("really fight back");
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
