using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using MapUtility;
using BuffUtility;

public class RoundCounter : MonoBehaviour {
	public int roundCounter = 0;
	public IList AllChesses;
	public IList PlayerAChesses;
	public IList PlayerBChesses;
	public IList PlayerATerritory;
	public IList PlayerBTerritory;
	public Transform playerA;
	public Transform playerB;
	public Texture2D InfoBlack;
	Vector3 noWhere = new Vector3(0.0f,1000.0f,0.0f);
	private bool camMoveMode = false;
	private Vector3 oldCamPosition;
	private Vector3 newCamPosition;
	private const float viewOffsetZ = 30.0f;
	private int camStep = 0;
	public Material TerritoryA;
	public Material TerritoryB;
	
	
	// Use this for initialization
	void Start () {
		roundCounter = 1;
		playerA = GameObject.Find("pSummonerA").transform;
		playerB = GameObject.Find("pSummonerB").transform;
		AllChesses = new List<Transform>();
		PlayerAChesses = new List<Transform>();
		PlayerBChesses = new List<Transform>();
		PlayerATerritory = new List<Transform>();
		PlayerBTerritory = new List<Transform>();
		PlayerAChesses.Clear();
		PlayerBChesses.Clear();
		PlayerAChesses.Add(playerA);
		foreach(Transform gf in playerA.GetComponent<CharacterProperty>().soldiers){
			Transform gfClone = Instantiate(gf,noWhere,Quaternion.identity) as Transform;
			gfClone.gameObject.layer = 10;
			PlayerAChesses.Add(gfClone);
			gfClone.GetComponent<CharacterProperty>().death = true;
			gfClone.GetComponent<CharacterProperty>().Player = playerA.GetComponent<CharacterProperty>().Player;
			gfClone.GetComponent<CharacterProperty>().InitPlayer = playerA.GetComponent<CharacterProperty>().Player;
		}
		PlayerBChesses.Add(playerB);
		foreach(Transform gf in playerB.GetComponent<CharacterProperty>().soldiers){
			Transform gfClone = Instantiate(gf,noWhere,Quaternion.identity) as Transform;
			gfClone.gameObject.layer = 10;
			PlayerBChesses.Add(gfClone);
			gfClone.GetComponent<CharacterProperty>().death = true;
			gfClone.GetComponent<CharacterProperty>().Player = playerB.GetComponent<CharacterProperty>().Player;
			gfClone.GetComponent<CharacterProperty>().InitPlayer = playerB.GetComponent<CharacterProperty>().Player;
		}
		foreach(Transform chess in PlayerAChesses){
			AllChesses.Add(chess);
		}
		foreach(Transform chess in PlayerBChesses){
			AllChesses.Add(chess);
		}
		
		PlayerATerritory.Add(GameObject.Find("unit_start_point_A").transform);
		PlayerBTerritory.Add(GameObject.Find("unit_start_point_B").transform);
	}
	
	void Awake(){
		roundCounter = 1;
	}
	
	public void AddTerritory(Transform map, int player){
		if(player==1){
			PlayerATerritory.Add(map);
			networkView.RPC("RPCAddTerritory",RPCMode.Others,map.name,1);
		}else if(player==2){
			PlayerBTerritory.Add(map);
			networkView.RPC("RPCAddTerritory",RPCMode.Others,map.name,2);
		}
		
	}
	
