using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class firingArrow : MonoBehaviour {
	public float force = 200.0f;
	public Transform HitFX;
	bool critiqHit = false;
	bool fightBackMode = false;
	
	bool excuted = true;
	Transform attacker, target;
	IList ignoreList;
	MainInfoUI chessUI;
	RoundCounter rc;
	
	public void SetTarget(Transform atk, Transform sel, bool fightBack, bool critiq){
		attacker = atk;
		ignoreList = new List<Transform>();
		rc = Camera.mainCamera.GetComponent<RoundCounter>();
		if(sel!=null){
			target = sel;
			foreach(Transform x in rc.AllChesses){
				if(x != target){
					ignoreList.Add(x);
				}
			}
		}else{
			if(attacker.GetComponent<CharacterProperty>().Player == 1){
				ignoreList = rc.PlayerAChesses;
			}else if(attacker.GetComponent<CharacterProperty>().Player == 2){
				ignoreList = rc.PlayerBChesses;
			}
		}
		fightBackMode = fightBack;
		critiqHit = critiq;
		excuted = false;
		
		foreach(Transform chess in ignoreList){
			Physics.IgnoreCollision(transform.collider, chess.collider);
		}
	}
	
	// Use this for initialization
	void Start () {
		
		chessUI = Camera.mainCamera.GetComponent<MainInfoUI>();
	}
	
	void Awake(){
		transform.rigidbody.AddForce(transform.forward * force);
	}
	
	void OnCollisionEnter(Collision collision){
		if(target==null){
			target = collision.transform;
			print(target.name);
		}
		if(!excuted){
			Vector3 cPoint = collision.transform.position;
			GameObject explode = Instantiate(HitFX, cPoint, Quaternion.identity) as GameObject;
			Destroy(explode, 2.0f);
			AttackCalFX aCal = Camera.mainCamera.GetComponent<AttackCalFX>();
			if(fightBackMode){
				aCal.fightBack = false;
				aCal.CriticalHit = false;
				aCal.SetAttackSequence(attacker, target.GetComponent<CharacterSelect>().getMapPosition());
			}else{
				aCal.fightBack = true;
				aCal.CriticalHit = critiqHit;
				aCal.SetAttackSequence(attacker, target.GetComponent<CharacterSelect>().getMapPosition());
				chessUI.Critical = critiqHit;
				chessUI.DelayFadeOut = true;
				chessUI.TargetFadeIn = false;
			}
			Destroy(transform.gameObject,0.1f);
			excuted = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		//transform.Translate(Vector3.forward *force *Time.deltaTime); 
	}
}
