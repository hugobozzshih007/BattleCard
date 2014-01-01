using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;
using System.Linq;
using BuffUtility;

public class CalculateTactics : MonoBehaviour {
	PlacePrizes pp;
	GeneralSelection currentSel;
	AttackCalFX atkCal; 
	RoundCounter currentRC; 
	const float singleUnit = 6.9282f; 
	const int rangeAffect = 5;
	const int debuffAffect = 10;
	const int nobuffAffect = 5;
	const float unitScore = 10.0f;
	const float basicAtkScore = 100.0f;
	const float leadingAtkScore = 200.0f;
	const float preVisionTacticWeight = 0.2f;
	const float preVisionMoveWeight = 0.8f;
	int allMaps;
	BuffInfoUI buffInfo;
		
	// Use this for initialization
	void Start () {
		allMaps = GameObject.Find("Maps").transform.childCount;
		currentSel = Camera.mainCamera.GetComponent<GeneralSelection>();
		atkCal = Camera.mainCamera.GetComponent<AttackCalFX>();
		currentRC = Camera.mainCamera.GetComponent<RoundCounter>();
		buffInfo = Camera.mainCamera.GetComponent<BuffInfoUI>();
		pp = GameObject.Find("Maps").GetComponent<PlacePrizes>();
	}
	
	bool IsTacticSkill(TacticPoint tp){
		bool tacticSkill = false;  
		if(tp.Tactic != Tactics.Expend && tp.Tactic != Tactics.Range_Attack && tp.Tactic != Tactics.Melee_Attack && tp.Tactic != Tactics.none)
			tacticSkill = true; 
		else 
			tacticSkill = false;
			
		return tacticSkill;
	}
	
