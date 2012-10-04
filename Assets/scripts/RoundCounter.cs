using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoundCounter : MonoBehaviour {
	public int roundCounter = 0;
	public IList PlayerAChesses;
	public IList PlayerBChesses;
	Transform playerA;
	Transform playerB;
	Vector3 noWhere = new Vector3(0.0f,1000.0f,0.0f);
	
	bool CheckRound(IList chesses){
		bool phaseEnd = false;
		int x = 0;
		
		foreach(Transform gf in chesses){
			if(!gf.GetComponent<CharacterProperty>().death){
				if(!gf.GetComponent<CharacterProperty>().TurnFinished){
					phaseEnd = false;
					break;
				}else{
					phaseEnd = true;
				}
			}
		}
		return phaseEnd;
	}
	
	void updateRound(IList chesses){
		foreach(Transform gf in chesses){
			CharacterProperty gfp = gf.GetComponent<CharacterProperty>();
			gfp.Moved = false;
			gfp.Attacked = false;
			gfp.Activated = false;
			gfp.TurnFinished = false;
			if(!gfp.death){
				gfp.Hp = gfp.defPower;
			}
		}
	}
	
	void updateWaitRounds(){
		foreach(Transform gf in PlayerAChesses){
			if(!gf.GetComponent<CharacterProperty>().Ready){
				if(gf.GetComponent<CharacterProperty>().WaitRounds>0){
					gf.GetComponent<CharacterProperty>().WaitRounds-=1;
				}
			}
		}
		foreach(Transform gf in PlayerBChesses){
			if(!gf.GetComponent<CharacterProperty>().Ready){
				if(gf.GetComponent<CharacterProperty>().WaitRounds>0){
					gf.GetComponent<CharacterProperty>().WaitRounds-=1;
				}
			}	
		}
	}
	
	// Use this for initialization
	void Start () {
		roundCounter = 1;
		playerA = GameObject.Find("pSummonerA").transform;
		playerB = GameObject.Find("pSummonerB").transform;
		
		PlayerAChesses = new List<Transform>();
		PlayerBChesses = new List<Transform>();
		PlayerAChesses.Clear();
		PlayerBChesses.Clear();
		PlayerAChesses.Add(playerA);
		foreach(Transform gf in playerA.GetComponent<CharacterProperty>().soldiers){
			Transform gfClone = Instantiate(gf,noWhere,Quaternion.identity) as Transform;
			PlayerAChesses.Add(gfClone);
			gfClone.GetComponent<CharacterProperty>().death = true;
			gfClone.GetComponent<CharacterProperty>().Player = playerA.GetComponent<CharacterProperty>().Player;
		}
		PlayerBChesses.Add(playerB);
		foreach(Transform gf in playerB.GetComponent<CharacterProperty>().soldiers){
			Transform gfClone = Instantiate(gf,noWhere,Quaternion.identity) as Transform;
			PlayerBChesses.Add(gfClone);
			gfClone.GetComponent<CharacterProperty>().death = true;
			gfClone.GetComponent<CharacterProperty>().Player = playerB.GetComponent<CharacterProperty>().Player;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(roundCounter%2 == 1){
			if(CheckRound(PlayerAChesses)){
				roundCounter += 1;
				updateWaitRounds();
				updateRound(PlayerBChesses);
			}
		}else{
			if(CheckRound(PlayerBChesses)){
				roundCounter += 1;
				updateWaitRounds();
				updateRound(PlayerAChesses);
			}
		}
	}
	
	void OnGUI(){
		GUI.Box(new Rect(10,10,100,90), "Round: "+roundCounter);
	}
	
	void OnApplicationQuit(){
		foreach(Transform gf in playerA.GetComponent<CharacterProperty>().soldiers){
			gf.GetComponent<CharacterProperty>().death = true;
		}
		foreach(Transform gf in playerB.GetComponent<CharacterProperty>().soldiers){
			gf.GetComponent<CharacterProperty>().death = true;
		}
	}
}
