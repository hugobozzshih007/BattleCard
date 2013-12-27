using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;
using System.Linq;

public class TP_CtrlSteal : MonoBehaviour, CommonSkillTP {
	const float skillScore = 30.0f; 
	float midCtrlPoint = 2.0f; 
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
		maps = cSkill.GetSelectionRange();
		rc = Camera.main.GetComponent<RoundCounter>();
		TacticPoint tp = new TacticPoint(transform, skillTactic, map, 0);
		IList gfList = new List<Transform>();
		Dictionary<Transform, int> sortDict = new Dictionary<Transform, int>(); 
		if(maps.Count>0){
			foreach(Transform unit in maps){
				Transform gf = MapHelper.GetMapOccupiedObj(unit);
				gfList.Add(gf);
			}
			foreach(Transform gf in gfList){
				CharacterProperty gfp = gf.GetComponent<CharacterProperty>();
				int thretenPt = gfp.Damage + gfp.ModifiedDefPow;  
				sortDict.Add(gf, thretenPt);
			}
			var sortedDict = (from entry in sortDict orderby entry.Value descending select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
			tp.Target = (Transform)sortedDict.First().Key;
			tp.Point = Mathf.RoundToInt(((float)sortedDict.First().Value / midCtrlPoint)*skillScore);
		}
		
		return tp;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
