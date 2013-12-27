using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using MapUtility;
using BuffUtility;

public class RoundCounter : MonoBehaviour {
	bool npcMode;
	public int roundCounter = 0;
	public IList AllChesses;
	public IList PlayerAChesses;
	public IList PlayerBChesses;
	public IList PlayerATerritory;
	public IList PlayerBTerritory;
	public IList AllTerritory;
	public Transform RealPlayerA;
	public Transform RealPlayerB;
	public Transform playerA;
	public Transform playerB;
	public Texture2D InfoBlack;
	Vector3 noWhere = new Vector3(0.0f,1000.0f,0.0f);
	private bool camMoveMode = false;
	private Vector3 oldCamPosition;
	private Vector3 newCamPosition;
	//private float viewOffsetZ = 30.0f;
	public Transform MidObject;
	public Vector3 CamOffest = new Vector3();
	private int camStep = 0;
	public Material TerritoryA;
	public Material TerritoryB;
	public bool MoveCam;
	bool checkRound = false;
	RoundUI rUI;
	MainUI mUI;
	MainInfoUI infoUI;
	GeneralSelection currentSel;
	NpcPlayer npc; 
	StatusMachine sMachine;
	GuardianStorage guardians;
	public bool SummonerLand;
	PlacePrizes pp;
	PlaceSummoner pSummoner; 
	float timeSeg = 0.0f;
	// Use this for initialization
	void Awake () {
		sMachine = GameObject.Find("StatusMachine").GetComponent<StatusMachine>();
		pSummoner = GameObject.Find("InitStage").transform.GetComponent<PlaceSummoner>();
		Transform maps = GameObject.Find("Maps").transform;
		pp = maps.GetComponent<PlacePrizes>();
		GameObject playerData = GameObject.Find("PlayerData");
		if(playerData!=null)
			guardians = playerData.transform.GetComponent<GuardianStorage>();
		
		PlayerATerritory = new List<Transform>();
		PlayerBTerritory = new List<Transform>();
		AllTerritory = new List<Transform>();
		PlayerATerritory.Add(GameObject.Find("unit_start_point_A").transform);
		//PlayerATerritory.Add(GameObject.Find("red_tower").transform);
		if(!sMachine.TutorialMode)
			PlayerBTerritory.Add(GameObject.Find("unit_start_point_B").transform);
		//PlayerBTerritory.Add(GameObject.Find("yellow_tower").transform);
		
		rUI = transform.GetComponent<RoundUI>();
		mUI = transform.GetComponent<MainUI>();
		infoUI = transform.GetComponent<MainInfoUI>();
		currentSel = transform.GetComponent<GeneralSelection>();
		
		CamOffest = MidObject.position - transform.position;
		
		//print(CamOffest);
		npc = GameObject.Find("NpcPlayer").GetComponent<NpcPlayer>();
		
		
		roundCounter = 0;
		for(int i=0; i<maps.childCount; i++){
			AllTerritory.Add(maps.GetChild(i));
		}
		if(Network.connections.Length>0){
			npcMode = false;
		}else{
			npcMode = true;
		}
		
	}
	
	public IList GetWhiteTerritory(){
		IList allMap = new List<Transform>();
		foreach(Transform m in AllTerritory){
			allMap.Add(m);
		}
		foreach(Transform m in PlayerATerritory){
			if(allMap.Contains(m))
				allMap.Remove(m);
		}
		foreach(Transform m in PlayerBTerritory){
			if(allMap.Contains(m))
				allMap.Remove(m);
		}
		return allMap;
	}
	
