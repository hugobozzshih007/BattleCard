using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;

public class OneMoreAct : MonoBehaviour, CommonSkill{
	
	int skillRate;
	RoundCounter chessStorage;
	Transform aider, target;
	public CommandType mode;
	
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
			switch(mode){
				case CommandType.Attack:
					if(!property.death && property.Attacked)
						selectionRange.Add(chess.GetComponent<CharacterSelect>().getMapPosition());
					break;
				case CommandType.Move:
					if(!property.death && property.Moved)
						selectionRange.Add(chess.GetComponent<CharacterSelect>().getMapPosition());
					break;
				case CommandType.Skill:
					if(!property.death && property.Activated)
						selectionRange.Add(chess.GetComponent<CharacterSelect>().getMapPosition());
					break;
				
			}
		}
		return selectionRange;
	}
	
	public void Execute ()
	{
		switch(mode){
			case CommandType.Attack:
				target.GetComponent<CharacterProperty>().Attacked = false;
				break;
			case CommandType.Move:
				target.GetComponent<CharacterProperty>().Moved = false;
				break;
			case CommandType.Skill:
				target.GetComponent<CharacterProperty>().Activated = false;
				break;
		}
		target.GetComponent<CharacterProperty>().TurnFinished = false;
	}
}
