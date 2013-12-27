using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlacePrizes : MonoBehaviour {
	
	public int InitRed;
	public int InitYel;
	IList PrizeMaps = new List<Transform>();
	BuffInfoUI buffUI;
	GeneralSelection currentSel;
	RoundCounter currentRC; 
	// Use this for initialization
	void Start () {
		buffUI = Camera.mainCamera.GetComponent<BuffInfoUI>();
		currentSel = Camera.mainCamera.GetComponent<GeneralSelection>();
		currentRC = Camera.mainCamera.GetComponent<RoundCounter>();
	}
	
	public void PlacePrize(int redPrize, int yelPrize){
		IList unitMaps = new List<Transform>();
		IList numList = new List<int>();
		int[] reds = new int[redPrize]; 
		int[] yels = new int[yelPrize];
		Transform startA = GameObject.Find("unit_start_point_A").transform;
		Transform startB = GameObject.Find("unit_start_point_B").transform;
		
		unitMaps = currentRC.GetWhiteTerritory();
		
		foreach(Transform map in unitMaps){
			Identy mapID = map.GetComponent<Identy>();
			if(mapID.PrizeRed || mapID.PrizeYel)
				unitMaps.Remove(map);
		}
		
		for(int i=0; i<unitMaps.Count; i++){
			numList.Add(i);
		}
		
		for(int i=0; i<reds.Length; i++){
			int n = Random.Range(0, numList.Count);
			reds[i] = (int)numList[n];
			numList.Remove(numList[n]);
		}
		for(int i=0; i<yels.Length; i++){
			int n = Random.Range(0, numList.Count);
			yels[i] = (int)numList[n];
			numList.Remove(numList[n]);
		}
		foreach(int i in yels){
			Transform map = unitMaps[i] as Transform;
			Identy mapID = map.GetComponent<Identy>();
			mapID.PrizeYel = true;
		}
		foreach(int i in reds){
			Transform map = unitMaps[i] as Transform;
			Identy mapID = map.GetComponent<Identy>();
			mapID.PrizeRed = true;
		}
	}
	
	public void PlacePrize(int mode, Transform map){
		Identy mapID = map.GetComponent<Identy>();
		if(mode == 1){
			mapID.PrizeRed = true;
		}else if(mode == 2){
			mapID.PrizeYel = true;
		}
	}
	
	public void AddPrizeMap(Transform map){
		PrizeMaps.Add(map);
	}
	
	public void RemovePrizeMap(Transform map){
		PrizeMaps.Remove(map);
	}
	
	public IList GetPrizeMap(){
		return PrizeMaps;
	}
	
	void Awake(){
		
	}
	// Update is called once per frame
	void Update () {
	
	}

}
