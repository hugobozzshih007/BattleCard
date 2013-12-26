using UnityEngine;
using System.Collections;

public class StatusMachine : MonoBehaviour {
	public bool InBusy = false;
	public bool InGame = false;
	public bool GameEnd = false;
	public bool InitGame = false;
	public bool TutorialMode = false;
	public bool TutorialBusy = false;
	bool lastBusy = true;
	NpcPlayer npc; 
	selection currentSel; 
	// Use this for initialization
	void Start () {
		currentSel = Camera.mainCamera.GetComponent<selection>();
		if(Application.loadedLevelName == "summon_land_tutorials"){
			TutorialMode = true;
			//GameObject.Find("Tutorial").GetComponent<Tutorial>().InitTutorial();
		}
		npc = GameObject.Find("NpcPlayer").transform.GetComponent<NpcPlayer>();
	}
	
	// Update is called once per frame
	void Update () {
		if(!TutorialMode){
			if(!InBusy && InBusy!=lastBusy && currentSel.npcMode){
				npc.SetPause(2.0f);
				lastBusy = InBusy;
				//print("npc is resting......................................................................");
			}else if(InBusy && currentSel.npcMode)
				lastBusy = InBusy;
		}
	}
}