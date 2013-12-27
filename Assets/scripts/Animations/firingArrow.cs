using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;

public class firingArrow : MonoBehaviour {
	public float force = 200.0f;
	public PassiveType PassiveMode = PassiveType.FlyingHit;
	public Transform HitFX;
	public Transform ExtraHitFx; 
	public AudioClip HitSound, ExtraHitSound, ArrowFly;
	AudioClip hitClip;
	bool critiqHit = false;
	bool fightBackMode = false;
	int playerSide;
	bool excuted = true;
	bool gotPassive = false; 
	Transform attacker, target;
	Transform explosion;
	GeneralSelection currentSelect; 
	IList ignoreList;
	MainInfoUI chessUI;
	RoundCounter rc;
	//CalculateTactics cTactic; 
	
	public void SetTarget(Transform atk, Transform sel, bool fightBack, bool critiq){
		attacker = atk;
		ignoreList = new List<Transform>();
		rc = Camera.mainCamera.GetComponent<RoundCounter>();
		playerSide = atk.GetComponent<CharacterProperty>().Player;
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
		gotPassive = MapHelper.CheckAddedPassive(PassiveMode,attacker);
		if(gotPassive){
			explosion = ExtraHitFx;
			hitClip = ExtraHitSound;
		}else{
			explosion = HitFX;
			hitClip = HitSound;
		}
		foreach(Transform chess in ignoreList){
			Physics.IgnoreCollision(transform.collider, chess.collider);
		}
	}
	
	// Use this for initialization
	void Start () {
		//cTactic = GameObject.Find("NpcPlayer").GetComponent<CalculateTactics>();
		chessUI = Camera.mainCamera.GetComponent<MainInfoUI>();
		currentSelect = Camera.mainCamera.GetComponent<GeneralSelection>();
		hitClip = HitSound;
	}
	
	void Awake(){
		transform.rigidbody.AddForce(transform.forward * force);
		audio.clip = ArrowFly;
		if(audio.clip!=null)
			audio.Play();
	}
	
	void OnCollisionEnter(Collision collision){
		if(target==null){
			target = collision.transform;
			print(target.name);
		}
		if(!excuted){
			if(hitClip!=null){
				audio.clip = hitClip;
				audio.loop = false;
				audio.Play();
			}
			Vector3 cPoint = collision.transform.position;
			//play vfx
			GameObject explode = Instantiate(explosion, cPoint, Quaternion.identity) as GameObject;
			//play sound fx
			
			AttackCalFX aCal = Camera.mainCamera.GetComponent<AttackCalFX>();
			if(fightBackMode){
				aCal.FightBack = false;
				aCal.CriticalHit = false;
				aCal.SetAttackSequence(attacker, target.GetComponent<CharacterSelect>().getMapPosition());
				//GameObject.Find("StatusMachine").GetComponent<StatusMachine>().InBusy = false;
			}else{
				aCal.FightBack = true;
				aCal.CriticalHit = critiqHit;
				aCal.SetAttackSequence(attacker, target.GetComponent<CharacterSelect>().getMapPosition());
				if(currentSelect.npcMode && playerSide == 2)
					chessUI.CriticalRight = critiqHit;
				else
					chessUI.Critical = critiqHit;
				chessUI.DelayFadeOut = true;
				chessUI.TargetFadeIn = false;
			}
			
			Destroy(transform.gameObject,0.1f);
			excuted = true;
			MapHelper.RemovePassive(PassiveMode, attacker);
		}
	}
	
	// Update is called once per frame
	void Update () {
	}
}
