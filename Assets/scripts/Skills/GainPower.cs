using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;
using BuffUtility;

public class GainPower : MonoBehaviour, CommonSkill {
	
	RoundCounter chessStorage;
	Transform aider, target;
	Transform fxBuffAtk, fxBuffDef, fxBuffRange, fxBuffCritiq, fxBuffMove, fxBuffSkill;
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
	}
	
	public void InsertSelection (Transform map)
	{
		target = MapHelper.GetMapOccupiedObj(map);
	}
	
	public IList GetSelectionRange ()
	{
		IList selectionRange = new List<Transform>();
		IList playerSide = new List<Transform>();
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
	
	void BuffVisualUI(BuffType type, int val, BuffSlidingUI bSUI){
		Dictionary<BuffType,int> dict = new Dictionary<BuffType, int>();
		dict.Add(type, val);
		BuffUI bUI = new BuffUI(target,dict);
		bSUI.UIItems.Add(bUI);
		bSUI.FadeInUI = true;
	}
	
	public void Execute ()
	{
		BuffCalculation bCal = new BuffCalculation(target);
		BuffSlidingUI bSUI = Camera.mainCamera.GetComponent<BuffSlidingUI>();
		switch(mode){
			case PowerType.Critical:
				target.GetComponent<BuffList>().ExtraDict[BuffType.CriticalHit] += Value;
				BuffVisualUI(BuffType.CriticalHit, Value, bSUI);
				MapHelper.SetFX(target,fxBuffCritiq,4.0f);
				bCal.UpdateBuffValue();
				break;
			case PowerType.Damage:
				target.GetComponent<BuffList>().ExtraDict[BuffType.Attack] += Value;
				BuffVisualUI(BuffType.Attack, Value, bSUI);
				MapHelper.SetFX(target,fxBuffAtk,4.0f);
				bCal.UpdateBuffValue();
				break;
			case PowerType.Hp:
				target.GetComponent<BuffList>().ExtraDict[BuffType.Defense] += Value;
				BuffVisualUI(BuffType.Defense, Value, bSUI);
				MapHelper.SetFX(target,fxBuffDef,4.0f);
				bCal.UpdateBuffValue();
				break;
			case PowerType.SkillRate:
				target.GetComponent<BuffList>().ExtraDict[BuffType.SkillRate] += Value;
				BuffVisualUI(BuffType.SkillRate, Value, bSUI);
				MapHelper.SetFX(target,fxBuffSkill,4.0f);
				bCal.UpdateBuffValue();
				break;
			case PowerType.MoveRange:
				target.GetComponent<BuffList>().ExtraDict[BuffType.MoveRange] += Value;
				BuffVisualUI(BuffType.MoveRange, Value, bSUI);
				MapHelper.SetFX(target,fxBuffMove,4.0f);
				bCal.UpdateBuffValue();
				break;
			case PowerType.AttackRange:
				target.GetComponent<BuffList>().ExtraDict[BuffType.AttackRange] += Value;
				BuffVisualUI(BuffType.AttackRange, Value, bSUI);
				MapHelper.SetFX(target,fxBuffRange,4.0f);
				bCal.UpdateBuffValue();
				break;
		}
	}
}
