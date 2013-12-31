using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;
using System.Linq;

public class Decisions : MonoBehaviour {
	GeneralSelection currentSelect; 
	RoundCounter currentRC;
	// Use this for initialization
	void Start () {
		currentSelect = Camera.main.GetComponent<GeneralSelection>();
		currentRC = Camera.main.GetComponent<RoundCounter>();
	}
	
	public UICommands GetFirstCommand(Transform chess){
		UICommands uiCmds = UICommands.none;
		CharacterProperty chessProperty = chess.GetComponent<CharacterProperty>();
		if(chessProperty.Summoner){
			if(MapHelper.Attackable(chess)){
				if(KillAble(chess,0, chessProperty.Damage)){
					uiCmds = UICommands.Attack;
				}else{
					uiCmds = UICommands.Defense;
				}
			}else{
				uiCmds = UICommands.Move;
				Transform map = GetMoveTarget(chess);
				if(map==null){
					uiCmds = UICommands.Defense;
				}
			}
		}else{
			if(MapHelper.Attackable(chess)){
				if(KillAble(chess, 0, chessProperty.Damage) || KillAble(chess, 1, 0)){
					uiCmds = UICommands.Attack;
				}else{
					uiCmds = UICommands.none;
				}
			}else{
				uiCmds = UICommands.Move;
			}
			
			if(uiCmds == UICommands.none){
				Transform[] skills = chess.GetComponent<SkillSets>().Skills;
				foreach(Transform skill in skills){
					if(currentSelect.player.GetComponent<ManaCounter>().Mana>=skill.GetComponent<SkillProperty>().SkillCost){
						SkillProperty skillProperty = skill.GetComponent<SkillProperty>();
						if(skillProperty.SType == SkillType.EnhanceSelf && skillProperty.Mode == PowerType.Damage){
							int newDamage = chessProperty.Damage + skillProperty.ModeValue;  
							if(KillAble(chess, 0, newDamage))
								uiCmds = UICommands.Skill;
							else
								uiCmds = UICommands.Move;
						}else{
							uiCmds = UICommands.Move;
						}
					}else{
						uiCmds = UICommands.Move;
					}
				}
			}
		}
		if(chessProperty.Tower)
			uiCmds = UICommands.none;
		return uiCmds;
	}
	
	public UICommands GetSecondCommand(Transform chess){
		UICommands uiCmds = UICommands.none;
		CharacterProperty chessP = chess.GetComponent<CharacterProperty>();
		AIMoveStore chessAI = new AIMoveStore(chess);
		if(chessAI.CmdsUsed.Count<=1){
			if(chessP.Summoner){
				if(chessAI.CmdsLeft.Contains(UICommands.Attack) && MapHelper.Attackable(chess)){
					if(KillAble(chess,0, chessP.Damage)){
						uiCmds = UICommands.Attack;
					}else if(chessAI.CmdsLeft.Contains(UICommands.Defense)){
						uiCmds = UICommands.Defense;
					}
				}else if(chessAI.CmdsUsed.Contains(UICommands.Move)){
					uiCmds = UICommands.Defense;
				}else if(chessAI.CmdsUsed.Contains(UICommands.Defense)){
					//check if summonable 
					Transform summonGF = GetSummonGF(chess);
					Transform summonPos = GetSummonPosition(chess);
					if(summonGF!=null && summonPos!=null)
						uiCmds = UICommands.Summon;
				}
			}else{
				if(MapHelper.Attackable(chess)){
					if(KillAble(chess, 0, chessP.Damage) || KillAble(chess, 1, 0)){
						if(chessAI.CmdsLeft.Contains(UICommands.Attack))
							uiCmds = UICommands.Attack;
					}else{
						Transform[] skills = chess.GetComponent<SkillSets>().Skills;
						foreach(Transform skill in skills){
							if(currentSelect.player.GetComponent<ManaCounter>().Mana>=skill.GetComponent<SkillProperty>().SkillCost){
								SkillProperty skillProperty = skill.GetComponent<SkillProperty>();
								if(skillProperty.SType == SkillType.EnhanceSelf && skillProperty.Mode == PowerType.Damage){
									int newDamage = chessP.Damage + skillProperty.ModeValue;  
									if(KillAble(chess, 0, newDamage) && chessAI.CmdsLeft.Contains(UICommands.Skill))
										uiCmds = UICommands.Skill;
									else if(chessAI.CmdsLeft.Contains(UICommands.Move))
										uiCmds = UICommands.Move;
								}else if(chessAI.CmdsLeft.Contains(UICommands.Move)){
									uiCmds = UICommands.Move;
								}
							}else if(chessAI.CmdsLeft.Contains(UICommands.Move)){
								uiCmds = UICommands.Move;
							}
						}
					}
				}
			}
		}else{
			print("what a fuck, it's not second move");
		}
		if(chessP.Tower)
			uiCmds = UICommands.none;
		return uiCmds;
	}
	
