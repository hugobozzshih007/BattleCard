using UnityEngine;
using System.Collections;
using MapUtility; 
using System.Collections.Generic;

public class CharacterProperty : MonoBehaviour {
	public string NameString = "";
	public Texture2D BigIcon;
	public Texture2D SmallIcon;
	public Texture2D GuardianIcon;
	public Transform DisplayModel;
	public bool LeadingCharacter; 
	public bool Summoner;
	public int Player = 1;
	public int InitPlayer = 1;
	public int moveRange = 1; 
	public int atkRange = 1;
	public int atkPower = 1; 
	public int defPower = 1;
	public int CriticalhitChance = 18;
	public int SkillRate = 18;
	public int ModifiedDefPow;
	public int MaxHp;
	public int Damage;
	public int Hp;
	public int BuffAtkRange;
	public int BuffMoveRange;
	public int BuffCriticalHit = 18;
	public int BuffSkillRate = 18;
	public int summonCost = 2;
	public int StandByRounds = 2;
	public int WaitRounds;
	public bool character = true;
	public bool UnStatus = false;
	public bool AbleRestore = true;
	public bool Damaged = false;
	public Dictionary<UnnormalStatus, int> UnStatusCounter;
	public Dictionary<UnnormalStatus, int> LastUnStatusCounter;
	public bool death;
	public bool Ready;
	public bool TurnFinished = false;
	public bool Moved = false;
	public bool Attacked = false;
	public bool Activated = false;
	public bool Defensed = false;
	public bool Tower = false;
	

	public Transform[] soldiers;
	selection currentSel;
	DeathFX dFX;
	Color red = new Color(1.0f,155.0f/255.0f,155.0f/255.0f,1.0f);
	Color yellow = new Color(1.0f,245.0f/255.0f,90.0f/255.0f,1.0f);
	float t = 0.0f;
	float timeToDeath = 2.0f;
	//bool guiShow;
	//bool summonList;
	Vector3 noWhere = new Vector3(0.0f,1000.0f,0.0f);
	Vector3 screenPos;
	
	
	// Use this for initialization
	void Start () {
		UnStatusCounter = new Dictionary<UnnormalStatus, int>();
		LastUnStatusCounter = new Dictionary<UnnormalStatus, int>();
		//print("Dictionary length:" + UnStatusCounter.Count);
		UnStatusCounter.Add(UnnormalStatus.Burned,0);
		UnStatusCounter.Add(UnnormalStatus.Chaos,0);
		UnStatusCounter.Add(UnnormalStatus.Freezed,0);
		UnStatusCounter.Add(UnnormalStatus.Poisoned,0);
		UnStatusCounter.Add(UnnormalStatus.Sleeping,0);
		UnStatusCounter.Add(UnnormalStatus.Wounded,0);
		
		LastUnStatusCounter.Add(UnnormalStatus.Burned,0);
		LastUnStatusCounter.Add(UnnormalStatus.Chaos,0);
		LastUnStatusCounter.Add(UnnormalStatus.Freezed,0);
		LastUnStatusCounter.Add(UnnormalStatus.Poisoned,0);
		LastUnStatusCounter.Add(UnnormalStatus.Sleeping,0);
		LastUnStatusCounter.Add(UnnormalStatus.Wounded,0);
		//print("Dictionary length after adds:" + UnStatusCounter.Count);
		
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
		
		if(Summoner || LeadingCharacter){
			Ready = true;
			death = false;
			WaitRounds = 0;
		}
		/*
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
		}*/
		
		currentSel = Camera.mainCamera.GetComponent<selection>();
		dFX = Camera.mainCamera.GetComponent<DeathFX>();
		
	}
	void Awake(){
		
	}
	// Update is called once per frame
	void Update () {
		screenPos = Camera.main.WorldToScreenPoint(transform.position);
		screenPos.y = Screen.height - screenPos.y;
		
		if(death){
			transform.position = noWhere;
			transform.renderer.enabled=false;
			Damaged = false;
		}
		if(Moved && Attacked && Activated){
			TurnFinished = true;
		}else{
			TurnFinished = false;
		}
		
		if(TurnFinished && !Tower && !currentSel.SummonIn /*&& !dFX.StartDie*/){
			Color sideCol = Color.white;
			if(Player==1)
				sideCol = red;
			else
				sideCol = yellow;
			transform.renderer.material.color = sideCol;
			List<Transform> models = new List<Transform>();
			Transform model = transform.FindChild("Models");
			if(model.childCount>0){
				for(int i=0; i<model.childCount; i++){
					if(model.GetChild(i).GetComponent<SkinnedMeshRenderer>()!=null){
						models.Add(model.GetChild(i));
					}
				}
			}
			if(models.Count>0){
				foreach(Transform m in models){
					m.renderer.material.color = sideCol;
				}
			}
				
		}else if(!TurnFinished && !Tower && Hp>0){
			transform.renderer.material.color = Color.white;
			List<Transform> models = new List<Transform>();
			Transform model = transform.FindChild("Models");
			if(model.childCount>0){
				for(int i=0; i<model.childCount; i++){
					if(model.GetChild(i).GetComponent<SkinnedMeshRenderer>()!=null){
						models.Add(model.GetChild(i));
					}
				}
			}
			if(models.Count>0){
				foreach(Transform m in models){
					m.renderer.material.color = Color.white;
				}
			}
		}
		
		if(WaitRounds==0){
			Ready = true;
		}else{
			Ready = false;
		}
		if(Tower){
			Moved = true;
			Activated = true;
			Attacked = true;
			TurnFinished = true;
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
	/*
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
	
	/*
	public bool criticalHit(){
		bool hit = false;
		int realNum = Random.Range(1,100);
		if(realNum<=CriticalhitChance){
			hit = true;
		}else{
			hit = false;
		}
		return hit;
	}*/
}
