using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StageStates : MonoBehaviour {
	public StageProperty[] StageList = new StageProperty[6];
	
	// Use this for initialization
	void Start () {
		StageList[0] = new StageProperty("human village", false, 1, 6); 
		StageList[1] = new StageProperty("mole village", false, 2, 5);
		StageList[2] = new StageProperty("elf village", false, 3, 5);
		StageList[3] = new StageProperty("murloc island", false, 4, 5);
		StageList[4] = new StageProperty("polar island", false, 5, 5);
		StageList[5] = new StageProperty("dark castle", false, 6, 5);
		
		//sub stage 1 
		StageList[0].InsertSubNames("1-1 Woods");
		StageList[0].InsertSubNames("1-2 Gate");
		StageList[0].InsertSubNames("1-3 Way Home");
		StageList[0].InsertSubNames("1-4 Home");
		StageList[0].InsertSubNames("1-5 Mayor Office");
		StageList[0].InsertSubNames("1-6 Devil Debut");
		
		//sub stage 2
		StageList[1].InsertSubNames("2-1 Desert");
		StageList[1].InsertSubNames("2-2 Cactus Gate");
		StageList[1].InsertSubNames("2-3 In the Village");
		StageList[1].InsertSubNames("2-4 Dear Friends");
		StageList[1].InsertSubNames("2-5 Devil Again!");
		
		//sub stage 3
		StageList[2].InsertSubNames("3-1 Big Elf Gate");
		StageList[2].InsertSubNames("3-2 Dark Forest");
		StageList[2].InsertSubNames("3-3 Follow the River");
		StageList[2].InsertSubNames("3-4 Elf Altar");
		StageList[2].InsertSubNames("3-5 Evil Circle");
		
		//sub stage4 
		StageList[3].InsertSubNames("4-1 New Journey");
		StageList[3].InsertSubNames("4-2 Murloc Hobor");
		StageList[3].InsertSubNames("4-3 Sneak Attack");
		StageList[3].InsertSubNames("4-4 Way to the Dark");
		StageList[3].InsertSubNames("4-5 Evil Guard");
		
		//sub stage5
		StageList[4].InsertSubNames("5-1 White World");
		StageList[4].InsertSubNames("5-2 Villgers");
		StageList[4].InsertSubNames("5-3 Wrong Direction");
		StageList[4].InsertSubNames("5-4 Strong Devil");
		StageList[4].InsertSubNames("5-5 Dark Power");
		
		//sub stage6
		StageList[5].InsertSubNames("6-1 Dark Power");
		StageList[5].InsertSubNames("6-2 Way to Evil");
		StageList[5].InsertSubNames("6-3 Endless Strike");
		StageList[5].InsertSubNames("6-4 The Prince");
		StageList[5].InsertSubNames("6-5 Final Destination");
		
		this.GetComponent<Basic_UI>().CurrentStage = (StageProperty)StageList[0];
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}


public class StageProperty{
	public bool Locked;  
	public int Level; 
	public string StageName; 
	public bool[] SubLevel; 
	public int SubNum;
	public IList SubLevelNames; 
	public StageProperty(string name, bool locked, int level, int sub){
		StageName = name; 
		Locked = locked; 
		Level  = level;
		SubNum = sub;
		SubLevel = new bool[sub];
		SubLevelNames = new List<string>(); 
		for(int i=0; i<sub; i++){
			SubLevel[i] = false;
		}
		if(level == 1){
			SubLevel[0] = true;
		}
	}
	
	public bool InsertSubNames(string name){
		bool succeed = false;
		
		SubLevelNames.Add(name);
		succeed = true;
		return succeed; 
	}
}