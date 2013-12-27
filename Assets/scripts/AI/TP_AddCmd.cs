using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;
using System.Linq;

public class TP_AddCmd : MonoBehaviour, CommonSkillTP {
	const float skillScore = 30.0f; 
	IList maps = new List<Transform>(); 
	Transform skill; 
	SkillProperty skillP;
	SkillInterface cSkill;
	Tactics skillTactic;
	RoundCounter rc;
	float checkBar = 3.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	public TacticPoint GetSkillTacticPoint(Transform map){
		skill = transform.FindChild("Skills").GetChild(0);
		skillP = skill.GetComponent<SkillProperty>();
		skillTactic = skillP.SkillTactic;
		cSkill = skill.GetComponent(skillP.ScriptName) as SkillInterface;
		rc = Camera.main.GetComponent<RoundCounter>();
		TacticPoint tp = new TacticPoint(transform, skillTactic,map, 0);
		
		Dictionary<Transform, float> sortDict = new Dictionary<Transform, float>();
		maps = cSkill.GetSelectionRange();
		if(maps.Count >0){
			foreach(Transform unit in maps){
				if(MapHelper.IsMapOccupied(unit)){
					Transform gf = MapHelper.GetMapOccupiedObj(unit);
					int cmdT = 3 - gf.GetComponent<CharacterProperty>().CmdTimes;
					float cmdPt = (float)cmdT/checkBar * skillScore;
					sortDict.Add(gf, cmdPt);
				}
			}
			var sortedDict = (from entry in sortDict orderby entry.Value descending select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
			tp.Target = (Transform)sortedDict.First().Key;
			tp.Point = Mathf.RoundToInt((float)sortedDict.First().Value);
		}
		return tp;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
