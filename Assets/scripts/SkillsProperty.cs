using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillsProperty : MonoBehaviour{
	
	public int SkillCost;
	public int SkillRate; 
	public string SkillName;
	public string[] ScriptName;
	public bool NeedToSelect;
	
	void Start(){
	}
	
	public void ActivateSkill(){
		foreach(string st in ScriptName){
			CommonSkill skill  = transform.GetComponent(st) as CommonSkill;
			skill.Execute();
		}
	}
}
