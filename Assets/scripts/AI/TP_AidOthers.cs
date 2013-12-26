using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;
using System.Linq;

public class TP_AidOthers : MonoBehaviour, CommonSkillTP {
	const float skillScore = 30.0f; 
	IList maps = new List<Transform>(); 
	Transform skill; 
	SkillProperty skillP;
	CommonSkill cSkill;
	Tactics skillTactic;
	RoundCounter rc;
	float hpCheckBar = 0.5f; 
	// Use this for initialization
	void Start () {
	}
	
	public TacticPoint GetSkillTacticPoint(Transform map){
		skill = transform.FindChild("Skills").GetChild(0);
		skillP = skill.GetComponent<SkillProperty>();
		skillTactic = skillP.SkillTactic;
		cSkill = skill.GetComponent(skillP.ScriptName) as CommonSkill;
		rc = Camera.main.GetComponent<RoundCounter>();
		TacticPoint tp = new TacticPoint(transform, skillTactic,map, 0);
		if(map!=null){
			switch(skillTactic){
				case Tactics.Aid_Other_Attack:
					tp.Point = GetAidAtkPoint().First().Value;
					tp.Target = GetAidAtkPoint().First().Key;
					break;
				case Tactics.Aid_Other_Def:
					tp.Point = GetAidDefPoint().First().Value;
					tp.Target = GetAidDefPoint().First().Key;
					break;
				case Tactics.Aid_Other_Move:
					tp.Point = GetAidMovePoint().First().Value;
					tp.Target = GetAidMovePoint().First().Key;
					break;
				case Tactics.Aid_Other_Fly:
					tp.Point = GetAidFlyPoint().First().Value;
					tp.Target = GetAidFlyPoint().First().Key;
					break;
				case Tactics.Heal_Other:
					tp.Point = GetAidHPPoint().First().Value;
					tp.Target = GetAidHPPoint().First().Key;
					break;
			}
		}
		return tp;
	}
	
	Dictionary<Transform, int> GetAidAtkPoint(){
		Transform helpNeeded = transform;
		int tp = 0;
		
		Dictionary<Transform, int> answer = new Dictionary<Transform, int>();
		answer.Add(helpNeeded, tp);
		return answer;
	}
	
	Dictionary<Transform, int> GetAidDefPoint(){
		Transform helpNeeded = transform;
		int tp = 0;
		
		Dictionary<Transform, int> answer = new Dictionary<Transform, int>();
		answer.Add(helpNeeded, tp);
		return answer;
	}
	
	Dictionary<Transform, int> GetAidMovePoint(){
		Transform helpNeeded = transform;
		int tp = 0;
		
		Dictionary<Transform, int> answer = new Dictionary<Transform, int>();
		answer.Add(helpNeeded, tp);
		return answer;
	}
	
	Dictionary<Transform, int> GetAidHPPoint(){
		Dictionary<Transform, float> sortDict = new Dictionary<Transform, float>();
		IList cGFList = new List<Transform>();
		//init transform
		Transform helpNeeded = transform;
		//get allies
		maps = cSkill.GetSelectionRange();
		foreach(Transform map in maps){
			Transform gf = MapHelper.GetMapOccupiedObj(map);
			float hpRate = (float)gf.GetComponent<CharacterProperty>().Hp / (float)gf.GetComponent<CharacterProperty>().MaxHp;
			sortDict.Add(gf, hpRate);
		}
		var sortedDict = (from entry in sortDict orderby entry.Value ascending select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
		float lowestHp = sortedDict.Values.ElementAt(0);
		foreach(KeyValuePair<Transform,float> entry in sortedDict){
			if(entry.Value == lowestHp){
				cGFList.Add(entry.Key);
			}
		}
		helpNeeded = (Transform)cGFList[0];
		int tp = Mathf.RoundToInt((1.0f - lowestHp)*skillScore);
		Dictionary<Transform, int> answer = new Dictionary<Transform, int>();
		answer.Add(helpNeeded, tp);
		return answer;
	}
	
	Dictionary<Transform, int> GetAidCritiqPoint(){
		Transform helpNeeded = transform;
		int tp = 0;
		
		Dictionary<Transform, int> answer = new Dictionary<Transform, int>();
		answer.Add(helpNeeded, tp);
		return answer;
	}
	
	Dictionary<Transform, int> GetAidFlyPoint(){
		Transform helpNeeded = transform;
		int tp = 0;
		
		Dictionary<Transform, int> answer = new Dictionary<Transform, int>();
		answer.Add(helpNeeded, tp);
		return answer;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
