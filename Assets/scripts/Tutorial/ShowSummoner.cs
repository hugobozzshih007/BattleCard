using UnityEngine;
using System.Collections;

public class ShowSummoner : MonoBehaviour {
	public Transform SummonerA;  
	public Transform SummonerB;
	public Transform TowerA; 
	public Transform TowerB;
	Vector3 noWhere = new Vector3(0.0f,1000.0f,0.0f);  
	// Use this for initialization
	void Start () {
		SummonerA.transform.position = noWhere;
		SummonerB.transform.position = noWhere;
		TowerA.transform.position = noWhere;
		TowerB.transform.position = noWhere;
		Camera.mainCamera.GetComponent<RoundCounter>().SetPlayerChesses();
	}
	
	public void ShowSummonerA(){
		SummonerA.transform.position = GameObject.Find("unit_start_point_A").transform.position;
		SummonerA.transform.Translate(0.0f,1.5f,0.0f);
		SummonerB.GetChild(0).GetComponent<CharacterProperty>().death = true;
		CharacterProperty ap = SummonerA.GetChild(0).GetComponent<CharacterProperty>();
		ap.Activated = true;
		ap.Attacked = true;
		//Camera.mainCamera.GetComponent<MainUI>().InTutorial = true;
		transform.GetComponent<InitStage>().ShowSelCmd = true;
	}
	
	public void ShowSummonerB(){
	}
	
	public void ShowTower(){
		TowerA.position = GameObject.Find("red_tower").transform.position;
		TowerA.Translate(0.0f,4.0f,0.0f);
		TowerB.position = GameObject.Find("yellow_tower").transform.position;
		TowerB.Translate(0.0f,4.0f,0.0f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
