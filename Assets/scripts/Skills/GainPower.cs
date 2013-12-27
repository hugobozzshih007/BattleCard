using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;
using BuffUtility;

public class GainPower : MonoBehaviour, SkillInterface {
	
	RoundCounter chessStorage;
	Transform aider, target;
	Transform fxBuffAtk, fxBuffDef, fxBuffRange, fxBuffCritiq, fxBuffMove, fxBuffSkill, fxBuffHp;
	public PowerType mode;
	public int Value;
	
	// Use this for initialization
	void Start () {
		aider = transform.parent.parent;
		chessStorage = Camera.main.GetComponent<RoundCounter>();
		CommonFX cFX  = Camera.main.GetComponent<CommonFX>();
		fxBuffAtk = cFX.BuffAtk;
		fxBuffCritiq = cFX.BuffCritiq;
		fxBuffDef = cFX.BuffDef;
		fxBuffMove = cFX.BuffMove;
		fxBuffRange = cFX.BuffRange;
		fxBuffSkill = cFX.BuffSkill;
		fxBuffHp = cFX.BuffHp;
	}
	
	public void InsertSelection (Transform map)
	{
		target = MapHelper.GetMapOccupiedObj(map);
	}
	
	public IList GetSelectionRange ()
	{
		IList selectionRange = new List<Transform>();
		IList playerSide = new List<Transform>();
		aider = transform.parent.parent;
		int player = aider.GetComponent<CharacterProperty>().Player;
		if(player==1)
			playerSide = chessStorage.PlayerAChesses;
		else if(player==2)
			playerSide = chessStorage.PlayerBChesses;
			
		foreach(Transform chess in playerSide){
			CharacterProperty property = chess.GetComponent<CharacterProperty>();
			if(!property.death){
				selectionRange.Add(chess.GetComponent<CharacterSelect>().getMapPosition());
			}
		}
		return selectionRange;
	}
	
	void BuffVisualUI(BuffType type, int val){
		Dictionary<BuffType,int> dict = new Dictionary<BuffType, int>();
		dict.Add(type, val);
		BuffSlidingFX targetBFX = target.GetComponent<BuffSlidingFX>();
		targetBFX.ActiveBuffSlidingFX(dict);
	}
	
	public void Execute ()
	{
		BuffCalculation bCal = new BuffCalculation(target);
		
		Transform model = transform.parent.parent.Find("Models");
		ActivateSkillFX asf = model.GetComponent<ActivateSkillFX>();
		switch(mode){
			case PowerType.Critical:
				target.GetComponent<BuffList>().ExtraDict[BuffType.CriticalHit] += Value;
				BuffVisualUI(BuffType.CriticalHit, Value);
				MapHelper.SetFX(target,fxBuffCritiq,4.0f);
				bCal.UpdateBuffValue();
				break;
			case PowerType.Damage:
				target.GetComponent<BuffList>().ExtraDict[BuffType.Attack] += Value;
				BuffVisualUI(BuffType.Attack, Value);
				MapHelper.SetFX(target,fxBuffAtk,4.0f);
				bCal.UpdateBuffValue();
				break;
			case PowerType.Defense:
				target.GetComponent<BuffList>().ExtraDict[BuffType.Defense] += Value;
				BuffVisualUI(BuffType.Defense, Value);
				MapHelper.SetFX(target,fxBuffDef,4.0f);
				bCal.UpdateBuffValue();
				break;
			case PowerType.Hp:
				CharacterProperty targetP = target.GetComponent<CharacterProperty>();
				if(targetP.Hp < targetP.MaxHp){
					int diffHp = targetP.MaxHp - targetP.Hp; 
					if(diffHp >= Value){ 
						targetP.Hp += Value;
						asf.InsertPowerValue(Value);
					}else{ 
					    targetP.Hp += diffHp;
						asf.InsertPowerValue(diffHp);
					}
				}
				asf.InsertTarget(target);
				break;
			case PowerType.SkillRate:
				target.GetComponent<BuffList>().ExtraDict[BuffType.SkillRate] += Value;
				BuffVisualUI(BuffType.SkillRate, Value);
				MapHelper.SetFX(target,fxBuffSkill,4.0f);
				bCal.UpdateBuffValue();
				break;
			case PowerType.MoveRange:
				target.GetComponent<BuffList>().ExtraDict[BuffType.MoveRange] += Value;
				BuffVisualUI(BuffType.MoveRange, Value);
				MapHelper.SetFX(target,fxBuffMove,4.0f);
				bCal.UpdateBuffValue();
				break;
			case PowerType.AttackRange:
				target.GetComponent<BuffList>().ExtraDict[BuffType.AttackRange] += Value;
				BuffVisualUI(BuffType.AttackRange, Value);
				MapHelper.SetFX(target,fxBuffRange,4.0f);
				bCal.UpdateBuffValue();
				break;
		}
	}
}
