using UnityEngine;
using System.Collections;

public class AnimVault : MonoBehaviour {
	public AnimationClip Idle, Run, Attack, Skill;
	public enum AnimState {idle, run, attack, skill};
	public AnimState CurrentState;
	AnimState oldState;
	public float FadeTime = 0.3f;
	bool stateChanged = false;
	// Use this for initialization
	void Start () {
		CurrentState = AnimState.idle;
		animation.AddClip(Idle,"idle");
		animation.AddClip(Run,"running");
		animation.AddClip(Attack, "attack");
		animation.AddClip(Skill, "skill");
		animation.CrossFade("idle");
	}
	
	void IdleState(){
		CurrentState = AnimState.idle;
	}
	
	void SetBusy(){
		GameObject.Find("StatusMachine").GetComponent<StatusMachine>().InBusy = true;
	}
	
	void UnBusy(){
		GameObject.Find("StatusMachine").GetComponent<StatusMachine>().InBusy = false;
	}
	
	
	// Update is called once per frame
	void Update () {
		if(oldState != CurrentState){
			switch(CurrentState){
				case AnimState.attack:
					animation.CrossFade("attack", FadeTime);
					break;
				case AnimState.idle:
					animation.CrossFade("idle", FadeTime);
					break;
				case AnimState.run:
					animation.CrossFade("running", FadeTime);
					break;
				case AnimState.skill:
					animation.CrossFade("skill", FadeTime);
					break;
			}
		}
		if(!animation.isPlaying){
			animation.CrossFade("idle", FadeTime);
			CurrentState = AnimState.idle;
		}
		oldState = CurrentState;
	}
}
