using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;

public class OneMoreAct : MonoBehaviour, SkillInterface{
	
	int skillRate;
	RoundCounter chessStorage;
	Transform aider, target;
	//public CommandType mode;
	
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
		int player = transform.parent.parent.GetComponent<CharacterProperty>().Player;
		if(player==1)
			playerSide = chessStorage.PlayerAChesses;
		else if(player==2)
			playerSide = chessStorage.PlayerBChesses;
			
		foreach(Transform chess in playerSide){
			CharacterProperty property = chess.GetComponent<CharacterProperty>();
			if(property.CmdTimes<3){
				selectionRange.Add(chess.GetComponent<CharacterSelect>().getMapPosition());
			}
		}
		return selectionRange;
	}
	
	public void Execute ()
	{
		Transform model = transform.parent.parent.Find("Models");
		ActivateSkillFX asf = model.GetComponent<ActivateSkillFX>(); 
		asf.InsertPowerValue(1);
		asf.InsertTarget(target);
		CharacterProperty targetP = target.GetComponent<CharacterProperty>();
		targetP.CmdTimes += 1;
		targetP.Attacked = false;
		targetP.TurnFinished = false;
	}
}
