using UnityEngine;
using System.Collections;
using MapUtility;

public class ShowFlamingArrow : MonoBehaviour {
	public Rigidbody NormalArrow;
	public Rigidbody FlameArrow;
	public string StartObjName; 
	public string ArrowName;
	bool fightBackMode = false;
	bool critiqHit = false;
	Transform startCenter;
	Transform arrow;
	Transform target;
	Transform attacker;
	Rigidbody insArrow;
	// Use this for initialization
	void Start () {
		arrow = MapHelper.FindAnyChildren(transform, ArrowName);
		startCenter = MapHelper.FindAnyChildren(transform, StartObjName);
		attacker = transform.parent;
	}
	
	public void SetTarget(Transform chess, bool fightBack, bool critiq){
		target = chess;
		fightBackMode = fightBack;
		critiqHit = critiq;
	}
	
	void RenderArrow(){
		arrow.renderer.enabled = true;
	}
	
	void UnRenderArrow(){
		arrow.renderer.enabled = false;
	}
	
	void LunchArrow(){
		bool skillAct = MapHelper.CheckAddedPassive(PassiveType.WoundBite,attacker);
		Vector3 relativePos = target.position - attacker.position;
		Quaternion rotation = Quaternion.LookRotation(relativePos);
		Transform starting_center = startCenter;
		if(skillAct){
			insArrow = Instantiate(FlameArrow,starting_center.position,rotation) as Rigidbody;
		}else{
			insArrow = Instantiate(NormalArrow,starting_center.position,rotation) as Rigidbody;
		}
		//print("angle: " + transform.parent.rotation);
		insArrow.gameObject.layer = 16;
		firingArrow fa = insArrow.GetComponent<firingArrow>();
		fa.SetTarget(attacker, target,fightBackMode, critiqHit);
		
	}
	
	void XrossFadeToIdle(){
		animation.CrossFade("idle");
		AnimVault av = transform.GetComponent<AnimVault>();
		av.CurrentState = AnimVault.AnimState.idle;
	}
}