	void CheckPrizes(){
		bool prizes = false;
		IList allMap = new List<Transform>();
		foreach(Transform m in AllTerritory){
			allMap.Add(m);
		}
		foreach(Transform m in allMap){
			Identy mID = m.GetComponent<Identy>();
			if(mID.PrizeRed || mID.PrizeYel){
				prizes = true;
				break;
			}else{
				prizes = false;
			}
		}
		if(!prizes){
			currentSel.PlacePrizes();
		}
	}
	//set initial environment start here
	public void SetPlayerChesses(){
		if(!SummonerLand)
			playerA = GameObject.Find("pSummonerA").transform.GetChild(0).transform;
		else 
			playerA = Instantiate(RealPlayerA, noWhere, Quaternion.identity) as Transform;
		if(!SummonerLand)
			playerB = GameObject.Find("pSummonerB").transform.GetChild(0).transform;
		else 
			playerB = Instantiate(RealPlayerB, noWhere, Quaternion.identity) as Transform;
		pSummoner.SummonerA = playerA;
		pSummoner.SummonerB = playerB;
		
		if(guardians!=null)
			guardians.SetTeamUp(playerA, playerB);
		PlayerAChesses = new List<Transform>();
		PlayerBChesses = new List<Transform>();
		AllChesses = new List<Transform>();
		PlayerAChesses.Add(playerA);
		foreach(Transform gf in playerA.GetComponent<CharacterProperty>().soldiers){
			if(gf!=null){
				Transform gfClone = Instantiate(gf,noWhere,Quaternion.identity) as Transform;
				gfClone.gameObject.layer = 10;
				PlayerAChesses.Add(gfClone);
				gfClone.GetComponent<CharacterProperty>().death = true;
				gfClone.GetComponent<CharacterProperty>().Player = 1;
				gfClone.GetComponent<CharacterProperty>().InitPlayer = 1;
			}
		}
		PlayerBChesses.Add(playerB);
		foreach(Transform gf in playerB.GetComponent<CharacterProperty>().soldiers){
			if(gf!=null){
				Transform gfClone = Instantiate(gf,noWhere,Quaternion.identity) as Transform;
				gfClone.gameObject.layer = 10;
				PlayerBChesses.Add(gfClone);
				gfClone.GetComponent<CharacterProperty>().death = true;
				gfClone.GetComponent<CharacterProperty>().Player = playerB.GetComponent<CharacterProperty>().Player;
				gfClone.GetComponent<CharacterProperty>().InitPlayer = playerB.GetComponent<CharacterProperty>().Player;
			}
		}
		foreach(Transform chess in PlayerAChesses){
			AllChesses.Add(chess);
		}
		foreach(Transform chess in PlayerBChesses){
			AllChesses.Add(chess);
		}
		
		if(!sMachine.TutorialMode)
			pSummoner.StartBattle();
		else{
			GameObject ss = GameObject.Find("StageSelection");
			int stage = 1;
			if(ss!=null)
				stage = ss.GetComponent<StageSelection>().GetStage();
			pSummoner.summonA = true;
			GameObject.Find("Tutorial").GetComponent<Tutorial>().InitTutorial(stage);
			if(ss!=null)
				DestroyObject(ss);
		}
		
		sMachine.InitGame = true;
		infoUI.InitCameras();
		GameObject playData = GameObject.Find("PlayerData");
		Destroy(playData,0.5f);
	}
	
	public void AddTerritory(Transform map, int player){
		if(player==1){
			PlayerATerritory.Add(map);
			if(!npcMode)
				networkView.RPC("RPCAddTerritory",RPCMode.Others,map.name,1);
		}else if(player==2){
			PlayerBTerritory.Add(map);
			if(!npcMode)
				networkView.RPC("RPCAddTerritory",RPCMode.Others,map.name,2);
		}
		
	}
	
	public void RemoveTerritory(Transform map, int player){
		if(player==1){
			PlayerATerritory.Remove(map);
			if(!npcMode)
				networkView.RPC("RPCRemoveTerritory",RPCMode.Others,map.name,1);
		}else if(player==2){
			PlayerBTerritory.Remove(map);
			if(!npcMode)
				networkView.RPC("RPCRemoveTerritory",RPCMode.Others,map.name,2);
		}
	}
	
	public int GetCountTerritory(int player){
		int count = 0;
		if(player==1)
			count = PlayerATerritory.Count;
		else if(player==2)
			count = PlayerBTerritory.Count;
		return count;
	}
	
	public bool IsInsideTerritory(Transform map, int player){
		bool inside = false;
		if(player==1)
			inside = PlayerATerritory.Contains(map);
		else if(player==2)
			inside = PlayerBTerritory.Contains(map);
		return inside;
	}
	
	public IList GetTerritory(int player){
		IList territory= new List<Transform>();
		if(player==1)
			territory = PlayerATerritory;
		else if(player==2)
			territory = PlayerBTerritory;
		return territory;
	}
	
