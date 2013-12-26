using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;
using System.Linq;

public class TP_BombAttack : MonoBehaviour, CommonSkillTP {
	
	const float skillScore = 30.0f; 
	int bombDamage = 1; 
	float midDamagePoint = 3.5f; 
	IList maps = new List<Transform>(); 
	Transform skill; 
	SkillProperty skillP;
	CommonSkill cSkill;
	Tactics skillTactic;
	RoundCounter rc;
	
	// Use this for initialization
	void Start () {
		
	}
	
	public TacticPoint GetSkillTacticPoint(Transform map){
		skill = transform.FindChild("Skills").GetChild(0);
		skillP = skill.GetComponent<SkillProperty>();
		skillTactic = skillP.SkillTactic;
		cSkill = skill.GetComponent(skillP.ScriptName) as CommonSkill;
		rc = Camera.main.GetComponent<RoundCounter>();
		TacticPoint tp = new TacticPoint(transform, skillTactic, map, 0);
		Dictionary<Transform, int> sortDict = new Dictionary<Transform, int>();
		maps = cSkill.GetSelectionRange();
		if(skillTactic == Tactics.Magic_Range_Attack && maps.Count > 0){
			foreach(Transform sel in maps){
				int damagePoint = 1; 
				IList gfAroundMap = MapHelper.GetAroundGFs(sel);
				if(gfAroundMap.Count>0){
					foreach(Transform target in gfAroundMap){
						damagePoint += 1; 
					}
				}
				sortDict.Add(sel, damagePoint);
			}
			var sortedDict = (from entry in sortDict orderby entry.Value descending select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
			Transform champMap = (Transform)sortedDict.First().Key;
			int point = (int)sortedDict.First().Value;
			tp.Target = MapHelper.GetMapOccupiedObj(champMap);
			tp.Point = Mathf.RoundToInt(((float)point / midDamagePoint)*skillScore);
		}
		return tp; 
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
