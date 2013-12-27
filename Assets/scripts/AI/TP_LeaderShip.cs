using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;

public class TP_LeaderShip : MonoBehaviour, CommonSkillTP {
	const float skillScore = 30.0f; 
	float midCtrlPoint = 3.0f; 
	IList maps = new List<Transform>(); 
	Transform skill; 
	SkillProperty skillP;
	SkillInterface cSkill;
	Tactics skillTactic;
	// Use this for initialization
	void Start () {
	
	}
	
	public TacticPoint GetSkillTacticPoint(Transform map){
		skill = transform.FindChild("Skills").GetChild(0);
		skillP = skill.GetComponent<SkillProperty>();
		skillTactic = skillP.SkillTactic;
		cSkill = skill.GetComponent(skillP.ScriptName) as SkillInterface;
		maps = cSkill.GetSelectionRange();
		TacticPoint tp = new TacticPoint(transform, skillTactic, map, 0);
		if(maps.Count>0){
			tp.Point = Mathf.RoundToInt(((float)maps.Count/midCtrlPoint)*skillScore);
		}
		return tp;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