	/*
	public int GetIntensifiedPower(Transform character, string mode){
		CharacterProperty property = character.GetComponent<CharacterProperty>();
		CharacterSelect selection = character.GetComponent<CharacterSelect>();
		Transform location = selection.getMapPosition();
		int playerSide = property.Player;
		float percentage = 1.0f;
		Transform allMap = GameObject.Find("Maps").transform;
		int mapUnitNum = allMap.GetChildCount();
		int locationSide = 1;
		if(PlayerATerritory.Contains(location))
			locationSide = 1;
		else if(PlayerBTerritory.Contains(location))
			locationSide = 2;
		else
			locationSide = 0;
		
		if(playerSide == 1){
			switch(locationSide){
				case 0:
					percentage = 0.0f;
					break;
				case 1:
					switch(mode){
						case "atk":
							percentage = ((float)PlayerATerritory.Count/mapUnitNum)*2*Mathf.Pow((float)property.atkPower,0.1f);
							break;
						case "def":
							percentage = ((float)PlayerATerritory.Count/mapUnitNum)*2*Mathf.Pow((float)property.defPower,0.1f);
							break;
						case "mov":
							percentage = ((float)PlayerATerritory.Count/mapUnitNum)*2*Mathf.Pow((float)property.moveRange,0.1f);
							break;
						case "atr":
							percentage = ((float)PlayerATerritory.Count/mapUnitNum)*2*Mathf.Pow((float)property.atkRange,0.1f);
							break;
						case "cri":
							percentage = Mathf.Pow((float)PlayerATerritory.Count/mapUnitNum*10.0f,0.2f)*(float)property.CriticalhitChance;
							break;
					}
					break;
				case 2:
					switch(mode){
						case "atk":
							percentage = -((float)PlayerATerritory.Count/mapUnitNum)*2*Mathf.Pow((float)property.atkPower,0.1f);
							break;
						case "def":
							percentage = -((float)PlayerATerritory.Count/mapUnitNum)*2*Mathf.Pow((float)property.defPower,0.1f);
							break;
						case "mov":
							percentage = -((float)PlayerATerritory.Count/mapUnitNum)*2*Mathf.Pow((float)property.moveRange,0.1f);
							break;
						case "atr":
							percentage = -((float)PlayerATerritory.Count/mapUnitNum)*2*Mathf.Pow((float)property.atkRange,0.1f);
							break;
						case "cri":
							percentage = -Mathf.Pow((float)PlayerATerritory.Count/mapUnitNum*10.0f,0.2f)*(float)property.CriticalhitChance;
							break;
					}
					break;
			}
		}else if(playerSide==2){
			switch(locationSide){
				case 0:
					percentage = 0.0f;
					break;
				case 1:
					switch(mode){
						case "atk":
							percentage = -((float)PlayerBTerritory.Count/mapUnitNum)*2*Mathf.Pow((float)property.atkPower,0.1f);
							break;
						case "def":
							percentage = -((float)PlayerBTerritory.Count/mapUnitNum)*2*Mathf.Pow((float)property.defPower,0.1f);
							break;
						case "mov":
							percentage = -((float)PlayerBTerritory.Count/mapUnitNum)*2*Mathf.Pow((float)property.moveRange,0.1f);
							break;
						case "atr":
							percentage = -((float)PlayerBTerritory.Count/mapUnitNum)*2*Mathf.Pow((float)property.atkRange,0.1f);
							break;
						case "cri":
							percentage = -Mathf.Pow((float)PlayerBTerritory.Count/mapUnitNum*10.0f,0.2f)*(float)property.CriticalhitChance;
							break;
					}
					break;
				case 2:
					switch(mode){
						case "atk":
							percentage = ((float)PlayerBTerritory.Count/mapUnitNum)*2*Mathf.Pow((float)property.atkPower,0.1f);
							break;
						case "def":
							percentage = ((float)PlayerBTerritory.Count/mapUnitNum)*2*Mathf.Pow((float)property.defPower,0.1f);
							break;
						case "mov":
							percentage = ((float)PlayerBTerritory.Count/mapUnitNum)*2*Mathf.Pow((float)property.moveRange,0.1f);
							break;
						case "atr":
							percentage = ((float)PlayerBTerritory.Count/mapUnitNum)*2*Mathf.Pow((float)property.atkRange,0.1f);
							break;
						case "cri":
							percentage = Mathf.Pow((float)PlayerBTerritory.Count/mapUnitNum*10.0f,0.2f)*(float)property.CriticalhitChance;
							break;
					}
					break;
			}
		}
		
		int addedVlaue = (int)Mathf.Round(percentage);
		//print("Player_"+playerSide+" gets: "+addedVlaue);
		return addedVlaue;
	}*/
	
