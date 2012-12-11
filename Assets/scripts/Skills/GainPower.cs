using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;

public class GainPower : MonoBehaviour, CommonSkill {
	
	RoundCounter chessStorage;
	Transform aider, target;
	public PowerType mode;
	public int Value;
	
	// Use this for initialization
	void Start () {
		aider = transform.parent.parent;
		chessStorage = Camera.main.GetComponent<RoundCounter>();
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
	
	public void Execute ()
	{
		switch(mode){
			case PowerType.Critical:
				target.GetComponent<CharacterProperty>().BuffCriticalHit += Value;
				break;
			case PowerType.Damage:
				target.GetComponent<CharacterProperty>().Damage += Value;
				break;
			case PowerType.Hp:
				target.GetComponent<CharacterProperty>().Hp += Value;
				break;
			case PowerType.SkillRate:
				target.GetComponent<CharacterProperty>().BuffSkillRate += Value;
				break;
			case PowerType.MoveRange:
				target.GetComponent<CharacterProperty>().BuffMoveRange += Value;
				break;
			case PowerType.AttackRange:
				target.GetComponent<CharacterProperty>().BuffAtkRange += Value;
				break;
		}
	}
}
