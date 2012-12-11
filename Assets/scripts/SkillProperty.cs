using UnityEngine;
using System.Collections;
using MapUtility;

public class SkillProperty : MonoBehaviour{
	
	public int SkillCost;
	public int SkillRate; 
	public string SkillName;
	public string ScriptName;
	public string info;
	public bool NeedToSelect;
	public bool PassSkillRate; 
	public Texture2D SkillIcon;
		
	void Start(){
		PassSkillRate = false;
	}
	
	public void GetRealSkillRate(){
		Transform skiller = transform.parent.parent;
		SkillRate = skiller.GetComponent<CharacterProperty>().BuffSkillRate;
	}
	
	public void ActivateSkill(){
		CommonSkill skill  = transform.GetComponent(ScriptName) as CommonSkill;
		skill.Execute();
	}
}