	void translateMainCam(float timeToReach){
		timeSeg+= Time.deltaTime/timeToReach;
		Vector3 newPos = Vector3.Lerp(oldCamPosition, newCamPosition, timeSeg);
		transform.position = newPos;
		float d = Vector3.Distance(transform.position, newCamPosition);
		if(d<0.03f){
			timeSeg = 0.0f;
			camMoveMode = false;
			if(currentSel.reviveMode){
				//currentSel.reviveMode = false;
				npc.npcReviveMode = false;
			}
		}
	}
	
	bool CheckRound(IList chesses){
		bool phaseEnd = false;
		foreach(Transform gf in chesses){
			if(!gf.GetComponent<CharacterProperty>().death){
				if(!gf.GetComponent<CharacterProperty>().TurnFinished){
					phaseEnd = false;
					break;
				}else{
					phaseEnd = true;
				}
			}
		}

		return phaseEnd;
	}
	
	bool checkAllDeath(IList chesses){
		bool allDeath = false;
		foreach(Transform gf in chesses){
			if(!gf.GetComponent<CharacterProperty>().death){
				allDeath = false;
				break;
			}else{
				allDeath = true;
			}
		}
		return allDeath;
	}
	
	void updateRound(IList chesses){
		foreach(Transform gf in chesses){
			CharacterProperty gfp = gf.GetComponent<CharacterProperty>();
			BuffList gfB = gf.GetComponent<BuffList>();
			gfp.Moved = false;
			gfp.Attacked = false;
			gfp.Activated = false;
			gfp.TurnFinished = false;
			gfp.Damaged = false;
			gfp.Defensed = false;
			gfp.CmdTimes = 3;
			//if(!gfp.death && gfp.AbleRestore){
				//gfp.Hp = gfp.ModifiedDefPow;
			//}
		}
	}
	
	void updateUnnormalStatus(){
		foreach(Transform chess in AllChesses){
			CharacterProperty cProperty = chess.GetComponent<CharacterProperty>();
			cProperty.UnStatus = false;
			List<UnnormalStatus> keys = new List<UnnormalStatus>(cProperty.UnStatusCounter.Keys);
			if(!cProperty.death){
				foreach(UnnormalStatus key in keys){
					if(cProperty.UnStatusCounter[key] > 0){
						cProperty.UnStatus = true;
						switch(key){
							case UnnormalStatus.Burned:
								break;
							case UnnormalStatus.Chaos:
								cProperty.Moved = true;
								cProperty.Activated = true;
								cProperty.Attacked = true;
								cProperty.TurnFinished = true;
								break;
							case UnnormalStatus.Freezed:
								cProperty.Moved = true;
								break;
							case UnnormalStatus.Poisoned:
								cProperty.Hp -= 1;
								break;
							case UnnormalStatus.Sleeping:
								break;
							case UnnormalStatus.Wounded:
								cProperty.AbleDef = false;
								cProperty.ModifiedDefPow = 0;
								break;
						}
						cProperty.UnStatusCounter[key] -= 1;
						
					}else{
						switch(key){
							case UnnormalStatus.Burned:
								break;
							case UnnormalStatus.Chaos:
								break;
							case UnnormalStatus.Freezed:
								break;
							case UnnormalStatus.Poisoned:
								break;
							case UnnormalStatus.Sleeping:
								break;
							case UnnormalStatus.Wounded:
								cProperty.AbleDef = true;
								break;
						}
					}
					if(cProperty.LastUnStatusCounter[key] > 0){
						cProperty.LastUnStatusCounter[key] -= 1;
					}
				}
			}else{
				foreach(UnnormalStatus key in keys){
					cProperty.UnStatusCounter[key] = 0;
				}
			}
		}
	}
	
	void updateExtraBuff(IList chesses){
		foreach(Transform gf in chesses){
			if(!gf.GetComponent<CharacterProperty>().death){
				//foreach(BuffType Buff in Enum.GetValues(typeof(BuffType))){
					if(MapHelper.CheckPassive(PassiveType.DefenseAddOne, gf))
						gf.GetComponent<BuffList>().ExtraDict[BuffType.Defense] = 0;
				//}
			}
		}
	}
	/*
	void updateASideMaxHp(){
		foreach(Transform gf in PlayerAChesses){
			if(!gf.GetComponent<CharacterProperty>().death){
				gf.GetComponent<CharacterProperty>().MaxHp = gf.GetComponent<CharacterProperty>().Hp;
			}
		}
	}
	
	void updateBSideMaxHp(){
		foreach(Transform gf in PlayerBChesses){
			if(!gf.GetComponent<CharacterProperty>().death){
				gf.GetComponent<CharacterProperty>().MaxHp = gf.GetComponent<CharacterProperty>().Hp;
			}
		}
	}
	*/
	
