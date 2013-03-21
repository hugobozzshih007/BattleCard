using UnityEngine;
using System.Collections;

public class EndStage : MonoBehaviour {
	Transform stageAward;
	Transform yellowTower; 
	Transform playerData;
	public string StageToGo;
	Rect endRect = new Rect(1105.0f,613.0f,122.0f,52.0f);
	public Texture2D nextBut;
	// Use this for initialization
	void Start () {
		Transform award = GameObject.Find("Award").transform;
		playerData = GameObject.Find("PlayerData").transform;
		yellowTower = GameObject.Find("yellow-tower").transform;
		stageAward = award.GetComponent<KeepAward>().EndAward; 
	}
	
	// Update is called once per frame
	void Update () {
		if(yellowTower.GetComponent<CharacterProperty>().death){
			playerData.GetComponent<GuardianStorage>().AddingAward(stageAward);
			playerData = GameObject.Find("PlayerData").transform;
			GuardianStorage guardians = playerData.GetComponent<GuardianStorage>();
			
			foreach(Transform gf in guardians.Guardians){
				print(gf);
			}
			
			Application.LoadLevel(StageToGo);
		}
	
	}
	
	void OnGUI(){
		if(GUI.Button(endRect, nextBut)){
			yellowTower.GetComponent<CharacterProperty>().death = true;
		}
	}
}
