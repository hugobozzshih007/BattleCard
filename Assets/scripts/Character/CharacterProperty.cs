using UnityEngine;
using System.Collections;
using MapUtility; 
using System.Collections.Generic;

public class CharacterProperty : MonoBehaviour {
	public string NameString = "";
	public string BigIcon_Name = "";
	public Texture2D BigIcon;
	public Texture2D SmallIcon;

	public Transform DisplayModel;
	 
	public Transform QuickInfoRealUI;
	//store it's own HUDtext gameobject 
	public Transform HUDText;

	public bool LeadingCharacter; 
	public bool Summoner;
	public bool InSelection; 
	public int Player = 1;
	public int InitPlayer = 1;
	public int MoveRange = 1; 
	public int AtkRange = 1;
	public int AtkPower = 1; 
	public int DefPower = 1;
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
	public int StandByRounds = 2;
	public int WaitRounds;
	//public bool character = true;
	public bool UnStatus = false;
	public bool AbleDef = true;
	public bool Damaged = false;
	public Dictionary<UnnormalStatus, int> UnStatusCounter;
	public Dictionary<UnnormalStatus, int> LastUnStatusCounter;
	public bool Death;
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

	float bloodVolume = 0.0f;

	GeneralSelection currentSel;
	MainInfoUI infoUI;  
	// Use this for initialization
	void Start () {
		UnStatusCounter = new Dictionary<UnnormalStatus, int>();
		LastUnStatusCounter = new Dictionary<UnnormalStatus, int>();
		infoUI = Camera.main.GetComponent<MainInfoUI>();
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
		Death = true;
		Ready = false;
		Hp = MaxHp;
		ModifiedDefPow = DefPower;
		Damage = AtkPower;
		BuffAtkRange = AtkRange;
		BuffCriticalHit = CriticalhitChance;
		BuffMoveRange = MoveRange;
		BuffSkillRate = SkillRate;
		
		if(Player>1)
			WaitRounds = StandByRounds-1;
		else
			WaitRounds = StandByRounds;
		
		if(Summoner || LeadingCharacter){
			Ready = true;
			Death = false;
			WaitRounds = 0;
		}
		CmdTimes = 3; 
		currentSel = Camera.mainCamera.GetComponent<GeneralSelection>();
	
		if(!currentSel.NpcPlaying && Player==2){
			CmdTimes = 0;
		}

		oldHp = Hp+1;
		arrow = new Texture2D(10, 10);
		redSpot = (Texture2D)Resources.Load("redSpot");
		GenerateHUDText();
	}
	void Awake(){
		
	}
	// Update is called once per frame
	void Update () {
		screenPos = Camera.main.WorldToScreenPoint(transform.position);
		screenPos.y = Screen.height - screenPos.y;
		
		if(Death){
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
		bloodVolume = (float)Hp/(float)MaxHp;//*(bloodWidth-4.0f)
		if(QuickInfoRealUI){
			Transform trueBlood = MapHelper.FindAnyChildren(QuickInfoRealUI, "true_blood");
			trueBlood.GetComponent<UISprite>().fillAmount = bloodVolume;
		}

		UpdateActPoints();
		UpdateBasicBuffInfo();
	}

	//Instance HUDtext for this character  
	void GenerateHUDText(){
		GameObject uiText = NGUITools.AddChild(GameObject.Find("main_info_panel"), infoUI.HUDText.gameObject); 
		uiText.gameObject.layer = 8;
		uiText.GetComponent<UIFollowTarget>().target = transform; 
		uiText.GetComponent<UIFollowTarget>().gameCamera = Camera.main;
		uiText.GetComponent<UIFollowTarget>().uiCamera = GameObject.Find("UICamera").camera; 
		HUDText = uiText.transform;
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
			s.GetComponent<Identity>().step = 0;
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
			s.GetComponent<Identity>().step = 0;
		}
		transform.GetComponent<CharacterSelect>().AttackRangeList.Clear();
		return targetLocations;
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


	//for new NGUI system
	void UpdateBasicBuffInfo(){
		if(QuickInfoRealUI){
			GameObject atkBuffInfo = QuickInfoRealUI.GetChild(1).GetChild(0).gameObject;
			GameObject defBuffInfo = QuickInfoRealUI.GetChild(1).GetChild(1).gameObject;
			if(Damage>AtkPower){
				atkBuffInfo.SetActive(true); 
				atkBuffInfo.transform.GetChild(1).gameObject.SetActive(false);
				atkBuffInfo.transform.GetChild(0).gameObject.SetActive(true);
			}else if(Damage == AtkPower){
				atkBuffInfo.SetActive(false);
				atkBuffInfo.transform.GetChild(1).gameObject.SetActive(false);
				atkBuffInfo.transform.GetChild(0).gameObject.SetActive(false);
			}else{
				atkBuffInfo.SetActive(true);
				atkBuffInfo.transform.GetChild(1).gameObject.SetActive(true);
				atkBuffInfo.transform.GetChild(0).gameObject.SetActive(false);
			}

			if(ModifiedDefPow > DefPower){
				defBuffInfo.SetActive(true); 
				defBuffInfo.transform.GetChild(1).gameObject.SetActive(false);
				defBuffInfo.transform.GetChild(0).gameObject.SetActive(true);
			}else if(ModifiedDefPow == DefPower){
				defBuffInfo.SetActive(false); 
				defBuffInfo.transform.GetChild(1).gameObject.SetActive(false);
				defBuffInfo.transform.GetChild(0).gameObject.SetActive(false);
			}else{
				defBuffInfo.SetActive(true); 
				defBuffInfo.transform.GetChild(1).gameObject.SetActive(true);
				defBuffInfo.transform.GetChild(0).gameObject.SetActive(false);
			}
		}
	}

	//for character's HUD text info
	public void UpdateHudText(string content, Color textColor){
		if(HUDText){
			HUDText.GetComponent<HUDText>().Add(content, textColor,0.2f);
		}
	}

	//for new NGUI system
	void UpdateActPoints(){
		if(QuickInfoRealUI){
			GameObject[] actPoint = {QuickInfoRealUI.GetChild(0).GetChild(0).gameObject, QuickInfoRealUI.GetChild(0).GetChild(1).gameObject, QuickInfoRealUI.GetChild(0).GetChild(2).gameObject};
			if(guiShow){
				QuickInfoRealUI.gameObject.SetActive(true);
				if(!Tower && CheckInPlaying()){
					QuickInfoRealUI.GetChild(0).gameObject.SetActive(true);
					switch(CmdTimes){
						case 0:
							foreach(GameObject obj in actPoint){
								obj.SetActive(false);
							}
							break;
						case 1:
							actPoint[0].SetActive(false);
							actPoint[1].SetActive(false);
							actPoint[2].SetActive(true);
							if(!Attacked){
								actPoint[2].GetComponent<UISprite>().spriteName = "atk_turn";
							}else{
								actPoint[2].GetComponent<UISprite>().spriteName = "one_turn";
							}
							break;
						case 2:
							actPoint[0].SetActive(false);
							actPoint[1].SetActive(true);
							actPoint[2].SetActive(true);
							if(!Attacked){
								actPoint[1].GetComponent<UISprite>().spriteName = "atk_turn";
								actPoint[2].GetComponent<UISprite>().spriteName = "one_turn";
							}else{
								actPoint[1].GetComponent<UISprite>().spriteName = "one_turn";
								actPoint[2].GetComponent<UISprite>().spriteName = "one_turn";
							}
							break;
						case 3:
							actPoint[0].SetActive(true);
							actPoint[1].SetActive(true);
							actPoint[2].SetActive(true);
							
							actPoint[0].GetComponent<UISprite>().spriteName = "atk_turn";
							actPoint[1].GetComponent<UISprite>().spriteName = "one_turn";
							actPoint[2].GetComponent<UISprite>().spriteName = "one_turn";
							
							break;
						default:
							break;
					}
				}else{
					QuickInfoRealUI.GetChild(0).gameObject.SetActive(false);
				}
			}else{
				QuickInfoRealUI.gameObject.SetActive(false);
			}
		}
	}
}
