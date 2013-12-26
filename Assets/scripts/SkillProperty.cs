using UnityEngine;
using System.Collections;
using MapUtility;

public class SkillProperty : MonoBehaviour{
	Transform character; 
	public SkillType SType;
	public PowerType Mode;
	public int ModeValue;
	public int SkillCost;
	public int CoolDownRounds;  
	public int WaitingRounds;
	public int SkillRate; 
	public string SkillName;
	public string ScriptName;
	public string info;
	public bool NeedToSelect;
	public bool SkillReady;
	public bool PassSkillRate; 
	public Texture2D SkillIcon;
	public Tactics SkillTactic;	
	void Start(){
		PassSkillRate = false;
		SkillReady = false;
		character = transform.parent.parent;
		if(character.GetComponent<CharacterProperty>().Summoner && !character.GetComponent<CharacterProperty>().death){
			DefaultCDRounds();
		}
	}
	
	public void GetRealSkillRate(){
		Transform skiller = transform.parent.parent;
		SkillRate = skiller.GetComponent<CharacterProperty>().BuffSkillRate;
	}
	
	public void DefaultCDRounds(){
		SkillReady = false;
		WaitingRounds = CoolDownRounds;
	}
	
	public void ActivateSkill(){
		CommonSkill skill  = transform.GetComponent(ScriptName) as CommonSkill;
		skill.Execute();
	}
	
	void Update(){
		if(WaitingRounds <= 0){
			SkillReady = true;
		}else{
			SkillReady = false;
		}
	}
}
