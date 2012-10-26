using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoundCounter : MonoBehaviour {
	public int roundCounter = 0;
	public IList PlayerAChesses;
	public IList PlayerBChesses;
	public IList PlayerATerritory;
	public IList PlayerBTerritory;
	public Transform playerA;
	public Transform playerB;
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
		PlayerAChesses = new List<Transform>();
		PlayerBChesses = new List<Transform>();
		PlayerATerritory = new List<Transform>();
		PlayerBTerritory = new List<Transform>();
		PlayerAChesses.Clear();
		PlayerBChesses.Clear();
		PlayerAChesses.Add(playerA);
		foreach(Transform gf in playerA.GetComponent<CharacterProperty>().soldiers){
			Transform gfClone = Instantiate(gf,noWhere,Quaternion.identity) as Transform;
			PlayerAChesses.Add(gfClone);
			gfClone.GetComponent<CharacterProperty>().death = true;
			gfClone.GetComponent<CharacterProperty>().Player = playerA.GetComponent<CharacterProperty>().Player;
		}
		PlayerBChesses.Add(playerB);
		foreach(Transform gf in playerB.GetComponent<CharacterProperty>().soldiers){
			Transform gfClone = Instantiate(gf,noWhere,Quaternion.identity) as Transform;
			PlayerBChesses.Add(gfClone);
			gfClone.GetComponent<CharacterProperty>().death = true;
			gfClone.GetComponent<CharacterProperty>().Player = playerB.GetComponent<CharacterProperty>().Player;
		}
		PlayerATerritory.Add(GameObject.Find("unit_start_point_A").transform);
		PlayerBTerritory.Add(GameObject.Find("unit_start_point_B").transform);
	}
	
	public void AddTerritory(Transform map, int player){
		if(player==1)
			PlayerATerritory.Add(map);
		else if(player==2)
			PlayerBTerritory.Add(map);
	}
	
	public void RemoveTerritory(Transform map, int player){
		if(player==1)
			PlayerATerritory.Remove(map);
		else if(player==2)
			PlayerBTerritory.Remove(map);
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
	}
	
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
			if(!gfp.death){
				gfp.Hp = gfp.ModifiedDefPow;
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
	
	// Update is called once per frame
	void Update () {
		if(roundCounter%2 == 1){
			if(CheckRound(PlayerAChesses)|| checkAllDeath(PlayerAChesses)){
				roundCounter += 1;
				updateWaitRounds();
				updateRound(PlayerBChesses);
				oldCamPosition = transform.position;
				getNewCamPos(2);
				camMoveMode = true;
				playerB.GetComponent<ManaCounter>().Mana+=2;
				revivePlayer(playerB);
			}
		}else{
			if(CheckRound(PlayerBChesses)|| checkAllDeath(PlayerBChesses)){
				roundCounter += 1;
				updateWaitRounds();
				updateRound(PlayerAChesses);
				oldCamPosition = transform.position;
				getNewCamPos(1);
				camMoveMode = true;
				playerA.GetComponent<ManaCounter>().Mana+=2;
				revivePlayer(playerA);
			}
		}
		
		if(camMoveMode){
			translateMainCam(80);
		}
	}
	
	void OnGUI(){
		GUI.Box(new Rect(10,10,100,90), "Round: "+roundCounter);
	}
	
	void OnApplicationQuit(){
		foreach(Transform gf in playerA.GetComponent<CharacterProperty>().soldiers){
			gf.GetComponent<CharacterProperty>().death = true;
		}
		foreach(Transform gf in playerB.GetComponent<CharacterProperty>().soldiers){
			gf.GetComponent<CharacterProperty>().death = true;
		}
	}
}
