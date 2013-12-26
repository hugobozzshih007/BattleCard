using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProceduralControl : MonoBehaviour {
	
	public ProceduralMaterial[] PlayerSide; 
	public ProceduralMaterial[] ComSide; 
	
	public string Softness = "Softness"; 
	public string BlendingPos = "Blending_Position"; 
	
	RoundCounter currentRC;  
	int oldCountOfATerritory = 1; 
	int oldCountOfBTerritory = 1;
	
	StatusMachine sMachine;
	// Use this for initialization
	void Start () {
		currentRC = Camera.main.GetComponent<RoundCounter>();
		sMachine = GameObject.Find("StatusMachine").GetComponent<StatusMachine>();
		/*
		foreach(ProceduralMaterial pm in PlayerSide){
			pm.SetProceduralFloat(Softness, 0.0f);
			pm.SetProceduralFloat(BlendingPos, 0.0f);
		}
		foreach(ProceduralMaterial pm in ComSide){
			pm.SetProceduralFloat(Softness, 0.0f);
			pm.SetProceduralFloat(BlendingPos, 0.0f);
		}*/
	}
	
	public void FinalWinSet(){
		foreach(ProceduralMaterial pm in PlayerSide){
			pm.SetProceduralFloat(Softness, 1.0f);
			pm.SetProceduralFloat(BlendingPos, 0.2f);
			pm.RebuildTextures();
		}
	}
	
	bool CheckWinningA(){
		bool winning = false;
		int aNum = currentRC.PlayerATerritory.Count;  
		int bNum = currentRC.PlayerBTerritory.Count;
		if(aNum > bNum)
			winning = true;  
		else
			winning = false;
		return winning;
	}
	
	bool CheckUpdateTerritory(int side){
		if(side == 1){
			int aNum = currentRC.PlayerATerritory.Count;
			if(aNum == oldCountOfATerritory)
				return false;
			else{
				oldCountOfATerritory = aNum; 
				return true;
			}
		}else{
			int bNum = currentRC.PlayerBTerritory.Count;
			if(bNum == oldCountOfBTerritory)
				return false;  
			else{
				oldCountOfBTerritory = bNum;
				return true;
			}
		}
	}
	
	float GetBlendingValue(int mapNum){
		int mapTotal = GameObject.Find("Maps").GetComponent<NameMaps>().GetMapNum();
		float blending =(float)mapNum / (float)mapTotal;  
		return blending; 
	}
	
	// Update is called once per frame
	void Update () {
		if(CheckUpdateTerritory(1) && sMachine.InitGame){
			if(CheckWinningA()){
				float soft = Mathf.Abs(0.5f-GetBlendingValue(currentRC.PlayerATerritory.Count));
				float pos = GetBlendingValue(currentRC.PlayerATerritory.Count);
				foreach(ProceduralMaterial pm in PlayerSide){
					pm.SetProceduralFloat(Softness, soft);
					pm.SetProceduralFloat(BlendingPos, pos);
					pm.RebuildTextures();
				}
			}else{
				foreach(ProceduralMaterial pm in PlayerSide){
					pm.SetProceduralFloat(Softness, 0.0f);
					pm.SetProceduralFloat(BlendingPos, 0.0f);
					pm.RebuildTextures();
				}
			}
		}
	}
}
