using UnityEngine;
using System.Collections;
using MapUtility; 
using System.Collections.Generic;

public class CharacterProperty : MonoBehaviour {
	public string NameString = "";
	public RaceType Race;
	public Occupation Occupation;
	public PassiveType[] PassiveAbility;
	public bool Summoner;
	public int Player = 1;
	public int moveRange = 1; 
	public int atkRange = 1;
	public int atkPower = 1; 
	public int defPower = 1;
	public int CriticalhitChance = 18;
	public int SkillRate = 18;
	public int ModifiedDefPow;
	public int Damage;
	public int Hp;
	public int BuffAtkRange;
	public int BuffMoveRange;
	public int BuffCriticalHit;
	public int BuffSkillRate;
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
	public bool Tower = false;
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
		if(Player==1){
			Moved = false;
			Attacked = false;
			Activated = false;
		}else{
			Moved = true;
			Attacked = true;
			Activated = true;
		}
		//init initial status
		death = true;
		Ready = false;
		Hp = defPower;
		ModifiedDefPow = defPower;
		Damage = atkPower;
		BuffAtkRange = atkRange;
		BuffCriticalHit = CriticalhitChance;
		BuffMoveRange = moveRange;
		BuffSkillRate = SkillRate;
		
		if(Player>1)
			WaitRounds = StandByRounds-1;
		else
			WaitRounds = StandByRounds;
		
		if(Summoner){
			Ready = true;
			death = false;
			WaitRounds = 0;
		}
		
		if(!Tower){
			if(Player==1){
				transform.position = GameObject.Find("unit_start_point_A").transform.position;
				transform.Translate(0.0f,1.5f,0.0f);
			}else if(Player==2){
				transform.position = GameObject.Find("unit_start_point_B").transform.position;
				transform.Translate(0.0f,1.5f,0.0f);
			}
		}else{
			if(Player==1){
				transform.position = GameObject.Find("red_tower").transform.position;
				transform.Translate(0.0f,4.0f,0.0f);
			}else if(Player==2){
				transform.position = GameObject.Find("yellow_tower").transform.position;
				transform.Translate(0.0f,4.0f,0.0f);
			}
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
		if(TurnFinished && !Tower){
			if(Player==1){
				transform.renderer.material.SetColor("_Color",new Color(1.0f,155.0f/255.0f,155.0f/255.0f));
			}else if(Player==2){
				transform.renderer.material.SetColor("_Color",new Color(1.0f,245.0f/255.0f,90.0f/255.0f));
			}
		}else if(!TurnFinished && !Tower){
			transform.renderer.material.SetColor("_Color",new Color(1.0f,1.0f,1.0f));
		}
		
		if(WaitRounds==0){
			Ready = true;
		}else{
			Ready = false;
		}
	}
	
	public IList GetAttackPosition(){
		IList targetLocations = new List<Transform>();
		transform.GetComponent<CharacterSelect>().AttackRangeList.Clear();
		Transform rootPos = transform.GetComponent<CharacterSelect>().getMapPosition();
		transform.GetComponent<CharacterSelect>().findAttackRange(rootPos,0,BuffAtkRange);
		IList attackableMaps = transform.GetComponent<CharacterSelect>().AttackRangeList;
	    Transform localMap = transform.GetComponent<CharacterSelect>().getMapPosition();
		if(attackableMaps.Contains(localMap)){
			attackableMaps.Remove(localMap);
		}
		foreach(Transform map in attackableMaps){
			if(MapHelper.IsMapOccupied(map)){
				Transform occupiedObj = MapHelper.GetMapOccupiedObj(map);
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
	
	public bool criticalHit(){
		bool hit = false;
		int realNum = Random.Range(1,100);
		if(realNum<=CriticalhitChance){
			hit = true;
		}else{
			hit = false;
		}
		return hit;
	}
	
	void OnGUI(){
		GUI.skin.box.fontStyle = FontStyle.BoldAndItalic;
		GUI.skin.box.fontSize = 12;
		if(!death){
			GUI.Box(new Rect(screenPos.x-80,screenPos.y+40,150,30), NameString+" "+BuffMoveRange+"/"+BuffAtkRange+"/"+Damage+"/"+Hp);
		}else if(Summoner && death){
			Vector3 mapScreenPos = new Vector3(1.0f,1.0f);
			if(Player==1)
				mapScreenPos = Camera.main.WorldToScreenPoint(GameObject.Find("unit_start_point_A").transform.position);
			else if(Player==2)
				mapScreenPos = Camera.main.WorldToScreenPoint(GameObject.Find("unit_start_point_B").transform.position);
			mapScreenPos.y = Screen.height - mapScreenPos.y;
			
			GUI.Box(new Rect(mapScreenPos.x-80,mapScreenPos.y+40,150,30), "Revive in: "+WaitRounds+"turns");
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
