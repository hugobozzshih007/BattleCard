using UnityEngine;
using System.Collections;
using MapUtility; 
using System.Collections.Generic;

public class CharacterProperty : MonoBehaviour {
	public string NameString = "";
	public bool Summoner;
	public int Player = 1;
	public int moveRange = 1; 
	public int atkRange = 1;
	public int defPower = 1;
	public int Hp;
	public int atkPower = 1; 
	public int Damage;
	public int summonCost = 2;
	public int activeCost = 2;
	public int StandByRounds = 2;
	public int WaitRounds;
	public bool character = true;
	public bool death;
	public bool Ready;
	public bool TurnFinished = false;
	public bool Moved = false;
	public bool Attacked = false;
	public bool Activated = false;
	bool playerDead = false;
	public string activeAbility = "";
	public string passiveAbility = "";
	int guiSegX = 30;
	int guiSegY = 20;
	int guiSeg = 10;
	public Transform[] soldiers;
	
	//bool guiShow;
	//bool summonList;
	Vector3 noWhere = new Vector3(0.0f,1000.0f,0.0f);
	Vector3 screenPos;
	
	// Use this for initialization
	void Start () {
		Moved = false;
		death = true;
		Ready = false;
		Hp = defPower;
		Damage = atkPower;
		if(Summoner){
			Ready = true;
			death = false;
		}
		Attacked = false;
		Activated = false;
		if(Player>1)
			WaitRounds = StandByRounds-1;
		else
			WaitRounds = StandByRounds;
		//guiShow = false; 
		//summonList = false;
		if(Player==1){
			transform.position = GameObject.Find("unit_start_point_A").transform.position;
			transform.Translate(0.0f,1.5f,0.0f);
		}else if(Player==2){
			transform.position = GameObject.Find("unit_start_point_B").transform.position;
			transform.Translate(0.0f,1.5f,0.0f);
		}
	}
	
	// Update is called once per frame
	void Update () {
		screenPos = Camera.main.WorldToScreenPoint(transform.position);
		screenPos.y = Screen.height - screenPos.y;
		if(death){
			transform.position = noWhere;
			transform.renderer.enabled=false;
		}
		if(Moved && Attacked && Activated){
			TurnFinished = true;
		}else{
			TurnFinished = false;
		}
		
		if(WaitRounds==0){
			Ready = true;
		}else{
			Ready = false;
		}
		
		if(Hp<=0){
			if(!Summoner){
				death = true;
				WaitRounds = StandByRounds;
			}else{
				death = true;
				playerDead = true;
			}
		}
	}
	
	public IList GetAttackPosition(){
		IList targetLocations = new List<Transform>();
		Transform rootPos = transform.GetComponent<CharacterSelect>().getMapPosition();
		transform.GetComponent<CharacterSelect>().findAttackRange(rootPos,0,atkRange);
		IList attackableMaps = transform.GetComponent<CharacterSelect>().AttackRangeList;
	    Transform localMap = transform.GetComponent<CharacterSelect>().getMapPosition();
		if(attackableMaps.Contains(localMap)){
			attackableMaps.Remove(localMap);
		}
		print("attackableMaps: "+attackableMaps.Count);
		foreach(Transform map in attackableMaps){
			if(MapHelper.IsMapOccupied(map)){
				Transform occupiedObj = MapHelper.GetMapOccupiedObj(map);
				print(occupiedObj);
				if(occupiedObj!=null){
					if(occupiedObj.GetComponent<CharacterProperty>().Player!=transform.GetComponent<CharacterProperty>().Player){
						targetLocations.Add(map);
					}
				}
			}
		}
		foreach(Transform s in transform.GetComponent<CharacterSelect>().AttackRangeList){
			s.GetComponent<Identy>().step = 0;
		}
		
		transform.GetComponent<CharacterSelect>().AttackRangeList.Clear();
		return targetLocations;
	}
	
	public IList GetSummonPosition(){
		IList maps = new List<Transform>(); 
		CharacterSelect summoner = transform.GetComponent<CharacterSelect>();
		Transform occupiedMap = summoner.getMapPosition();
		Transform[] possibleUnitMap = occupiedMap.GetComponent<Identy>().neighbor;
		foreach(Transform unit in possibleUnitMap){
			Identy unitID = unit.GetComponent<Identy>();
			if(!MapHelper.IsMapOccupied(unit)&& !unitID.River && !unitID.Trees){
				maps.Add(unit);
			}
		}
		return maps;
	}
	
	void OnGUI(){
		GUI.skin.box.fontStyle = FontStyle.BoldAndItalic;
		GUI.skin.box.fontSize = 14;
		if(!death){
			GUI.Box(new Rect(screenPos.x-80,screenPos.y+40,150,30), NameString+" (HP:"+Hp+")");
		}
	}
	
	//summoning soldiers 
	/*
	public void SummonSoldier(Transform soldier){
		Vector3 summonPos = GetSummonPosition().position;
		summonPos.y = summonPos.y+1.5f;
		Transform gf = Instantiate(soldier,summonPos,Quaternion.identity) as Transform;
		RoundCounter counter = Camera.mainCamera.GetComponent<RoundCounter>();
		if(Player==1){
			counter.PlayerAChesses.Add(gf);
			print(counter.PlayerAChesses[1]);
		}else if(Player==2){
			counter.PlayerBChesses.Add(gf);
		}
	}*/
	
}
