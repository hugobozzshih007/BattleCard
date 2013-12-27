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
	public bool InSelection; 
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
	public bool AbleDef = true;
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
	public int CmdTimes; 
	int oldHp; 
	
	public Transform[] soldiers;
	Texture2D arrow, redSpot;
	Color red = new Color(1.0f,155.0f/255.0f,155.0f/255.0f,1.0f);
	Color yellow = new Color(1.0f,245.0f/255.0f,90.0f/255.0f,1.0f);
	float t = 0.0f;
	float timeToDeath = 2.0f;
	bool guiShow;
	//bool summonList;
	Vector3 noWhere = new Vector3(0.0f,1000.0f,0.0f);
	Vector3 screenPos;
	Rect cmdRect, bloodRect, trueBloodRect, rectRedSpot;
	Rect turnRect; 
	Rect[] buffRect = new Rect[4];
	GUIStyle cStyle = new GUIStyle();
	float bloodVolume = 0.0f;
	Texture2D bloodLine, trueBloodLine, arrowUp, arrowDown, s_Atk, s_Def; 
	Texture2D turnsFull, turnsTwo, turnsOne, turnsEmpty;
	Texture2D currentTurn;
	float bloodWidth = 62.0f/1280.0f*Screen.width; 
	float bloodHeight = 8.0f/720.0f*Screen.height;
	float turnsWidth = 62.0f/1280.0f*Screen.width;
	float turnsHeight = 12.0f/720.0f*Screen.height;
	
	GeneralSelection currentSel;
	  
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
		cStyle.normal.textColor = Color.green;
		cStyle.fontSize = 20; 
		cStyle.alignment = TextAnchor.MiddleCenter;
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
		Hp = MaxHp;
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
		CmdTimes = 3; 
		currentSel = Camera.mainCamera.GetComponent<GeneralSelection>();
	
		if(!currentSel.NpcPlaying && Player==2){
			CmdTimes = 0;
		}
		
		bloodLine = (Texture2D)Resources.Load("bloodLine");
		trueBloodLine = GetBloodTexture(Player);
		turnsFull = (Texture2D)Resources.Load("turns_line_full");
		turnsEmpty = (Texture2D)Resources.Load("turns_line_empty");
		turnsOne = (Texture2D)Resources.Load("turns_line_1");
		turnsTwo = (Texture2D)Resources.Load("turns_line_2");
		arrowUp = (Texture2D)Resources.Load("arrow_up");
		arrowDown = (Texture2D)Resources.Load("arrow_down");
		s_Atk = (Texture2D)Resources.Load("small_attack");
		s_Def = (Texture2D)Resources.Load("small_def");
		oldHp = Hp+1;
		arrow = new Texture2D(10, 10);
		redSpot = (Texture2D)Resources.Load("redSpot");
	}
	void Awake(){
		
	}
	// Update is called once per frame
	void Update () {
		screenPos = Camera.main.WorldToScreenPoint(transform.position);
		screenPos.y = Screen.height - screenPos.y;
		
		if(death){
			transform.position = noWhere;
			Damaged = false;
			guiShow = false;
		}else{
			guiShow = true;
		}
		
		if(CmdTimes < 1){
			TurnFinished = true;
		}else{
			TurnFinished = false;
		}
		
		if(TurnFinished && !Tower && !currentSel.SummonIn){
			Color sideCol = Color.white;
			if(Player==1)
				sideCol = red;
			else
				sideCol = yellow;
			
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
		bloodVolume = (float)Hp/(float)MaxHp*(bloodWidth-4.0f);
	}
	
	Texture2D GetBloodTexture(int side){
		Texture2D blood= new Texture2D(8, 8);
		switch(side){
			case 1:
				blood = (Texture2D)Resources.Load("redBlood");
				break;
			case 2:
				blood = (Texture2D)Resources.Load("yelBlood");
				break;
		}
		return blood;
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
	
	public IList GetAttackPosition(Transform root){
		IList targetLocations = new List<Transform>();
		transform.GetComponent<CharacterSelect>().AttackRangeList.Clear();
		Transform rootPos = root;
		transform.GetComponent<CharacterSelect>().findAttackRange(rootPos,0,BuffAtkRange);
		IList attackableMaps = transform.GetComponent<CharacterSelect>().AttackRangeList;
		if(attackableMaps.Contains(root)){
			attackableMaps.Remove(root);
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
	
	void GetTurnTexture(int cmdTime){
		
		switch(cmdTime){
			case 1:
				currentTurn = turnsOne;
				break;
			case 2:
				currentTurn = turnsTwo;
				break;
			case 3:
				currentTurn = turnsFull;
				break;
			case 0:
				currentTurn = turnsEmpty;
				break;
		}
	}
	
	bool CheckInPlaying(){
		bool playing = false;
		
		if(currentSel.APlaying && Player==1){
			playing = true;
		}else if(currentSel.BPlaying && Player == 2){
			playing = true; 
		}else if(currentSel.NpcPlaying && Player == 2){
			playing = true;
		}
		
		return playing;
	}
	
	Texture2D GetTrueArrow(PowerType mode){
		if(mode == PowerType.Damage){
			if(Damage > atkPower)
				arrow = arrowUp;
			else if(Damage < atkPower)
				arrow = arrowDown;
		}else if(mode == PowerType.Defense){
			if(ModifiedDefPow > defPower)
				arrow = arrowUp;
			else if(ModifiedDefPow < defPower)
				arrow = arrowDown;
		}
		return arrow;
	}
	
	void DrawBasicBuffUI(){
		for(int i=0; i<4; i++){
			buffRect[i] = new Rect(bloodRect.x + 12*i, bloodRect.y - 12, 10, 10);
		}
		if(Damage!=atkPower && ModifiedDefPow!=defPower){
			GUI.DrawTexture(buffRect[0], s_Atk);
			GUI.DrawTexture(buffRect[1], GetTrueArrow(PowerType.Damage));
			GUI.DrawTexture(buffRect[2], s_Def);
			GUI.DrawTexture(buffRect[3], GetTrueArrow(PowerType.Defense));
		}else if(Damage!=atkPower && ModifiedDefPow==defPower){
			GUI.DrawTexture(buffRect[0], s_Atk);
			GUI.DrawTexture(buffRect[1], GetTrueArrow(PowerType.Damage));
		}else if(Damage==atkPower && ModifiedDefPow!=defPower){
			GUI.DrawTexture(buffRect[0], s_Def);
			GUI.DrawTexture(buffRect[1], GetTrueArrow(PowerType.Defense));
		}
		
	}
	
	void OnGUI(){
		GUI.backgroundColor = Color.clear;
		GUI.color = new Color(1.0f,1.0f,1.0f,0.75f);
		GUI.depth = 5; 
		if(guiShow ){
			if(CmdTimes<0)
				CmdTimes = 0;
			//cmdRect = new Rect(screenPos.x, screenPos.y+40, 20.0f,20.0f);
			if(!Tower && CheckInPlaying()){
				turnRect = new Rect(screenPos.x-28, screenPos.y+45+bloodHeight, turnsWidth, turnsHeight);
				rectRedSpot = new Rect(turnRect.x+turnsWidth+1, turnRect.y+turnsHeight/4, turnsHeight/2, turnsHeight/2);
				//if(!currentSel.CheckIfShowBlood()){
				if(!Attacked && !TurnFinished){
					GUI.DrawTexture(rectRedSpot, redSpot);
				}
				GetTurnTexture(CmdTimes);
				GUI.DrawTexture(turnRect, currentTurn);
			}
			
			bloodRect = new Rect(screenPos.x-28, screenPos.y+45, bloodWidth, bloodHeight);
			GUI.DrawTexture(bloodRect, bloodLine);
			trueBloodRect = new Rect(bloodRect.x+2, bloodRect.y+2,bloodVolume, bloodHeight-4); 
			GUI.DrawTexture(trueBloodRect, trueBloodLine);
			DrawBasicBuffUI();
		}
	}
}
