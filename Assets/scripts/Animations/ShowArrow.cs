using UnityEngine;
using System.Collections;
using MapUtility;

public class ShowArrow : MonoBehaviour {
	public Rigidbody TrueArrow;
	public int ChildNumA, ChildNumB, ChildNumC;
	public string StartObjName; 
	public int ArrowNum;
	bool fightBackMode = false;
	bool critiqHit = false;
	Transform startCenter;
	Transform arrow;
	Transform target;
	Transform attacker;
	// Use this for initialization
	void Start () {
		arrow = transform.GetChild(ArrowNum);
		startCenter = MapHelper.FindAnyChildren(transform, StartObjName);
		if(startCenter==null){
			startCenter = transform.GetChild(ChildNumA).GetChild(ChildNumB).GetChild(ChildNumC);
		}
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
		bool skillAct = MapHelper.CheckAddedPassive(PassiveType.MultiArrow,attacker);
		Vector3 relativePos = target.position - attacker.position;
		Quaternion rotation = Quaternion.LookRotation(relativePos);
		Transform starting_center = startCenter;
		Rigidbody insArrow = Instantiate(TrueArrow,starting_center.position,rotation) as Rigidbody;
		//print("angle: " + transform.parent.rotation);
		insArrow.gameObject.layer = 16;
		firingArrow fa = insArrow.GetComponent<firingArrow>();
		fa.SetTarget(attacker, target,fightBackMode, critiqHit);
		if(skillAct){
			print("archer skill");
			Vector3 currentRotation = rotation.eulerAngles;
			Vector3 leftRotation = new Vector3(currentRotation.x,currentRotation.y-30.0f,currentRotation.z);
			Vector3 rightRotation = new Vector3(currentRotation.x,currentRotation.y+30.0f,currentRotation.z);
			Quaternion leftQuater = Quaternion.identity;
			leftQuater.eulerAngles = leftRotation;
			Quaternion rightQuater = Quaternion.identity;
			rightQuater.eulerAngles = rightRotation;
			Rigidbody leftArrow = Instantiate(TrueArrow,starting_center.position,leftQuater) as Rigidbody;
			leftArrow.gameObject.layer = 16;
			Rigidbody rightArrow = Instantiate(TrueArrow,starting_center.position,rightQuater) as Rigidbody;
			rightArrow.gameObject.layer = 16;
			
			Physics.IgnoreCollision(leftArrow.transform.collider, rightArrow.transform.collider);
			Physics.IgnoreCollision(leftArrow.transform.collider, insArrow.transform.collider);
			Physics.IgnoreCollision(insArrow.transform.collider, rightArrow.transform.collider);
			
			firingArrow leftFa = leftArrow.GetComponent<firingArrow>();
			leftFa.SetTarget(attacker, null, true, false);
			
			firingArrow rightFa = rightArrow.GetComponent<firingArrow>();
			rightFa.SetTarget(attacker, null, true, false);
			
			Destroy(leftArrow.gameObject,3.0f);
			Destroy(rightArrow.gameObject,3.0f);
			
			MapHelper.RemovePassive(PassiveType.MultiArrow, attacker);
		}
	}
	
	void XrossFadeToIdle(){
		animation.CrossFade("idle");
		AnimVault av = transform.GetComponent<AnimVault>();
		av.CurrentState = AnimVault.AnimState.idle;
	}
}