	public IList GetTactic(Transform gf, bool attacked, int leftCmd){
		Dictionary<TacticPoint[], int> sortDict = new Dictionary<TacticPoint[], int>();
		Transform localMap = gf.GetComponent<CharacterSelect>().getMapPosition();
		Transform skill = gf.FindChild("Skills").GetChild(0);
		SkillProperty skillP = skill.GetComponent<SkillProperty>();
		GetGfBasicScore(gf);
		Tactics[] tacNone = new Tactics[]{Tactics.none};
		if(attacked){
			tacNone = new Tactics[]{Tactics.Melee_Attack, Tactics.Range_Attack};
		}
		
		if(CheckSteps(gf, 1)){
			//case move -> do
			if(leftCmd > 1){
				TacticPoint[] moveDo = new TacticPoint[1]; 
				moveDo[0] = GetMoveTactic(gf, 1,tacNone);
				int avp = Mathf.RoundToInt((float)moveDo[0].Point/2.0f);
				sortDict.Add(moveDo, avp);
			}
			//case move
			TacticPoint[] moveOnly = new TacticPoint[1];
			moveOnly[0] = GetUnitToMove(gf, 1, localMap);
			sortDict.Add(moveOnly, moveOnly[0].Point);
		}
		
		if(CheckSteps(gf, 2)&& leftCmd > 2){
			// Case Three: Move -> Move -> Do
			TacticPoint[] moveMoveDo = new TacticPoint[1];
			moveMoveDo[0] = GetMoveTactic(gf, 2, tacNone);
			int avp = Mathf.RoundToInt((float)moveMoveDo[0].Point/3.0f);
			sortDict.Add(moveMoveDo, avp);
		}
		//case Do
		TacticPoint[] doOnly = new TacticPoint[1];
		doOnly[0] = GetUnitTactic(localMap, gf, tacNone);
		sortDict.Add(doOnly, doOnly[0].Point);
		//Sort the dict by it's value
		var sortedDict = (from entry in sortDict orderby entry.Value descending select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
		IList tactics = new List<TacticPoint>();
		foreach(TacticPoint tp in sortedDict.ElementAt(0).Key){
			tactics.Add(tp);
		}
		
		return tactics;
	}
	
	//get the different score from different gf
	void GetGfBasicScore(Transform gf){
		
	}

	public Transform GetRevivePos(){
		Transform targetMap = null;
		IList pPosList = new List<Transform>();
		pPosList = currentSel.GetAllEmptyMaps();
		IList farList = new List<Transform>();
		farList = MapHelper.GetFarestMaps(currentRC.playerA, pPosList);
		int rnd = Random.Range(0, farList.Count-1);
		targetMap = farList[rnd] as Transform;
		return targetMap;
	}
	
	// move and do somthing
	TacticPoint GetMoveTactic(Transform gf, int moveTimes, Tactics[] lastTactic){
		TacticPoint tp = new TacticPoint(gf, gf.GetComponent<CharacterSelect>().getMapPosition());
		IList maxSteps = new List<Transform>();
		if(moveTimes == 1)
			maxSteps = GetFirstSteps(gf, null);
		else if(moveTimes == 2)
			maxSteps = GetMaxSteps(gf, null);
		
		Dictionary<int,TacticPoint> sortDict = new Dictionary<int, TacticPoint>();
		
		if(maxSteps.Count>0){
			foreach(Transform map in maxSteps){
				TacticPoint tPoint = GetUnitTactic(map, gf, lastTactic);
				if(!sortDict.ContainsKey(tPoint.Point))
					sortDict.Add(tPoint.Point, tPoint);
			}
			
			var list = sortDict.Keys.ToList();
			list.Sort();
			list.Reverse();
			tp = sortDict[list[0]];
		}
		
		return tp;
	}
	
	bool CheckSteps(Transform gf, int step){
		bool isStep = false;
		IList firstStep  = new List<Transform>();
		firstStep = GetFirstSteps(gf, null); 
		if(firstStep.Count>0){
			if(step == 1)
				isStep = true;
			else if(step == 2){
				IList secondStep  = new List<Transform>();
				secondStep = GetMaxSteps(gf, null);
				if(secondStep.Count>0)
					isStep = true;
			}else if(step >2){
				IList secondStep  = new List<Transform>();
				secondStep = GetMaxSteps(gf, null);
				if(secondStep.Count>0){
					IList thirdStep  = new List<Transform>();
					thirdStep = GetThirdSteps(gf, null);
					if(thirdStep.Count>0)
						isStep = true;
				}
			}
		}
		return isStep;
	}
	
	public IList GetThirdSteps(Transform gf, Transform root){
		IList thirdSteps = new List<Transform>();
		IList maxSteps = new List<Transform>();
		IList firstSteps = new List<Transform>();
		CharacterProperty gfp = gf.GetComponent<CharacterProperty>();
		CharacterSelect gfSel = gf.GetComponent<CharacterSelect>();
		gfSel.MoveRangeList.Clear();
		currentSel.updateMapSteps();
		if(root == null){
			root = gfSel.getMapPosition();
		}
		gfSel.findMoveRange(root, 0, gfp.BuffMoveRange*3);
		foreach(Transform maps in gfSel.MoveRangeList){
			thirdSteps.Add(maps);
		}
		firstSteps = GetFirstSteps(gf, null);
		maxSteps = GetMaxSteps(gf, null);
		foreach(Transform m in firstSteps){
			if(thirdSteps.Contains(m))
				thirdSteps.Remove(m);
		}
		foreach(Transform m in maxSteps){
			if(thirdSteps.Contains(m))
				thirdSteps.Remove(m);
		}

		return thirdSteps;  
	}
	
	public IList GetMaxSteps(Transform gf, Transform root){
		IList maxSteps = new List<Transform>();
		IList firstSteps = new List<Transform>();
		CharacterProperty gfp = gf.GetComponent<CharacterProperty>();
		CharacterSelect gfSel = gf.GetComponent<CharacterSelect>();
		gfSel.MoveRangeList.Clear();
		currentSel.updateMapSteps();
		if(root == null){
			root = gfSel.getMapPosition();
		}
			
		gfSel.findMoveRange(root, 0, gfp.BuffMoveRange*2);
		foreach(Transform maps in gfSel.MoveRangeList){
			maxSteps.Add(maps);
		}
		firstSteps = GetFirstSteps(gf, null);
		foreach(Transform m in firstSteps){
			if(maxSteps.Contains(m))
				maxSteps.Remove(m);
		}
		return maxSteps;
	}
	
	public IList GetFirstSteps(Transform gf, Transform root){
		IList initSteps = new List<Transform>();
		CharacterProperty gfp = gf.GetComponent<CharacterProperty>();
		CharacterSelect gfSel = gf.GetComponent<CharacterSelect>();
		gfSel.MoveRangeList.Clear();
		currentSel.updateMapSteps();
		if(root == null){
			root = gfSel.getMapPosition();
		}
		gfSel.findMoveRange(root, 0, gfp.BuffMoveRange);
		foreach(Transform maps in gfSel.MoveRangeList){
			initSteps.Add(maps);
		}
		return initSteps;
	}
	
	TacticPoint GetUnitToMove(Transform gf, int step, Transform root){
		IList steps = new List<Transform>();
		TacticPoint topTp = new TacticPoint(gf, null); 
		Dictionary<float, TacticPoint> tpDict = new Dictionary<float, TacticPoint>();
		
		//Transform dirMap = PreVision.GetDirectionMap(gf);
		
		if(step == 1){
			steps = GetFirstSteps(gf, root);
		}else if(step == 2){
			steps = GetMaxSteps(gf, root);
		}else if(step == 3){
			steps = GetThirdSteps(gf, root);
		}
		
		if(steps.Count>0){
			foreach(Transform maps in steps){
				TacticPoint tp = GetUnitPoint(maps, gf);
				//print(gf.name + " direction map: " +dirMap.name);
				/*if(maps == dirMap){
					float addWeightPoint = (1.0f+preVisionMoveWeight)*(float)tp.Point;
					tp.Point = Mathf.RoundToInt(addWeightPoint);
					print(gf.name + " Got Pre-Vision");
				}*/
				if(!tpDict.ContainsKey(tp.Point))
					tpDict.Add(tp.Point, tp);
			}
			var list = tpDict.Keys.ToList();
			list.Sort();
			list.Reverse();
			topTp = tpDict[list[0]];
		}
		return topTp;
	}
	
	//do somthing
	TacticPoint GetUnitTactic(Transform map, Transform gf, Tactics[] reMoveT){
		TacticPoint tp = new TacticPoint(gf, map);
		CharacterTactics gft = gf.GetComponent<CharacterTactics>();  
		IList tacticList = new List<Tactics>();
		float weight = 1.0f;
		Transform dirMap = PreVision.GetDirectionMap(gf);
		if(map == dirMap){
			weight += preVisionTacticWeight;
		}
		
		// not to do same thing again 
		foreach(Tactics t in gft.TacticList){
			tacticList.Add(t);
		}
		for(int i=0; i<reMoveT.Length; i++){
			foreach(Tactics t in gft.TacticList){
				if(t == reMoveT[i])
					tacticList.Remove(t);
			}
		}
		Dictionary<int, TacticPoint> tacticPointDict = new Dictionary<int,TacticPoint>();
		//Dictionary<Tactics, int> tacticPointDict = new Dictionary<Tactics, int>();
		//IList tacticPointList = new List<TacticPoint>();
		foreach(Tactics t in tacticList){
			TacticPoint tPoint = GetSpecificTacticPoint(t, map, gf);
			//tacticPointList.Add(tPoint);
			float addWeightPoint = (float)tPoint.Point*weight;
			tPoint.Point = Mathf.RoundToInt(addWeightPoint);
			if(!tacticPointDict.ContainsKey(tPoint.Point))
				tacticPointDict.Add(tPoint.Point, tPoint);
			/*if(!tacticPointDict.ContainsKey(tPoint.Tactic))
				tacticPointDict.Add(tPoint.Tactic, tPoint.Point);*/
		}
		var list = tacticPointDict.Keys.ToList();
		list.Sort();
		list.Reverse();
		tp = tacticPointDict[list[0]];
		/*var sortedDict = (from entry in tacticPointDict orderby entry.Value ascending select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
		int biggestPoint = sortedDict.Values.ElementAt(0);
		foreach(TacticPoint tpp in tacticPointList){
			if(tpp.Point == biggestPoint){
				tp = tpp;  
				break;
			}
		}*/
		return tp;
	}
	
	TacticPoint GetUnitPoint(Transform map, Transform gf){
		TacticPoint unitTp = new TacticPoint(gf, map);
		float score = 0.0f; 
		IList nearMaps = new List<Transform>();
		CharacterProperty gfp = gf.GetComponent<CharacterProperty>();
		CharacterSelect gfSel = gf.GetComponent<CharacterSelect>();
		gfSel.MoveRangeList.Clear();
		currentSel.updateMapSteps();
		gfSel.findMoveRange(map, 0, 2);
		foreach(Transform maps in gfSel.MoveRangeList){
			nearMaps.Add(maps);
		}
		if(!nearMaps.Contains(map)){
			nearMaps.Add(map);
		}
		foreach(Transform unit in nearMaps){
			float unitFact = GetEmptyUnitFact(unit);
			float newScore = unitFact*unitScore*0.3f;
			score += newScore; 
		}
		if(pp.GetPrizeMap().Count>0){
			if(pp.GetPrizeMap().Contains(map)){
				score+=5.0f;
			}
		}
		unitTp.Point = Mathf.RoundToInt(score);
		return unitTp; 
	}
	
	TacticPoint GetSpecificTacticPoint(Tactics tactic, Transform map, Transform gf){
		TacticPoint tp = new TacticPoint(gf, map);
		switch(tactic){
			case Tactics.Melee_Attack:
				tp = GetMeleeAttackPoint(map, gf);
				break;
			case Tactics.Range_Attack:
				tp = GetMeleeAttackPoint(map, gf);
				break;
			case Tactics.Expend:
				tp = GetExpandPoint(map, gf);
				break;
			default:
				//calculate skill tactics
				tp = GetSkillPoint(map, gf);		
				break;
		}
		return tp;
	}
	
	bool CheckIfDefensed(Tactics[] tacs){
		bool defensed = false; 
		foreach(Tactics t in tacs){
			if(t == Tactics.Expend){
				defensed = true;
				break;
			}
		}
		return defensed;
	}
	
	TacticPoint GetMeleeAttackPoint(Transform map, Transform gf){
		IList attackableList = new List<Transform>();
		int sideMapCount = currentRC.PlayerBTerritory.Count;
		Identity mapID = map.GetComponent<Identity>();
		int currentBuff = BuffCalculation.BuffXValue(buffInfo.GetTerritoryPersent(2)); 
		//int currentRedBuff = BuffCalculation.BuffXValue(buffInfo.GetTerritoryPersent(1));
		//int potentailBuff = 0;
		int gfNewDamage = 0;
		int gfNewDef = 0;
		//int potentailDeBuff = 0; 
		bool possibleDie = false;
		int trueDamage = 0;  
		IList mapsNotInTerritory = new List<Transform>(); 
		IList mapsInRedTerritory = new List<Transform>();
		TacticPoint tp = new TacticPoint(gf, Tactics.Melee_Attack,map,0);
		Dictionary<Transform, int> pointDict = new Dictionary<Transform, int>();
		CharacterProperty gfp = gf.GetComponent<CharacterProperty>();
		BuffList gfBuffList = gf.GetComponent<BuffList>();
		bool atkBuff = gf.GetComponent<BuffList>().CheckBuff(BuffType.Attack);
		attackableList = atkCal.GetAttackableTarget(map,gf);
	
		if(attackableList.Count>0){
			//counting the new buffrate if this map has been claimed
			/*if(CheckIfDefensed(expended)){
				int sideMapCount = currentRC.PlayerBTerritory.Count;
				if(gfp.Summoner || gfp.LeadingCharacter){
					
					foreach(Transform m in mapID.neighbor){
						if(m!=null && !currentRC.PlayerBTerritory.Contains(m)){
							mapsNotInTerritory.Add(m);
						}
						if(currentRC.PlayerATerritory.Contains(m)){
							mapsInRedTerritory.Add(m);
						}
					}
					if(currentRC.PlayerATerritory.Contains(map))
						mapsInRedTerritory.Add(map);
					
					if(!currentRC.PlayerBTerritory.Contains(map)) 
						mapsNotInTerritory.Add(map);
					
					int newMapCount = mapsNotInTerritory.Count + sideMapCount;
					int newRate = Mathf.RoundToInt(((float)newMapCount / (float)allMaps)*100.0f);
					potentailBuff = BuffCalculation.BuffXValue(newRate);
					
					int newRedMapCount = currentRC.PlayerATerritory.Count - mapsInRedTerritory.Count;
					int newRedRate = Mathf.RoundToInt(((float)newRedMapCount / (float)allMaps)*100.0f);
					potentailDeBuff = 0-BuffCalculation.BuffXValue(newRedRate);
					
				}else{
					if(!currentRC.PlayerBTerritory.Contains(map)){
						int newMapCount = 1 + sideMapCount;
						int newRate = Mathf.RoundToInt(((float)newMapCount / (float)allMaps)*100.0f);
						potentailBuff = BuffCalculation.BuffXValue(newRate);
					}else{
						potentailBuff = currentBuff;
					}
					if(currentRC.PlayerATerritory.Contains(map)){
						mapsInRedTerritory.Add(map);
						int newRedMapCount = currentRC.PlayerATerritory.Count - 1;
						int newRedRate = Mathf.RoundToInt(((float)newRedMapCount / (float)allMaps)*100.0f);
						potentailDeBuff = BuffCalculation.BuffXValue(newRedRate);
					}else{
						potentailDeBuff = currentRedBuff;
					}	
				}
				if(atkBuff)
					 gfNewDamage = gfp.Damage + 2*potentailBuff;
				else
					gfNewDamage = gfp.Damage; 
			}else{
				gfNewDamage = gfp.Damage; 
			}*/
			if(currentRC.PlayerBTerritory.Contains(map)){
				if(MapHelper.CheckBuffList(BuffType.Attack, gf)){
					gfNewDamage = gfp.AtkPower + gfBuffList.ExtraDict[BuffType.Attack] + currentBuff;
				}else{
					gfNewDamage = gfp.Damage; 
				}
				
				if(MapHelper.CheckBuffList(BuffType.Defense, gf)){
					gfNewDef = gfp.DefPower+ gfBuffList.ExtraDict[BuffType.Defense] + currentBuff;
				}else{
					gfNewDef = gfp.ModifiedDefPow;
				}
			}else if(currentRC.PlayerATerritory.Contains(map)){
				if(MapHelper.CheckDeBuffList(BuffType.Attack, gf)){
					gfNewDamage = gfp.AtkPower + gfBuffList.ExtraDict[BuffType.Attack] - currentBuff;
				}else{
					gfNewDamage = gfp.Damage;
				}
				
				if(MapHelper.CheckDeBuffList(BuffType.Defense, gf)){
					gfNewDef = gfp.DefPower + gfBuffList.ExtraDict[BuffType.Defense] - currentBuff;
				}else{
					gfNewDef = gfp.ModifiedDefPow;
				}
			}else{
				gfNewDamage = gfp.Damage;
				gfNewDef = gfp.ModifiedDefPow;
			}
			
			
			//calculate tactic points
		
			foreach(Transform unit in attackableList){
				Transform chess = MapHelper.GetMapOccupiedObj(unit);
				CharacterProperty chessP = chess.GetComponent<CharacterProperty>();
				/*BuffList chessBuff = chess.GetComponent<BuffList>();
				int newDefPower = chessP.ModifiedDefPow;
				if(chessBuff.CheckDeBuff(BuffType.Defense)){
					if(mapsInRedTerritory.Contains(unit)){
						newDefPower -= 2*potentailDeBuff;
					}else if(mapsInRedTerritory.Count>0){
						newDefPower = chessP.defPower + (int)chessBuff.ExtraDict[BuffType.Defense] + potentailDeBuff;
					}
				}*/
				//calculate possible die, truedamge and gotDamage
				trueDamage = gfNewDamage - chessP.ModifiedDefPow; 
				if(trueDamage <= 0)
					trueDamage = 0;
				else if(trueDamage >=chessP.Hp)
					trueDamage = chessP.Hp;
				float damageRate = (float)trueDamage/(float)chessP.Hp;
				int point = Mathf.RoundToInt(basicAtkScore*damageRate);
				/*if(!chessP.Summoner)
					point = Mathf.RoundToInt(basicAtkScore*damageRate);
				else
					point = Mathf.RoundToInt(leadingAtkScore*damageRate);
				//Simply count if the map is side one 
				if(currentRC.PlayerATerritory.Contains(map))
					point -= debuffAffect;
				if(!currentRC.PlayerATerritory.Contains(map) && !currentRC.PlayerBTerritory.Contains(map))
					point -= nobuffAffect;*/
				int damagedScore = DamagedScore(gf, chess, gfNewDef);
				if(damageRate<1.0f)
					point -= damagedScore;
				if(point <=0)
					point = 0;
				pointDict.Add(chess, point);
			}
			var sortedDict = (from entry in pointDict orderby entry.Value descending select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
			tp = new TacticPoint(gf, Tactics.Melee_Attack, map, (Transform)sortedDict.ElementAt(0).Key, (int)sortedDict.ElementAt(0).Value);
		}
		//print(gf.name + ": " + tp.Tactic + " " + tp.Point + " " + tp.MapUnit); 
		return tp;
	}
	
	/*TacticPoint GetRangeAttackPoint(Transform map, Transform gf, Tactics[] expended){
		IList attackableList = new List<Transform>();
		Identy mapID = map.GetComponent<Identy>();
		int currentBuff = BuffCalculation.BuffXValue(buffInfo.GetTerritoryPersent(2)); 
		int currentRedBuff = BuffCalculation.BuffXValue(buffInfo.GetTerritoryPersent(1));
		int potentailBuff = 0;
		int gfNewDamage = 0;
		int potentailDeBuff = 0; 
		IList mapsNotInTerritory = new List<Transform>(); 
		IList mapsInRedTerritory = new List<Transform>();
		TacticPoint tp = new TacticPoint(gf, Tactics.Range_Attack,map,0);
		Dictionary<Transform, int> pointDict = new Dictionary<Transform, int>();
		CharacterProperty gfp = gf.GetComponent<CharacterProperty>();
		bool atkBuff = gf.GetComponent<BuffList>().CheckBuff(BuffType.Attack);
		attackableList = atkCal.GetAttackableTarget(map,gf);
		//counting the new buffrate if this map has been claimed by last move
		if(CheckIfDefensed(expended)){
			int sideMapCount = currentRC.PlayerBTerritory.Count;
			if(gfp.Summoner || gfp.LeadingCharacter){
				
				foreach(Transform m in mapID.neighbor){
					if(m!=null && !currentRC.PlayerBTerritory.Contains(m)){
						mapsNotInTerritory.Add(m);
					}
					if(currentRC.PlayerATerritory.Contains(m)){
						mapsInRedTerritory.Add(m);
					}
				}
				if(currentRC.PlayerATerritory.Contains(map))
					mapsInRedTerritory.Add(map);
				
				if(!currentRC.PlayerBTerritory.Contains(map)) 
					mapsNotInTerritory.Add(map);
				
				int newMapCount = mapsNotInTerritory.Count + sideMapCount;
				int newRate = Mathf.RoundToInt(((float)newMapCount / (float)allMaps)*100.0f);
				potentailBuff = BuffCalculation.BuffXValue(newRate);
				
				int newRedMapCount = currentRC.PlayerATerritory.Count - mapsInRedTerritory.Count;
				int newRedRate = Mathf.RoundToInt(((float)newRedMapCount / (float)allMaps)*100.0f);
				potentailDeBuff = 0-BuffCalculation.BuffXValue(newRedRate);
				
			}else{
				if(!currentRC.PlayerBTerritory.Contains(map)){
					int newMapCount = 1 + sideMapCount;
					int newRate = Mathf.RoundToInt(((float)newMapCount / (float)allMaps)*100.0f);
					potentailBuff = BuffCalculation.BuffXValue(newRate);
				}else{
					potentailBuff = currentBuff;
				}
				if(currentRC.PlayerATerritory.Contains(map)){
					mapsInRedTerritory.Add(map);
					int newRedMapCount = currentRC.PlayerATerritory.Count - 1;
					int newRedRate = Mathf.RoundToInt(((float)newRedMapCount / (float)allMaps)*100.0f);
					potentailDeBuff = BuffCalculation.BuffXValue(newRedRate);
				}else{
					potentailDeBuff = currentRedBuff;
				}	
			}
			if(atkBuff)
				 gfNewDamage = gfp.Damage + 2*potentailBuff;
			else
				gfNewDamage = gfp.Damage; 
		}else{
			gfNewDamage = gfp.Damage; 
		}
		//calculate tactic points
		if(attackableList.Count>0){
			foreach(Transform unit in attackableList){
				Transform chess = MapHelper.GetMapOccupiedObj(unit);
				CharacterProperty chessP = chess.GetComponent<CharacterProperty>();
				BuffList chessBuff = chess.GetComponent<BuffList>();
				int newDefPower = chessP.ModifiedDefPow;
				if(chessBuff.CheckDeBuff(BuffType.Defense)){
					if(mapsInRedTerritory.Contains(unit)){
						newDefPower -= 2*potentailDeBuff;
					}else if(mapsInRedTerritory.Count>0){
						newDefPower = chessP.defPower + (int)chessBuff.ExtraDict[BuffType.Defense] + potentailDeBuff;
					}
				}
				
				float distant = Vector3.Distance(gf.position, chess.position);
				int distantUnit = Mathf.RoundToInt(distant/singleUnit); 
				int distantFact = gfp.BuffAtkRange - distantUnit;
				if( distantFact<=0)
					distantFact = 0; 
				int distantFactPoint = distantFact * rangeAffect;
			
				
				int trueDamage = gfNewDamage - newDefPower; 
				if(trueDamage <= 0)
					trueDamage = 0;
				float damageRate = (float)trueDamage/(float)chessP.Hp;
				int point = 0;
				if(!chessP.Summoner)
					point = Mathf.RoundToInt(basicAtkScore*damageRate)- distantFactPoint;
				else
					point = Mathf.RoundToInt(leadingAtkScore*damageRate)- distantFactPoint;
				//Simply count if the map is side one 
				if(currentRC.PlayerATerritory.Contains(map))
					point -= debuffAffect;
				if(!currentRC.PlayerATerritory.Contains(map) && !currentRC.PlayerBTerritory.Contains(map))
					point -= nobuffAffect;
				int damagedScore = DamagedScore(gf, chess);
				point -= damagedScore;
				if(point <=0)
					point = 0;
				pointDict.Add(chess, point);
			}
			var sortedDict = (from entry in pointDict orderby entry.Value descending select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
			tp = new TacticPoint(gf, Tactics.Range_Attack, map, (Transform)sortedDict.ElementAt(0).Key, (int)sortedDict.ElementAt(0).Value);
		}
		//print(gf.name + ": " + tp.Tactic + " " + tp.Point + " " + tp.MapUnit); 
		return tp;
	}*/
	
	int DamagedScore(Transform gf, Transform target, int newDef){
		int score = 0;
		int trueDamage = 0; 
		CharacterProperty gfp = gf.GetComponent<CharacterProperty>();
		CharacterProperty targetP = target.GetComponent<CharacterProperty>();
		//if target cannot fight back score = 0
		
		bool fightBack = IsTargetFightBack(gf, target);
		if(!fightBack)
			score = 0;
		else{
			trueDamage = targetP.Damage - newDef; 
			if(trueDamage <= 0)
				trueDamage = 0;
			else if(trueDamage >=gfp.Hp)
				trueDamage = gfp.Hp;
			float damageRate = (float)trueDamage/(float)gfp.Hp;
			score = Mathf.RoundToInt(basicAtkScore*damageRate);
		}  
		return score;
	}
	
	public bool IsTargetFightBack(Transform attacker, Transform target){
		bool fightBack = false;
		AttackCalFX atkCal = Camera.main.GetComponent<AttackCalFX>();
		IList fightBackList = atkCal.GetAttackableTarget(target);
		if(fightBackList != null && fightBackList.Count>0){
			foreach(Transform nMap in fightBackList){
				Transform ngf = MapHelper.GetMapOccupiedObj(nMap);
				if(ngf == attacker){
					fightBack = true;
					break;
				}else{
					fightBack = false;
				}
			}
		}
		return fightBack;
	}
	
	
	TacticPoint GetSkillPoint(Transform map, Transform gf){
		TacticPoint tp = new TacticPoint(gf, map);
		Transform skill = gf.FindChild("Skills").GetChild(0);
		SkillProperty skillP = skill.GetComponent<SkillProperty>();
		if(skillP.SkillReady){
			CharacterTactics cTactic = gf.GetComponent<CharacterTactics>();
			CommonSkillTP cSkillTp = gf.GetComponent(cTactic.SkillTacticName) as CommonSkillTP;
			if(cSkillTp!=null){
				tp = cSkillTp.GetSkillTacticPoint(map);
			}else{
				tp.Tactic = Tactics.Expend;
				tp.Point = 0;
			}
		}else{
			tp.Tactic = Tactics.Expend;
			tp.Point = 0;
		}
		return tp;
	}
	
	
	TacticPoint GetExpandPoint(Transform map, Transform gf){
		TacticPoint tp = new TacticPoint(gf, Tactics.Expend,map,0);
		CharacterProperty gfp = gf.GetComponent<CharacterProperty>();
		Identity mapID = map.GetComponent<Identity>();
		// AI always plays as player2
		if(!gfp.Summoner && !gfp.LeadingCharacter){
			float mapFact = GetUnitFact(map);
			int expendintPoint = Mathf.RoundToInt(mapFact*unitScore);
			if(!currentRC.PlayerBTerritory.Contains(map)){
				if(mapID.FixedSide == 3)
					expendintPoint += 10;
				else if(mapID.FixedSide==1)
					expendintPoint = 0;
			}
			tp = new TacticPoint(gf, Tactics.Expend, map, expendintPoint);
		}else{
			int expendintPoint = 0;
			IList aroundMaps = new List<Transform>();
			foreach(Transform unit in mapID.Neighbor){
				if(unit != null ){
					Identity unitID = unit.GetComponent<Identity>();
					if(!currentRC.PlayerBTerritory.Contains(unit) && unitID.FixedSide!=1 ){
						aroundMaps.Add(unit);
					}
				}
			}
			if(!currentRC.PlayerBTerritory.Contains(map) && mapID.FixedSide!=1)
				aroundMaps.Add(map);
			foreach(Transform unit in aroundMaps){
				float unitFact = GetUnitFact(unit); 
				int point = Mathf.RoundToInt(unitFact*unitScore);
				expendintPoint += point; 
			}
			tp = new TacticPoint(gf, Tactics.Expend, map, expendintPoint);
		}
		//print(gf.name + ": " + tp.Tactic + " " + tp.Point + " " + tp.MapUnit);
		return tp;
	}
	
	float GetUnitFact(Transform unit){
		float unitFact = 0.0f;
		Identity unitID = unit.GetComponent<Identity>();
		if(currentRC.PlayerATerritory.Contains(unit) && unitID.FixedSide==3){
			unitFact = 1.5f;
			Transform aChess = null;
			if(MapHelper.IsMapOccupied(unit)){
				aChess = MapHelper.GetMapOccupiedObj(unit);
				if(aChess.GetComponent<CharacterProperty>().Player == 1){
					unitFact = 2.3f;
				}else if(aChess.GetComponent<CharacterProperty>().Player == 2){
					unitFact = 2.0f;
				}
			}
		}else if(currentRC.PlayerBTerritory.Contains(unit)){
			unitFact = 0.2f;
		}else if(!currentRC.PlayerATerritory.Contains(unit) && !currentRC.PlayerBTerritory.Contains(unit)){
			unitFact = 1.0f;
			Transform aChess = null;
			if(MapHelper.IsMapOccupied(unit)){
				aChess = MapHelper.GetMapOccupiedObj(unit);
				if(aChess.GetComponent<CharacterProperty>().Player == 1){
					unitFact = 1.6f;
				}else if(aChess.GetComponent<CharacterProperty>().Player == 2){
					unitFact = 1.3f;
				}
			}
		}else{
			unitFact = 0.0f;
		}
		return unitFact;
	}
	
	float GetEmptyUnitFact(Transform unit){
		float unitFact = 0.0f;
		Identity unitID = unit.GetComponent<Identity>();
		if(unitID.FixedSide == 3 && currentRC.PlayerATerritory.Contains(unit)){
			unitFact = 0.5f;
		}else if(currentRC.PlayerBTerritory.Contains(unit)){
			unitFact = 0.1f; 
		}else if(!currentRC.PlayerATerritory.Contains(unit) && !currentRC.PlayerBTerritory.Contains(unit)){
			unitFact = 0.3f; 
		}else{
			unitFact = 0.0f;
		}
		return unitFact; 
	}
	
	int Summonable(int side){
		int summonableNum = 0;
		IList sideGFs = new List<Transform>();
		if(side == 1){
			sideGFs = currentRC.PlayerAChesses;
		}else if(side == 2){
			sideGFs = currentRC.PlayerBChesses;
		}
		foreach(Transform gf in sideGFs){
			if(gf.GetComponent<CharacterProperty>().Ready){
				summonableNum += 1;
			}
		}
		return summonableNum; 
	}

	public IList GetSummonGF(int number){
		IList gfs = new List<Transform>();
		int summonAble = Summonable(2); 
		if(summonAble>0){
			foreach(Transform gf in currentRC.PlayerBChesses){
				if(gf.GetComponent<CharacterProperty>().Ready && gf.GetComponent<CharacterProperty>().Death){
					gfs.Add(gf);
				}
			}
		}
		if(gfs.Count>number){
			for(int i=number;i<gfs.Count; i++){
				gfs.RemoveAt(i);
			}
		}
		return gfs;
	}
	
	public Transform GetSummonPosition(Transform chess){
		IList possibleMaps = new List<Transform>();
		CharacterProperty chessP = chess.GetComponent<CharacterProperty>();
		Transform map = null;
		Dictionary<float, Transform> mapDict = new Dictionary<float, Transform>();
		foreach(Transform m in currentRC.PlayerBTerritory){
			if(!MapHelper.IsMapOccupied(m)){
				possibleMaps.Add(m);
			}
		}
		
		if(MapHelper.CheckTowerOnSite() && chessP.Tower){
			IList mapsToRemove = MapHelper.GetTowerMaps();
			foreach(Transform m in mapsToRemove){
				if(possibleMaps.Contains(m)){
					possibleMaps.Remove(m);
				}
			}
		}
		
		if(possibleMaps.Count>0){
			foreach(Transform unit in possibleMaps){
				TacticPoint tp = GetUnitPoint(unit, chess);
				if(!mapDict.ContainsKey(tp.Point)){
					mapDict.Add(tp.Point, unit);
				}
			}
			var list = mapDict.Keys.ToList();
			list.Sort();
			list.Reverse();
			map = mapDict[list[0]];
		}
		return map; 
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}


public enum Tactics{
	Aid_Other_Attack, 
	Aid_Self_Attack,
	Aid_Other_Def, 
	Aid_Self_Def,
	Aid_Other_Move,
	Aid_Self_Move,
	Aid_Other_Fly,
	Clear_Vision,
	Heal_Other, 
	Heal_Self,
	Expend, 
	Flee,
	Magic_Attack,
	Magic_Range_Attack,
	Melee_Attack,
	Multiple_Arrow,
	Range_Attack, 
	Self_Destruct,
	Steal_Gems, 
	Steal_Map, 
	Summon,
	Switch_Pos,
	none,
}

public struct TacticPoint{
	public Transform MapUnit;
	public Transform Target;
	public Transform Owner;
	public Tactics Tactic; 
	public int Point;
	
	public TacticPoint(Transform owner, Tactics tac, Transform unit, Transform target, int point){
		Owner = owner;
		MapUnit = unit;
		Point = point; 
		Target = target;
		Tactic = tac;
	}
	public TacticPoint(Transform owner, Tactics tac, Transform unit, int point){
		Owner = owner;
		MapUnit = unit;
		Target = null; 
		Point = point; 
		Tactic = tac; 
	}
	public TacticPoint(Transform owner, Transform map){
		Owner = owner;
		MapUnit = map;
		Target = null;
		Point = 0; 
		Tactic = Tactics.none;		
	}
}