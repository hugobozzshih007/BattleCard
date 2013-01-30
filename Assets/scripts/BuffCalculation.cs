using UnityEngine;
using System.Collections;

namespace BuffUtility{
	public class BuffCalculation{
		Transform Character;
		BuffType[] addBuff;
		BuffType[] deBuff;
		CharacterSelect selection;
		CharacterProperty property;
		BuffList buffList;
		int playerSide;
		
		enum BuffUsage{
			Intensify,
			None,
			Decrease,
		}
		
		BuffUsage usage;
		
		public BuffCalculation(Transform character){
			Character = character;
			property = character.GetComponent<CharacterProperty>();
			buffList = character.GetComponent<BuffList>();
			selection = character.GetComponent<CharacterSelect>();
			addBuff = (BuffType[])buffList.addBuff.Clone();
			deBuff = (BuffType[])buffList.deBuff.Clone();
			playerSide = property.Player;
		}
		
		void CalBuffUsage(){
			BuffUsage bUsage = BuffUsage.None;
			int locationSide = 0;
			Transform location = selection.getMapPosition();
			RoundCounter rnd = Camera.mainCamera.GetComponent<RoundCounter>();
			if(rnd.PlayerATerritory.Contains(location))
				locationSide = 1;
			else if(rnd.PlayerBTerritory.Contains(location))
				locationSide = 2;
			else
				locationSide = 0;
			
			if(playerSide ==1){
				switch(locationSide){
					case(1):
						bUsage = BuffUsage.Intensify;
						break;
					case(2):
						bUsage = BuffUsage.Decrease;
						break;
					case(0):
						bUsage = BuffUsage.None;
						break;
				}
			}else if(playerSide ==2){
				switch(locationSide){
					case(1):
						bUsage = BuffUsage.Decrease;
						break;
					case(2):
						bUsage = BuffUsage.Intensify;
						break;
					case(0):
						bUsage = BuffUsage.None;
						break;
				}
			}
			usage = bUsage;
		}
		
		public static int BuffXValue(int rate){
			int buff = 0;
			if(rate >= 25 && rate < 50){
				buff = 1;
			}else if(rate >= 50 && rate < 75){
				buff = 2;
			}else if(rate > 75){
				buff = 3;
			}
			
			return buff;
		}
		
		public static int BuffRateValue(int rate){
			int buff = 0;
			if(rate >= 10 && rate < 25){
				buff = 5;
			}else if(rate >= 25 && rate < 36){
				buff = 10;
			}else if(rate >= 36 && rate < 50){
				buff = 17;
			}else if(rate >= 50 && rate < 66){
				buff = 30;
			}else if(rate >= 66 && rate > 75){
				buff = 45;
			}else if(rate >= 75){
				buff = 60;
			}
			return buff;
		}
		
