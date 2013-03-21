using UnityEngine;
using System.Collections;

public class PlaceSummoner : MonoBehaviour {
	public Transform SummonerA, SummonerB, TowerA, TowerB;  
	// Use this for initialization
	void Start () {
		SummonerA.transform.position = GameObject.Find("unit_start_point_A").transform.position;
		SummonerA.transform.Translate(0.0f,1.5f,0.0f);
		SummonerB.transform.position = GameObject.Find("unit_start_point_B").transform.position;
		SummonerB.transform.Translate(0.0f,1.5f,0.0f);
		TowerA.transform.position = GameObject.Find("red_tower").transform.position;
		TowerA.transform.Translate(0.0f,4.0f,0.0f);
		TowerB.transform.position = GameObject.Find("yellow_tower").transform.position;
		TowerB.transform.Translate(0.0f,4.0f,0.0f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
