using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;
using System.Linq;

public class TP_MultiArrow : MonoBehaviour, CommonSkillTP {
	const float skillScore = 100.0f; 
	Transform skill; 
	SkillProperty skillP;
	CommonSkill cSkill;
	Tactics skillTactic;
	//RoundCounter rc;
	// Use this for initialization
	void Start () {
	
	}
	
	public TacticPoint GetSkillTacticPoint(Transform map){
		skill = transform.FindChild("Skills").GetChild(0);
		skillP = skill.GetComponent<SkillProperty>();
		skillTactic = skillP.SkillTactic;
		//cSkill = skill.GetComponent(skillP.ScriptName) as CommonSkill;
		//rc = Camera.main.GetComponent<RoundCounter>();
		TacticPoint tp = new TacticPoint(transform,skillTactic, map, transform, (int)skillScore);
		return tp;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