	public UICommands GetThirdCommand(Transform chess){
		UICommands uiCmds = UICommands.none;
		CharacterProperty chessP = chess.GetComponent<CharacterProperty>();
		AIMoveStore chessAI = new AIMoveStore(chess);
		if(chessP.Summoner){
			if(chessAI.CmdsLeft.Contains(UICommands.Attack) && MapHelper.Attackable(chess)){
				if(KillAble(chess,0, chessP.Damage)){
					uiCmds = UICommands.Attack;
				}else if(chessAI.CmdsLeft.Contains(UICommands.Defense)){
					uiCmds = UICommands.Defense;
				}
			}else if(chessAI.CmdsUsed.Contains(UICommands.Move)){
				uiCmds = UICommands.Defense;
			}else if(chessAI.CmdsUsed.Contains(UICommands.Defense) || chessAI.CmdsLeft.Contains(UICommands.Summon)){
				//check if summonable 
				Transform summonGF = GetSummonGF(chess);
				Transform summonPos = GetSummonPosition(chess);
				if(summonGF!=null && summonPos!=null)
					uiCmds = UICommands.Summon;
			}
		}else{
			if(MapHelper.Attackable(chess)){
				if(KillAble(chess, 0, chessP.Damage) || KillAble(chess, 1, 0)){
					if(!chessAI.CmdsUsed.Contains(UICommands.Attack))
						uiCmds = UICommands.Attack;
				}else if(chessAI.CmdsLeft.Contains(UICommands.Defense)){
					uiCmds = UICommands.Defense;
				}else if(chessAI.CmdsLeft.Contains(UICommands.Move)){
					uiCmds = UICommands.Move;
				}
			}	
		}
		if(chessP.Tower)
			uiCmds = UICommands.none;
		return uiCmds;
	}
	
	public Transform GetSkill(Transform chess){
		Transform theSkill = null;
		Transform[] skills = chess.GetComponent<SkillSets>().Skills;
		foreach(Transform skill in skills){
			SkillProperty skillProperty = skill.GetComponent<SkillProperty>();
			if(skillProperty.SType == SkillType.EnhanceSelf){
				theSkill = skill;
				break;
			}
		}
		return theSkill;  
	}
	
	public Transform GetRevivePos(){
		Transform targetMap = null;
		IList pPosList = new List<Transform>();
		pPosList = currentSelect.GetAllEmptyMaps();
		IList farList = new List<Transform>();
		farList = MapHelper.GetFarestMaps(currentRC.playerA, pPosList);
		int rnd = Random.Range(0, farList.Count-1);
		targetMap = farList[rnd] as Transform;
		return targetMap;
	}
	
