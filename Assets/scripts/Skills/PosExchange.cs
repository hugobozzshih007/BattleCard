using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;

public class PosExchange :MonoBehaviour, CommonSkill{
	
	int skillRate;
	RoundCounter chessStorage;
	Transform attacker, selectedMap, target;
	
	// Use this for initialization
	void Start () {
		attacker = transform.parent.parent;
		chessStorage = Camera.main.GetComponent<RoundCounter>();
	}
	public void InsertSelection(Transform map){
		selectedMap = map;
		target = MapHelper.GetMapOccupiedObj(map);
	}
	
    public IList GetSelectionRange ()
	{
		IList selectionRange = new List<Transform>();
		foreach(Transform chess in chessStorage.AllChesses){
			CharacterProperty property = chess.GetComponent<CharacterProperty>();
			if((chess != attacker) && !property.death){
				selectionRange.Add(chess.GetComponent<CharacterSelect>().getMapPosition());
			}
		}
		return selectionRange;
	}
	
	public void Execute ()
	{
		if(transform.GetComponent<SkillProperty>().PassSkillRate){
			Vector3 oldAtkPos = attacker.position;
			Vector3 oldTargetPos = target.position;
			attacker.position = oldTargetPos;
			target.position = oldAtkPos;
			print("Position Switched");
		}else{
			print("Stealing failed");
		}
	}
}
