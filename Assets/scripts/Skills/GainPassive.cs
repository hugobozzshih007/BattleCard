using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;

public class GainPassive : MonoBehaviour, SkillInterface{
	
	public PassiveType mode;
	int skillRate;
	RoundCounter chessStorage;
	Transform target;
	
	// Use this for initialization
	void Start () {
		//aider = transform.parent;
		chessStorage = Camera.main.GetComponent<RoundCounter>();
	}
	
	public void InsertSelection (Transform map)
	{
		target = MapHelper.GetMapOccupiedObj(map);
	}
	
	public IList GetSelectionRange ()
	{
		IList selectionRange = new List<Transform>();
		foreach(Transform chess in chessStorage.AllChesses){
			CharacterProperty property = chess.GetComponent<CharacterProperty>();
			if(!property.Death){
				selectionRange.Add(chess.GetComponent<CharacterSelect>().getMapPosition());
			}
		}
		return selectionRange;
	}
	
	public void Execute ()
	{
		CharacterPassive targetPassive = target.GetComponent<CharacterPassive>();
		targetPassive.PassiveDict[mode] = true;
	}
}
