using UnityEngine;
using System.Collections;
using BuffUtility;

public class BuffInfoUI : MonoBehaviour {
	
	public Texture2D TerritoryRed, TerritoryYel;
	public Font Number;
	GUIStyle[] numberStyle = new GUIStyle[2];
	Rect territoryStart;
	int allMaps;
	InfoUI iconVault;
	RoundCounter rc;
	MainInfoUI mainInfoUI;
	
	// Use this for initialization
	void Start () {
		territoryStart = new Rect(Screen.width-350.0f, 6.0f, 28.0f, 28.0f);
		allMaps = GameObject.Find("Maps").transform.childCount;
		iconVault = transform.GetComponent<InfoUI>();
		rc = transform.GetComponent<RoundCounter>();
		mainInfoUI = transform.GetComponent<MainInfoUI>();
		
		numberStyle[0] = new GUIStyle();
		numberStyle[1] = new GUIStyle();
		numberStyle[0].font = Number;
		numberStyle[1].font = Number;
		numberStyle[0].normal.textColor = new Color(0.8f,0.8f,0.8f,1.0f);
		numberStyle[1].normal.textColor = new Color(0.8f,0.8f,0.8f,1.0f);
		numberStyle[0].fontSize = 24;
		numberStyle[1].fontSize = 20;
	}
	
	void DoBuffInfo(){
		Texture2D startTex = TerritoryRed;
		int territoryPersent = 0;
		int redT =Mathf.RoundToInt((float)rc.PlayerATerritory.Count /(float)allMaps*100.0f);
		int yelT =Mathf.RoundToInt((float)rc.PlayerBTerritory.Count /(float)allMaps*100.0f);
		int buffX = 0;
		int buffRate = 0;
		
		if(mainInfoUI.playerSide == 1){
			territoryPersent = redT;
			startTex = TerritoryRed;
		}else{
			territoryPersent = yelT;
			startTex = TerritoryYel;
		}
		
		buffX = BuffCalculation.BuffXValue(territoryPersent);
		buffRate = BuffCalculation.BuffRateValue(territoryPersent);
		
		GUI.DrawTexture(territoryStart,startTex);
		GUI.Label(new Rect(territoryStart.x+30.0f, territoryStart.y, 60.0f, 24.0f),territoryPersent.ToString()+"%",numberStyle[0]);
		GUI.DrawTexture(new Rect(territoryStart.x+90.0f,territoryStart.y+4, 20,20), iconVault.BuffInt[0]);
		GUI.Label(new Rect(territoryStart.x+112, territoryStart.y+4, 40,20), "+"+buffX.ToString(), numberStyle[1]);
		GUI.DrawTexture(new Rect(territoryStart.x+152,territoryStart.y+4, 20,20), iconVault.BuffInt[1]);
		GUI.Label(new Rect(territoryStart.x+174, territoryStart.y+4, 40,20), "-"+buffX.ToString(), numberStyle[1]);
		GUI.DrawTexture(new Rect(territoryStart.x+214,territoryStart.y+4, 20,20), iconVault.BuffRate[0]);
		GUI.Label(new Rect(territoryStart.x+236, territoryStart.y+4, 40,20), "+"+buffRate.ToString(), numberStyle[1]);
		GUI.DrawTexture(new Rect(territoryStart.x+276,territoryStart.y+4, 20,20), iconVault.BuffRate[1]);
		GUI.Label(new Rect(territoryStart.x+298, territoryStart.y+4, 40,20), "-"+buffRate.ToString(), numberStyle[1]);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI(){
		//Territory GUI 
		DoBuffInfo();
	}
}
