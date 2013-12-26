using UnityEngine;
using System.Collections;

public class StageSelection : MonoBehaviour {
	public int stageNum = 1;
	
	// Use this for initialization
	void Start () {
		stageNum = 1;
	}
	
	public void SetStage(int stage){
		stageNum = stage;
		Application.LoadLevel("summon_land_tutorials");
	}
	
	public int GetStage(){
		return stageNum;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void Awake(){
		if(Application.loadedLevelName == "tutorial_selection"){
			DontDestroyOnLoad(transform.gameObject);
		}
	}
}
