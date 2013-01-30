using UnityEngine;
using System.Collections;

public class AnimVault : MonoBehaviour {
	public AnimationClip Idle, Run, Attack, Skill;
	public enum AnimState {idle, run, attack, skill};
	public AnimState CurrentState;
	AnimState oldState;
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
	
	// Update is called once per frame
	void Update () {
		if(oldState != CurrentState){
			switch(CurrentState){
				case AnimState.attack:
					animation.CrossFade("attack");
					break;
				case AnimState.idle:
					animation.CrossFade("idle");
					break;
				case AnimState.run:
					animation.CrossFade("running");
					break;
				case AnimState.skill:
					animation.CrossFade("skill");
					break;
			}
		}
		if(!animation.isPlaying){
			animation.CrossFade("idle");
			CurrentState = AnimState.idle;
		}
		oldState = CurrentState;
	}
}