	public void RemoveTerritory(Transform map, int player){
		if(player==1){
			PlayerATerritory.Remove(map);
			networkView.RPC("RPCRemoveTerritory",RPCMode.Others,map.name,1);
		}else if(player==2){
			PlayerBTerritory.Remove(map);
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
	
	void translateMainCam(int segment){
		float segX = (oldCamPosition.x-newCamPosition.x)/segment;
		float segZ = (oldCamPosition.z-newCamPosition.z)/segment;
		transform.position = new Vector3(transform.position.x-segX,transform.position.y,transform.position.z-segZ);
		camStep+=1;
		if(camStep==segment){
			camMoveMode = false;
			camStep = 0;
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
			gfp.Moved = false;
			gfp.Attacked = false;
			gfp.Activated = false;
			gfp.TurnFinished = false;
			if(!gfp.death && gfp.AbleRestore){
				gfp.Hp = gfp.ModifiedDefPow;
			}
		}
	}
	
	void updateUnnormalStatus(){
		foreach(Transform chess in AllChesses){
			CharacterProperty cProperty = chess.GetComponent<CharacterProperty>();
			cProperty.UnStatus = false;
			List<UnnormalStatus> keys = new List<UnnormalStatus>(cProperty.UnStatusCounter.Keys);
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
							cProperty.AbleRestore = false;
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
							cProperty.AbleRestore = true;
							break;
					}
				}
				if(cProperty.LastUnStatusCounter[key] > 0){
					cProperty.LastUnStatusCounter[key] -= 1;
				}
			}
		}
	}
	
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
	
	void updatePassive(){
		foreach(Transform chess in AllChesses){
			if(!chess.GetComponent<CharacterProperty>().death){
				CharacterPassive chessPassive = chess.GetComponent<CharacterPassive>();
				foreach(PassiveType passive in Enum.GetValues(typeof(PassiveType))){
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
			foreach(Transform chess in PlayerBChesses){
				if(!chess.GetComponent<CharacterProperty>().death){
					liveOne = chess;
					break;
				}
			}
		}
		if(liveOne != null)
			newCamPosition = new Vector3(liveOne.position.x, liveOne.position.y, liveOne.position.z-viewOffsetZ);
	}
	
	void revivePlayer(Transform player){
		CharacterProperty property = player.GetComponent<CharacterProperty>();
		if(property.death && property.Ready){
			if(property.Player==1)
				player.position = GameObject.Find("unit_start_point_A").transform.position;
			else if(property.Player==2)
				player.position = GameObject.Find("unit_start_point_B").transform.position;
			property.death = false;
			property.Hp = property.defPower;
			player.renderer.enabled = true;
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
		if(Network.peerType == NetworkPeerType.Client){
			transform.GetComponent<selection>().Playing = transform.GetComponent<selection>().BPlaying;
		}else if(Network.peerType == NetworkPeerType.Server){
			transform.GetComponent<selection>().Playing = transform.GetComponent<selection>().APlaying;
		}
	}
	
	
	// Update is called once per frame
	void Update () {
		if(roundCounter%2 == 1){
			if(CheckRound(PlayerAChesses)|| checkAllDeath(PlayerAChesses)){
				updateWaitRounds();
				oldCamPosition = transform.position;
				getNewCamPos(2);
				camMoveMode = true;
				playerB.GetComponent<ManaCounter>().Mana+=2;
				updateUnnormalStatus();
				revivePlayer(playerB);
				UpdateChessList();
				updateRound(PlayerBChesses);
				updateAllCharactersPowers();
				updatePassive();
				transform.GetComponent<selection>().APlaying=false;
				transform.GetComponent<selection>().BPlaying=true;
				updatePlaying();
				roundCounter += 1;
			}
		} 
		if(roundCounter%2 == 0){
			if(CheckRound(PlayerBChesses)|| checkAllDeath(PlayerBChesses)){
				updateWaitRounds();
				oldCamPosition = transform.position;
				getNewCamPos(1);
				camMoveMode = true;
				playerA.GetComponent<ManaCounter>().Mana+=2;
				updateUnnormalStatus();
				revivePlayer(playerA);
				UpdateChessList();
				updateRound(PlayerAChesses);
				updateAllCharactersPowers();
				updatePassive();
				updatePlaying();
				transform.GetComponent<selection>().APlaying=true;
				transform.GetComponent<selection>().BPlaying=false;
				updatePlaying();
				roundCounter += 1;
				
			}
		}
		
		if(camMoveMode){
			translateMainCam(80);
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
			gf.GetComponent<CharacterProperty>().death = true;
		}
		foreach(Transform gf in playerB.GetComponent<CharacterProperty>().soldiers){
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