	public Transform GetAttackTarget(Transform chess){
		Transform target = null;
		IList attackableList = new List<Transform>();
		CharacterProperty chessProperty = chess.GetComponent<CharacterProperty>();
		AttackCalculation aCal = new AttackCalculation(chess);
		attackableList = aCal.GetAttableTarget(aCal.Attacker);
		if(attackableList.Count > 0){
			foreach(Transform map in attackableList){
				Transform gf = MapHelper.GetMapOccupiedObj(map);
				if(chessProperty.Damage >= gf.GetComponent<CharacterProperty>().Hp){
					target = gf;
					break;
				}else{
					target = null;
				}
			}
			if(target == null){
				foreach(Transform map in attackableList){
					Transform gf = MapHelper.GetMapOccupiedObj(map);
					AttackCalculation atkCal = new AttackCalculation(gf);
					IList fightBackList = atkCal.GetAttableTarget(atkCal.Attacker);
					if(fightBackList != null && fightBackList.Count>0){
						bool fightBack = false;
						
						foreach(Transform nMap in fightBackList){
							Transform ngf = MapHelper.GetMapOccupiedObj(nMap);
							if(ngf == chess){
								fightBack = true;
								break;
							}else{
								fightBack = false;
							}
						}
						if(!fightBack){
							target = gf; 
							break;
						}else{
							target = null;
						}
					}else{
						continue;
					}
				}
			}
			if(target == null){
				foreach(Transform map in attackableList){
					Transform gf = MapHelper.GetMapOccupiedObj(map);
					if(chessProperty.Hp > gf.GetComponent<CharacterProperty>().Damage){
						target = gf;
						break;
					}
				}
			}
		}
		return target;
	}
	
	
	public Transform GetMoveTarget(Transform chess){
		IList mapList = new List<Transform>();
		IList selfTerritory = new List<Transform>();
		IList closeList = new List<Transform>();
		Transform targetMap = null;
		CharacterSelect chessSel = chess.GetComponent<CharacterSelect>();
		currentSelect.updateMapSteps();
		Transform currentPos = chessSel.getMapPosition();
		chessSel.findMoveRange(currentPos,0,chess.GetComponent<CharacterProperty>().BuffMoveRange);
		foreach(Transform map in chessSel.MoveRangeList){
			if(!MapHelper.IsMapOccupied(map))
				mapList.Add(map);
		}
		chessSel.MoveRangeList.Clear();
		
		foreach(Transform map in currentRC.PlayerBTerritory){
			if(!MapHelper.IsMapOccupied(map))
				selfTerritory.Add(map);
		}
		
		Transform closestOne = Camera.main.GetComponent<MoveCharacter>().GetClosetChess(chess);
		closeList = MapHelper.GetClosestMaps(closestOne, mapList);
		//print("The Closest One: "+closestOne);
		if(closeList.Count == 1){
			targetMap = closeList[0] as Transform;
		}else if(closeList.Count > 1){
			Transform[] pPos = new Transform[selfTerritory.Count];
			selfTerritory.CopyTo(pPos, 0);
			Transform[] closestPos = new Transform[closeList.Count];
			closeList.CopyTo(closestPos, 0);
			IEnumerable<Transform> bothPos = pPos.Intersect(closestPos);
			Transform[] bothArray = bothPos.ToArray();
			if(bothArray.Length == 1){
				targetMap = bothArray[0] as Transform;
			}else if(bothArray.Length > 1){
				int rnd = Random.Range(0, bothArray.Length-1);
				targetMap = bothArray[rnd] as Transform;
			}else if(bothArray.Length == 0){
				int rnd = Random.Range(0, closeList.Count-1);
				targetMap = closeList[rnd] as Transform;
			}
		}
		
		return targetMap;
	}
	
