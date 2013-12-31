using UnityEngine;
using System.Collections;
using BuffUtility;

public class BuffInfoUI : MonoBehaviour {
	public Transform BuffContainer; 
	int allMaps;
	RoundCounter rc;
	MainInfoUI mainInfoUI;
	StatusMachine sMachine;

	//NGUI
	Transform hexagon, territory_pa, buffpa_num, debuffpa_num, buffx_num, debuffx_num;
	// Use this for initialization
	void Start () {
		allMaps = GameObject.Find("Maps").transform.childCount;
		rc = transform.GetComponent<RoundCounter>();
		mainInfoUI = transform.GetComponent<MainInfoUI>();
		sMachine = GameObject.Find("StatusMachine").transform.GetComponent<StatusMachine>();

		//NGUI
		hexagon = BuffContainer.FindChild("hexagon").transform;
		territory_pa = hexagon.GetChild(0);
		buffpa_num = BuffContainer.FindChild("buff_pa").transform.GetChild(0);
		debuffpa_num = BuffContainer.FindChild("debuff_pa").transform.GetChild(0);
		buffx_num = BuffContainer.FindChild("buff_x").transform.GetChild(0);
		debuffx_num = BuffContainer.FindChild("debuff_x").transform.GetChild(0);
	}
	
	public int GetTerritoryPersent(int side){
		int rate = 0;
		if(side == 1){
			rate = Mathf.RoundToInt((float)rc.PlayerATerritory.Count /(float)allMaps*100.0f);
		}else if(side == 2){
			rate = Mathf.RoundToInt((float)rc.PlayerBTerritory.Count /(float)allMaps*100.0f);
		}
		return rate;
	}

	//NGUI
	void UpdateBuffInfo(){
		int territoryPersent = 0;
		int redT =Mathf.RoundToInt((float)rc.PlayerATerritory.Count /(float)allMaps*100.0f);
		int yelT =Mathf.RoundToInt((float)rc.PlayerBTerritory.Count /(float)allMaps*100.0f);
		int buffX = 0;
		int buffRate = 0;
			

		if(mainInfoUI.PlayerSide == 1){
			hexagon.GetComponent<UISprite>().spriteName = "RedHaxagon";
			territoryPersent = redT;
		}else{
			territoryPersent = yelT;
			hexagon.GetComponent<UISprite>().spriteName = "YelHaxagon";
		}

		buffX = BuffCalculation.BuffXValue(territoryPersent);
		buffRate = BuffCalculation.BuffRateValue(territoryPersent);

		territory_pa.GetComponent<UILabel>().text = territoryPersent.ToString()+"%";
		buffx_num.GetComponent<UILabel>().text = "+" + buffX.ToString();
		debuffx_num.GetComponent<UILabel>().text = "-" + buffX.ToString();
		buffpa_num.GetComponent<UILabel>().text ="+"+buffRate.ToString();
		debuffpa_num.GetComponent<UILabel>().text ="-"+buffRate.ToString();
	}
	
	// Update is called once per frame
	void Update () {
		if(sMachine.InitGame)
			UpdateBuffInfo();
	}
}