	void updateWaitRounds(){
		foreach(Transform gf in PlayerAChesses){
			if(!gf.GetComponent<CharacterProperty>().Ready){
				if(gf.GetComponent<CharacterProperty>().WaitRounds>0){
					gf.GetComponent<CharacterProperty>().WaitRounds-=1;
				}
			}
		}
		foreach(Transform gf in PlayerBChesses){
			if(!gf.GetComponent<CharacterProperty>().Ready){
				if(gf.GetComponent<CharacterProperty>().WaitRounds>0){
					gf.GetComponent<CharacterProperty>().WaitRounds-=1;
				}
			}	
		}
	}
	
	void updateSkillCDRounds(){
		foreach(Transform gf in AllChesses){
			if(!gf.GetComponent<CharacterProperty>().death){
				Transform[] skills = gf.GetComponent<SkillSets>().Skills;
				foreach(Transform skill in skills){
					SkillProperty gfSp = skill.GetComponent<SkillProperty>();
					if(!gfSp.SkillReady){
						gfSp.WaitingRounds -= 1;
					}
				}
			}
		}
	}
	
	
	void updatePassive(IList chesses){
		foreach(Transform chess in chesses){
			if(!chess.GetComponent<CharacterProperty>().death){
				CharacterPassive chessPassive = chess.GetComponent<CharacterPassive>();
				foreach(PassiveType passive in Enum.GetValues(typeof(PassiveType))){
					if(passive == PassiveType.Flying)
						chessPassive.PassiveDict[passive] = false;
				}
				if((chessPassive.PassiveAbility!=null) && (chessPassive.PassiveAbility.Length>0)){
					foreach(PassiveType p in chessPassive.PassiveAbility){
						chessPassive.PassiveDict[p] = true;
					}
				}
			}
		}
	}
	
	void getNewCamPos(int player){
		Transform liveOne = null;
		if(player ==1){
			foreach(Transform chess in PlayerAChesses){
				if(!chess.GetComponent<CharacterProperty>().death){
					liveOne = chess;
					break;
				}
			}
		}else if(player ==2){
			if(npc.npcReviveMode){
				liveOne = playerB;
			}else{
				foreach(Transform chess in PlayerBChesses){
					if(!chess.GetComponent<CharacterProperty>().death){
						liveOne = chess;
						break;
					}
				}
			}
		}
		if(liveOne != null)
			newCamPosition = liveOne.position - CamOffest;
	}
	
	void revivePlayer(Transform player){
		CharacterProperty property = player.GetComponent<CharacterProperty>();
		if(property.death && property.Ready){
			if(npcMode && currentSel.player == playerB){
				npc.ReviveSummoner(player);
			}else
				currentSel.ReviveCommand(player);
		}
	}
	
	void updateAllCharactersPowers(){
		foreach(Transform character in AllChesses){
			BuffCalculation buffCal = new BuffCalculation(character);
			buffCal.UpdateBuffValue();
		}
	}
	
	void UpdateChessList(){
		PlayerAChesses.Clear();
		PlayerBChesses.Clear();
		foreach(Transform chess in AllChesses){
			if(chess.GetComponent<CharacterProperty>().InitPlayer == 1){
				PlayerAChesses.Add(chess);
				chess.GetComponent<CharacterProperty>().Player = 1;
			}else if(chess.GetComponent<CharacterProperty>().InitPlayer == 2){
				PlayerBChesses.Add(chess);
				chess.GetComponent<CharacterProperty>().Player = 2;
			}
		}
	}
	
	void updatePlaying(){
		GeneralSelection currentSel = transform.GetComponent<GeneralSelection>();
		if(!npcMode){
			if(Network.peerType == NetworkPeerType.Client){
				currentSel.Playing = currentSel.BPlaying;
			}else if(Network.peerType == NetworkPeerType.Server){
				currentSel.Playing = currentSel.APlaying;
			}
		}else{
			if(roundCounter%2 == 1){
				currentSel.Playing = false;
				currentSel.NpcPlaying = true;
				currentSel.player = playerB;
			}else{
				currentSel.Playing = true;
				currentSel.NpcPlaying = false;
				currentSel.player = playerA;
			}
		}
	}
	
