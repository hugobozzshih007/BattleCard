using UnityEngine;
using System.Collections;

public class PlaceSummoner : MonoBehaviour {
	public Transform SummonerA, SummonerB; //TowerA, TowerB;
	selection currentSel; 
	public bool summonA = false; 
	public bool summonB = false;
	public bool TalkA = false;
	public bool TalkB = false;
	public bool InitialA = false;  
	public bool InitialB = false;
	public bool Initial = true;
	public bool ZeroTurn = true;
	Transform redStartPos; 
	Transform yelStartPos;
	FollowCam fc; 
	NpcPlayer npc;
	StatusMachine sMachine; 
	MainInfoUI chessUI; 
	bool[] pause = {false, false, false}; 
	float pauseTime = 1.0f; 
	float timeSeg = 0.0f; 
	// Use this for initialization
	void Start () {
		sMachine = GameObject.Find("StatusMachine").GetComponent<StatusMachine>();
		currentSel = Camera.main.GetComponent<selection>();
		fc = Camera.main.GetComponent<FollowCam>();
		redStartPos = GameObject.Find("unit_start_point_A").transform;
		yelStartPos = GameObject.Find("unit_start_point_B").transform;
		npc = GameObject.Find("NpcPlayer").transform.GetComponent<NpcPlayer>();
		chessUI = Camera.main.GetComponent<MainInfoUI>();
	}
	
	public void StartBattle(){
		pause[0] = true;
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
	
	public void ResetSummoner(bool both, Transform posA, Transform posB){
		if(posA == null)
			posA = redStartPos;
		if(posB == null)
			posB = yelStartPos;
		
		ReviveSummoner(SummonerA, posA);
		
		if(both)
			ReviveSummoner(SummonerB, posB);
	}
	
	
	// Update is called once per frame
	void Update () {
		
		if(pause[0]){
			timeSeg += Time.deltaTime/pauseTime;
			if(timeSeg >= 0.9f){
				timeSeg = 0.0f; 
				pause[0] = false;
				summonB = true;
				pauseTime = 2.0f;
			}
		}
		
		if(pause[1]){
			timeSeg += Time.deltaTime/pauseTime;
			if(timeSeg >= 0.9f){
				timeSeg = 0.0f; 
				pause[1] = false;
				summonA = true;
				TalkB = false;
			}
		}
		
		if(pause[2]){
			timeSeg += Time.deltaTime/pauseTime;
			if(timeSeg >= 0.9f){
				timeSeg = 0.0f; 
				pause[2] = false;
				TalkA = false;
				Initial = false;
			}
		}
		
		if(!sMachine.TutorialMode){
			if(summonA){
				ReviveSummoner(SummonerA, redStartPos);
				summonA = false;
				InitialA = true;
			}
			if(summonB){
				ReviveSummoner(SummonerB, yelStartPos);
				summonB = false;
				InitialB = true;
			}
			if(TalkB){
				chessUI.SomeoneTaking(SummonerB, SummonerB.GetComponent<TalkingContent>().AttackWords[1], true);
				pause[1] = true;
			}
			if(TalkA){
				chessUI.SomeoneTaking(SummonerA, SummonerA.GetComponent<TalkingContent>().AttackWords[1], false);
				pause[2] = true;
			}
		}
	}
}
