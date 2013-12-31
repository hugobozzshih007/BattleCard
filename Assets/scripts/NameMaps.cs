using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NameMaps : MonoBehaviour {
	public Material mapOrigin, mapOrigin_alpha, mapTerritoryA, mapTerritoryB; 
	public bool ShowMap;
	IList allUnits = new List<Transform>();
	IList entireUnits = new List<Transform>();
	RoundCounter rc; 
	// Use this for initialization
	void Awake() {
		Transform allMap = GameObject.Find("Maps").transform;
		rc = Camera.mainCamera.GetComponent<RoundCounter>();
		if(ShowMap)
			mapOrigin.SetColor("_Color", new Color(1.0f,1.0f,1.0f,0.5f));
		else
			mapOrigin.SetColor("_Color", new Color(1.0f,1.0f,1.0f,0.0f));
		int ac = allMap.childCount;
		for(int i=0;i<ac;i++){
			if(allMap.GetChild(i).name == "unit"){
				allUnits.Add(allMap.GetChild(i));
			}
			entireUnits.Add(allMap.GetChild(i));
		}
		int gg = 0;
		foreach(Transform child in allUnits){
			child.name = child.name + gg;
			gg+=1;
		}
		
	}
	
	public int GetMapNum(){
		return transform.childCount;
	}
	
	public void ToggleGrid(bool hex){
		if(hex){
			foreach(Transform unit in entireUnits){
				Identity uID = unit.GetComponent<Identity>();
				
				if(rc.PlayerATerritory.Contains(unit)){
					unit.renderer.material = mapTerritoryA;
					uID.OriginalMat = mapTerritoryA;
				}else if(rc.PlayerBTerritory.Contains(unit)){
					unit.renderer.material = mapTerritoryB;
					uID.OriginalMat = mapTerritoryB;
				}else{
					unit.renderer.material = mapOrigin_alpha;
					uID.OriginalMat = mapOrigin_alpha;
				}
			}
		}else{
			foreach(Transform unit in entireUnits){
				Identity uID = unit.GetComponent<Identity>();
				unit.renderer.material = mapOrigin;
				uID.OriginalMat = mapOrigin;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	}
}