	public Transform GetSummonGF(Transform chess){
		Transform summonGF = null;
		IList soldiers = new List<Transform>();
		IList qSoldiers = new List<Transform>();
		foreach(Transform gf in currentRC.PlayerBChesses){
			if(!gf.GetComponent<CharacterProperty>().Summoner)
				soldiers.Add(gf);
		}
		foreach(Transform gf in soldiers){
			CharacterProperty gfp = gf.GetComponent<CharacterProperty>();
			if(gfp.Ready && (gfp.summonCost<=currentSelect.player.GetComponent<ManaCounter>().Mana) && gfp.death)
				qSoldiers.Add(gf);
		}
		
		if(qSoldiers.Count>0){
			Dictionary<int,Transform> sortDict = new Dictionary<int, Transform>();
			foreach(Transform gf in qSoldiers){
				if(!sortDict.ContainsKey(gf.GetComponent<CharacterProperty>().summonCost))
					sortDict.Add(gf.GetComponent<CharacterProperty>().summonCost, gf);
			}
			var list = sortDict.Keys.ToList();
			list.Sort();
			summonGF = sortDict[list[0]];
		}else{
			summonGF = null;
		}
		return summonGF;
	}
	
	public Transform GetSummonPosition(Transform chess){
		CharacterProperty chessP = chess.GetComponent<CharacterProperty>();
		Transform summonPos = null;
		IList possiblePos = new List<Transform>();
		foreach(Transform map in currentRC.PlayerBTerritory){
			if(!MapHelper.IsMapOccupied(map))
				possiblePos.Add(map);
		}
		Transform[] pPos = new Transform[possiblePos.Count];
		possiblePos.CopyTo(pPos,0);
		
		Transform currentPos = chess.GetComponent<CharacterSelect>().getMapPosition();
		Transform[] closestPos = currentPos.GetComponent<Identity>().Neighbor;
		
		int ran = Random.Range(0, pPos.Length-1);
		
		summonPos = pPos[ran];
		/*
		IEnumerable<Transform> bothPos = pPos.Intersect(closestPos);
		IList interSect = new List<Transform>();
		if(bothPos.ToArray().Length>0){
			foreach(Transform map in bothPos){
				if(!MapHelper.IsMapOccupied(map)){
					interSect.Add(map);
				}
			}
			
			if(interSect.Count>0){
				int rnd = Random.Range(0,interSect.Count-1);
				summonPos = interSect[rnd] as Transform;
			}
			
			if(summonPos==null){
				IList pPosList = new List<Transform>();
				foreach(Transform g in pPos){
					pPosList.Add(g);
				}
				foreach(Transform map in bothPos){
					pPosList.Remove(map);
				}
				summonPos = MapHelper.GetClosestMap(chess, pPosList);
			}
		}
		*/
		return summonPos;
	}
	
	bool KillAble(Transform chess, int mode, int damage){
		bool killSomeone = false;
		IList attackableList = new List<Transform>();
		CharacterProperty chessProperty = chess.GetComponent<CharacterProperty>();
		AttackCalculation aCal = new AttackCalculation(chess);
		attackableList = aCal.GetAttableTarget(aCal.Attacker);
		if(mode == 0){
			foreach(Transform map in attackableList){
				Transform target = MapHelper.GetMapOccupiedObj(map);
				CharacterProperty targetProperty = target.GetComponent<CharacterProperty>();
				if(damage >= targetProperty.Hp){
					killSomeone = true;
					break;
				}else{
					killSomeone = false;
				}
			}
		}else if(mode == 1){
			foreach(Transform map in attackableList){
				Transform target = MapHelper.GetMapOccupiedObj(map);
				AttackCalculation atkCal = new AttackCalculation(target);
				IList fightBackList = atkCal.GetAttableTarget(atkCal.Attacker);
				if(fightBackList != null && fightBackList.Count>0){
					foreach(Transform mapX in fightBackList){
						Transform gf = MapHelper.GetMapOccupiedObj(mapX);
						if(gf == chess){
							killSomeone = false;
							break;
						}else{
							killSomeone = true;
						}
					}
				}else{
					killSomeone = true;
				}
			}
		}
		return killSomeone;
	}
	// Update is called once per frame
	void Update () {
	
	}
}
