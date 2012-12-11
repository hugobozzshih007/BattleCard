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
		
		int GetBuffValue(BuffType type){
			float buffValue = 0.0f;
			RoundCounter rnd = Camera.mainCamera.GetComponent<RoundCounter>();
			Transform allMap = GameObject.Find("Maps").transform;
			int mapUnitNum = allMap.GetChildCount();
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
			return (int)Mathf.Round(buffValue);
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
						case(BuffType.Attack):
							property.Damage = property.atkPower + GetBuffValue(bt);
							break;
						case(BuffType.AttackRange):
							property.BuffAtkRange = property.atkRange + GetBuffValue(bt);
							break;
						case(BuffType.CriticalHit):
							property.BuffCriticalHit = property.CriticalhitChance + GetBuffValue(bt);
							break;
						case(BuffType.Defense):
							if(property.AbleRestore){
								property.ModifiedDefPow = property.defPower + GetBuffValue(bt);
								if(property.Hp == oldDefPower)
									property.Hp = property.ModifiedDefPow;
							}
							break;
						case(BuffType.MoveRange):
							property.BuffMoveRange = property.moveRange + GetBuffValue(bt);
							break;
						case(BuffType.SkillRate):
							property.BuffSkillRate = property.SkillRate + GetBuffValue(bt);
							break;
					}
				}
			}
			
		}
	}
}