	void UpdateNPCplayerList(){
		if(Network.connections.Length == 0){
			Transform npcPlayer = GameObject.Find("NpcPlayer").transform;
			NpcPlayer npc = npcPlayer.GetComponent<NpcPlayer>();
			npc.InitNPCTurn();
		}
	}
	
	void UpdateLayers(){
		foreach(Transform t in AllChesses){
			t.gameObject.layer = 10;
		}
		infoUI.MainFadeIn = false;
		infoUI.TargetFadeIn = false;
	}
	// Update is called once per frame
	void Update () {
		if(!sMachine.InBusy && sMachine.InitGame && !sMachine.TutorialMode){
			if(roundCounter%2 == 1 && !pSummoner.Initial && !pSummoner.ZeroTurn){
				if(CheckRound(PlayerAChesses)|| checkAllDeath(PlayerAChesses)){
					if(!checkRound && !currentSel.reviveMode){
						rUI.SetRoundUI(Color.yellow);
						checkRound = true;
					}
				}
				if(rUI.UIfinished){
					UpdateLayers();
					updateWaitRounds();
					oldCamPosition = transform.position;
					
					updateUnnormalStatus();
					updateExtraBuff(PlayerBChesses);
					updatePassive(PlayerBChesses);
					//updateASideMaxHp();
					UpdateChessList();
					updateRound(PlayerBChesses);
					updateAllCharactersPowers();
					updateSkillCDRounds();
					transform.GetComponent<GeneralSelection>().APlaying=false;
					transform.GetComponent<GeneralSelection>().BPlaying=true;
					updatePlaying();
					UpdateNPCplayerList();
					revivePlayer(playerB);
					getNewCamPos(2);
					camMoveMode = true;
					checkRound = false;
					rUI.UIfinished = false;
					if(!sMachine.TutorialMode){
						CheckPrizes();
						roundCounter += 1;
					}
				}
			} 
			if(roundCounter%2 == 0 && !pSummoner.Initial){
				if(CheckRound(PlayerBChesses)|| checkAllDeath(PlayerBChesses)){
					if(!checkRound && !currentSel.reviveMode){
						rUI.SetRoundUI(Color.red);
						checkRound = true;
					}
				}
				if(rUI.UIfinished){
					UpdateLayers();
					updateWaitRounds();
					oldCamPosition = transform.position;
					getNewCamPos(1);
					camMoveMode = true;
					
					updateUnnormalStatus();
					updateExtraBuff(PlayerAChesses);
					updatePassive(PlayerAChesses);
					//updateBSideMaxHp();
					UpdateChessList();
					updateRound(PlayerAChesses);
					updateAllCharactersPowers();
					updateSkillCDRounds();
					transform.GetComponent<GeneralSelection>().APlaying=true;
					transform.GetComponent<GeneralSelection>().BPlaying=false;
					updatePlaying();
					revivePlayer(playerA);
					roundCounter += 1;
					checkRound = false;
					rUI.UIfinished = false;
					if(!sMachine.TutorialMode){
						CheckPrizes();
						
					}
					pSummoner.ZeroTurn = false;
				}
			}
		}
		if(camMoveMode && MoveCam){
			translateMainCam(0.4f);
		}
	}
	
	void OnGUI(){
		//GUI.backgroundColor = Color.clear;
		//GUI.Box(new Rect(10,10,100,90), "Round: "+roundCounter);
		//Head Bar
		GUI.depth = 5;
		GUI.DrawTexture(new Rect(0,0,Screen.width,40),InfoBlack);
	}
	
	void OnApplicationQuit(){
		foreach(Transform gf in playerA.GetComponent<CharacterProperty>().soldiers){
			if(gf!=null)
				gf.GetComponent<CharacterProperty>().death = true;
		}
		foreach(Transform gf in playerB.GetComponent<CharacterProperty>().soldiers){
			if(gf!=null)
				gf.GetComponent<CharacterProperty>().death = true;
		}
	}
	
	[RPC]
	void RPCAddTerritory(string tName, int side){
		Transform map = GameObject.Find(tName).transform;
		if(side == 1)
			PlayerATerritory.Add(map);
		else if(side == 2)
			PlayerBTerritory.Add(map);
	}
	
	[RPC]
	void RPCRemoveTerritory(string tName, int side){
		Transform map = GameObject.Find(tName).transform;
		if(side == 1)
			PlayerATerritory.Remove(map);
		else if(side == 2)
			PlayerBTerritory.Remove(map);
	}
}
