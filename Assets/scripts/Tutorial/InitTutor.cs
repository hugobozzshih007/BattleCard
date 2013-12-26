using UnityEngine;
using System.Collections;

public class InitTutor : MonoBehaviour {
	public Transform SummonerA, SummonerB; //TowerA, TowerB;
	selection currentSel; 
	public bool summonA = false; 
	public bool summonB = false; 
	public bool InitialA = false;  
	public bool InitialB = false;
	public bool Initial = true;
	public bool ZeroTurn = true;
	Transform redStartPos; 
	Transform yelStartPos;
	FollowCam fc; 
	NpcPlayer npc;
	// Use this for initialization
	void Start () {
		currentSel = Camera.main.GetComponent<selection>();
		fc = Camera.main.GetComponent<FollowCam>();
		redStartPos = GameObject.Find("unit_start_point_A").transform;
		yelStartPos = GameObject.Find("unit_start_point_B").transform;
		npc = GameObject.Find("NpcPlayer").transform.GetComponent<NpcPlayer>();
		//summonB = true;
		/*
		TowerA.transform.position = GameObject.Find("red_tower").transform.position;
		TowerA.transform.Translate(0.0f,4.0f,0.0f);
		TowerB.transform.position = GameObject.Find("yellow_tower").transform.position;
		TowerB.transform.Translate(0.0f,4.0f,0.0f);
		*/
	}
	
	void ReviveSummoner(Transform masterChess, Transform map){
		if(map!=null){
			currentSel.currentGF = masterChess;
			currentSel.SummonTrueCmd(masterChess, masterChess, map);
			npc.npcReviveMode = false;
			currentSel.reviveMode = false;
			fc.timeSeg = 0.0f;
			fc.CamFollowMe(masterChess);
		}
	}
	
	public void ResetSummoner(bool both){
		ReviveSummoner(SummonerA, redStartPos);
		if(both)
			ReviveSummoner(SummonerB, redStartPos);
	}
	
	// Update is called once per frame
	void Update () {
		if(summonA){
			ReviveSummoner(SummonerA, redStartPos);
			summonA = false;
		}
	}
}