		int GetBuffValue(BuffType type){
			int buffValue = 0;
			RoundCounter rnd = Camera.mainCamera.GetComponent<RoundCounter>();
			Transform allMap = GameObject.Find("Maps").transform;
			int mapUnitNum = allMap.childCount;
			int rate = 0;
			if(playerSide == 1)
				rate = Mathf.RoundToInt((float)rnd.PlayerATerritory.Count / (float)mapUnitNum * 100.0f);
			else if(playerSide == 2)
				rate = Mathf.RoundToInt((float)rnd.PlayerBTerritory.Count / (float)mapUnitNum * 100.0f);
			
			switch(type){
				case BuffType.Attack:
					buffValue = BuffXValue(rate);
					break;
				case BuffType.Defense:
					buffValue = BuffXValue(rate);
					break;
				case BuffType.AttackRange:
					buffValue = BuffXValue(rate);
					break;
				case BuffType.MoveRange:
					buffValue = BuffXValue(rate);
					break;
				case BuffType.CriticalHit:
					buffValue = BuffRateValue(rate);
					break;
				case BuffType.SkillRate:
					buffValue = BuffRateValue(rate);
					break;
			}
			
			
			if(usage == BuffUsage.Intensify)
				buffValue = buffValue;
			else if(usage == BuffUsage.None)
				buffValue = 0;
			else if(usage == BuffUsage.Decrease)
				buffValue = -buffValue;
			
			/*
			switch(type){
				case(BuffType.Attack):
					if(playerSide==1){
						if(usage == BuffUsage.Intensify)
							buffValue = ((float)rnd.PlayerATerritory.Count/mapUnitNum)*2*Mathf.Pow((float)property.atkPower,0.1f);
						else if(usage == BuffUsage.Decrease)
							buffValue = -((float)rnd.PlayerATerritory.Count/mapUnitNum)*2*Mathf.Pow((float)property.atkPower,0.1f);
						else
							buffValue = 0.0f;
					}else if(playerSide==2){
						if(usage == BuffUsage.Intensify)
							buffValue = ((float)rnd.PlayerBTerritory.Count/mapUnitNum)*2*Mathf.Pow((float)property.atkPower,0.1f);
						else if(usage == BuffUsage.Decrease)
							buffValue = -((float)rnd.PlayerBTerritory.Count/mapUnitNum)*2*Mathf.Pow((float)property.atkPower,0.1f);
						else
							buffValue = 0.0f;
					}	
					break;
				case(BuffType.AttackRange):
					if(playerSide==1){
						if(usage == BuffUsage.Intensify)
							buffValue = ((float)rnd.PlayerATerritory.Count/mapUnitNum)*2*Mathf.Pow((float)property.atkRange,0.1f);
						else if(usage == BuffUsage.Decrease)
							buffValue = -((float)rnd.PlayerATerritory.Count/mapUnitNum)*2*Mathf.Pow((float)property.atkRange,0.1f);
						else
							buffValue = 0.0f;
					}else if(playerSide==2){
						if(usage == BuffUsage.Intensify)
							buffValue = ((float)rnd.PlayerBTerritory.Count/mapUnitNum)*2*Mathf.Pow((float)property.atkRange,0.1f);
						else if(usage == BuffUsage.Decrease)
							buffValue = -((float)rnd.PlayerBTerritory.Count/mapUnitNum)*2*Mathf.Pow((float)property.atkRange,0.1f);
						else
							buffValue = 0.0f;
					}	
					break;
				case(BuffType.CriticalHit):
					if(playerSide==1){
						if(usage == BuffUsage.Intensify)
							buffValue = Mathf.Pow((float)rnd.PlayerATerritory.Count/mapUnitNum*10.0f,0.2f)*(float)property.CriticalhitChance;
						else if(usage == BuffUsage.Decrease)
							buffValue = -Mathf.Pow((float)rnd.PlayerATerritory.Count/mapUnitNum*10.0f,0.2f)*(float)property.CriticalhitChance;
						else
							buffValue = 0.0f;
					}else if(playerSide==2){
						if(usage == BuffUsage.Intensify)
							buffValue = Mathf.Pow((float)rnd.PlayerBTerritory.Count/mapUnitNum*10.0f,0.2f)*(float)property.CriticalhitChance;
						else if(usage == BuffUsage.Decrease)
							buffValue = -Mathf.Pow((float)rnd.PlayerBTerritory.Count/mapUnitNum*10.0f,0.2f)*(float)property.CriticalhitChance;
						else
							buffValue = 0.0f;
					}	
					break;
				case(BuffType.Defense):
					if(playerSide==1){
						if(usage == BuffUsage.Intensify)
							buffValue = ((float)rnd.PlayerATerritory.Count/mapUnitNum)*2*Mathf.Pow((float)property.defPower,0.1f);
						else if(usage == BuffUsage.Decrease)
							buffValue = -((float)rnd.PlayerATerritory.Count/mapUnitNum)*2*Mathf.Pow((float)property.defPower,0.1f);
						else
							buffValue = 0.0f;
					}else if(playerSide==2){
						if(usage == BuffUsage.Intensify)
							buffValue = ((float)rnd.PlayerBTerritory.Count/mapUnitNum)*2*Mathf.Pow((float)property.defPower,0.1f);
						else if(usage == BuffUsage.Decrease)
							buffValue = -((float)rnd.PlayerBTerritory.Count/mapUnitNum)*2*Mathf.Pow((float)property.defPower,0.1f);
						else
							buffValue = 0.0f;
					}	
					break;
				case(BuffType.MoveRange):
					if(playerSide==1){
						if(usage == BuffUsage.Intensify)
							buffValue = ((float)rnd.PlayerATerritory.Count/mapUnitNum)*2*Mathf.Pow((float)property.moveRange,0.1f);
						else if(usage == BuffUsage.Decrease)
							buffValue = -((float)rnd.PlayerATerritory.Count/mapUnitNum)*2*Mathf.Pow((float)property.moveRange,0.1f);
						else
							buffValue = 0.0f;
					}else if(playerSide==2){
						if(usage == BuffUsage.Intensify)
							buffValue = ((float)rnd.PlayerBTerritory.Count/mapUnitNum)*2*Mathf.Pow((float)property.moveRange,0.1f);
						else if(usage == BuffUsage.Decrease)
							buffValue = -((float)rnd.PlayerBTerritory.Count/mapUnitNum)*2*Mathf.Pow((float)property.moveRange,0.1f);
						else
							buffValue = 0.0f;
					}	
					break;
				case(BuffType.SkillRate):
					if(playerSide==1){
						if(usage == BuffUsage.Intensify)
							buffValue = Mathf.Pow((float)rnd.PlayerATerritory.Count/mapUnitNum*10.0f,0.2f)*(float)property.SkillRate;
						else if(usage == BuffUsage.Decrease)
							buffValue = -Mathf.Pow((float)rnd.PlayerATerritory.Count/mapUnitNum*10.0f,0.2f)*(float)property.SkillRate;
						else
							buffValue = 0.0f;
					}else if(playerSide==2){
						if(usage == BuffUsage.Intensify)
							buffValue = Mathf.Pow((float)rnd.PlayerATerritory.Count/mapUnitNum*10.0f,0.2f)*(float)property.SkillRate;
						else if(usage == BuffUsage.Decrease)
							buffValue = -Mathf.Pow((float)rnd.PlayerATerritory.Count/mapUnitNum*10.0f,0.2f)*(float)property.SkillRate;
						else
							buffValue = 0.0f;
					}	
					break;
			}
			*/
			return buffValue;
		}
		
		
		public void UpdateBuffValue(){
			CalBuffUsage();
			BuffType[] bList = null;
			int oldDefPower = property.ModifiedDefPow;
			if(usage==BuffUsage.Intensify){
				bList = addBuff;
				foreach(var pair in buffList.AddBuffDict){
					if(pair.Value == false){
						switch(pair.Key){
							case BuffType.Attack:
								property.Damage = property.atkPower;
								break;
							case BuffType.AttackRange:
								property.BuffAtkRange = property.atkRange;
								break;
							case BuffType.CriticalHit:
								property.BuffCriticalHit = property.CriticalhitChance;
								break;
							case BuffType.Defense:
								if(property.AbleRestore){
									property.ModifiedDefPow = property.defPower;
									if(property.Hp == oldDefPower)
										property.Hp = property.ModifiedDefPow;
								}
								break;
							case BuffType.MoveRange:
								property.BuffMoveRange = property.moveRange;
								break;
							case BuffType.SkillRate:
								property.BuffSkillRate = property.SkillRate;
								break;
						}
					}
				}
			}else if(usage==BuffUsage.Decrease){
				bList = deBuff;
				foreach(var pair in buffList.DeBuffDict){
					if(pair.Value == false){
						switch(pair.Key){
							case BuffType.Attack:
								property.Damage = property.atkPower;
								break;
							case BuffType.AttackRange:
								property.BuffAtkRange = property.atkRange;
								break;
							case BuffType.CriticalHit:
								property.BuffCriticalHit = property.CriticalhitChance;
								break;
							case BuffType.Defense:
								if(property.AbleRestore){
									property.ModifiedDefPow = property.defPower;
									if(property.Hp == oldDefPower)
										property.Hp = property.ModifiedDefPow;
								}
								break;
							case BuffType.MoveRange:
								property.BuffMoveRange = property.moveRange;
								break;
							case BuffType.SkillRate:
								property.BuffSkillRate = property.SkillRate;
								break;
						}
					}
				}
			}else{
				property.Damage = property.atkPower;
				if(property.AbleRestore){
					property.ModifiedDefPow = property.defPower;
					if(property.Hp == oldDefPower)
						property.Hp = property.ModifiedDefPow;
				}
				property.BuffMoveRange = property.moveRange;
				property.BuffAtkRange = property.atkRange;
				property.BuffCriticalHit = property.CriticalhitChance;
				property.BuffSkillRate = property.SkillRate;
			}
			if(bList!=null){
				foreach(BuffType bt in bList){
					switch(bt){
						case BuffType.Attack:
							property.Damage = property.atkPower + GetBuffValue(bt);
							break;
						case BuffType.AttackRange:
							property.BuffAtkRange = property.atkRange + GetBuffValue(bt);
							break;
						case BuffType.CriticalHit:
							property.BuffCriticalHit = property.CriticalhitChance + GetBuffValue(bt);
							break;
						case BuffType.Defense:
							if(property.AbleRestore){
								property.ModifiedDefPow = property.defPower + GetBuffValue(bt);
								if(property.Hp == oldDefPower)
									property.Hp = property.ModifiedDefPow;
							}
							break;
						case BuffType.MoveRange:
							property.BuffMoveRange = property.moveRange + GetBuffValue(bt);
							break;
						case BuffType.SkillRate:
							property.BuffSkillRate = property.SkillRate + GetBuffValue(bt);
							break;
					}
				}
			}
			oldDefPower = property.ModifiedDefPow;
			property.Damage += buffList.ExtraDict[BuffType.Attack];
			property.ModifiedDefPow += buffList.ExtraDict[BuffType.Defense];
			if(property.Hp == oldDefPower)
				property.Hp = property.ModifiedDefPow;
			property.BuffMoveRange += buffList.ExtraDict[BuffType.MoveRange];
			property.BuffAtkRange += buffList.ExtraDict[BuffType.AttackRange];
			property.BuffCriticalHit += buffList.ExtraDict[BuffType.CriticalHit];
			property.BuffSkillRate += buffList.ExtraDict[BuffType.SkillRate];
			
			if(property.Hp < 1)
				property.Hp = 1;
			if(property.BuffSkillRate < 0)
				property.BuffSkillRate = 0;
			if(property.BuffCriticalHit < 0)
				property.BuffCriticalHit = 0;
			if(property.Damage < 0)
				property.Damage = 0;
			if(property.BuffMoveRange < 1)
				property.BuffMoveRange = 1;
			if(property.BuffAtkRange < 1)
				property.BuffAtkRange = 1;
				
		}
	}
}