using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;

public class TP_AidSelf : MonoBehaviour, CommonSkillTP {
	public PowerType PowerMode;
	const float skillScore = 30.0f; 
	IList maps = new List<Transform>(); 
	Transform skill; 
	SkillProperty skillP;
	SkillInterface cSkill;
	Tactics skillTactic;
	RoundCounter rc;
	
	// Use this for initialization
	void Start () {
	
	}
	
	public TacticPoint GetSkillTacticPoint(Transform map){
		skill = transform.FindChild("Skills").GetChild(0);
		skillP = skill.GetComponent<SkillProperty>();
		skillTactic = skillP.SkillTactic;
		cSkill = skill.GetComponent(skillP.ScriptName) as SkillInterface;
		TacticPoint tp = new TacticPoint(transform,skillTactic,map,0);
		
		switch(PowerMode){
			case PowerType.AttackRange:
				tp.Point = GetAtkRengeTP(map);
				break;
			case PowerType.Critical:
				tp.Point = GetAtkTP(map);
				break;
			case PowerType.Damage:
				tp.Point = GetAtkTP(map);
				break;
			case PowerType.Defense:
				tp.Point = GetAtkTP(map);
				break;
			case PowerType.MoveRange:
				tp.Point = GetMoveRengeTP(map);
				break;
			case PowerType.SkillRate:
				tp.Point = GetSkillRateTP(map);
				break;
			default:
				break;
		}
		return tp;
	}
	
	int GetAtkRengeTP(Transform map){
		int tPoint = (int)skillScore * 3;
		return tPoint;
	}
	
	int GetAtkTP(Transform map){
		int tPoint = 0;
		AttackCalFX attackerCal = Camera.main.GetComponent<AttackCalFX>();
		IList attackableLists = attackerCal.GetAttackableTarget(transform);
		float midTarget = 3.0f;
		tPoint = Mathf.RoundToInt(((float)attackableLists.Count / midTarget)*skillScore);
		return tPoint;
	}
	
	int GetCriticalTP(Transform map){
		int tPoint = 0;
		return tPoint;
	}
	
	int GetDefenseTP(Transform map){
		int tPoint = 0;
		return tPoint;
	}
	
	int GetMoveRengeTP(Transform map){
		int tPoint = (int)skillScore * 3;
		
		return tPoint;
	}
	
	int GetSkillRateTP(Transform map){
		int tPoint = (int)skillScore * 3;
		return tPoint;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
