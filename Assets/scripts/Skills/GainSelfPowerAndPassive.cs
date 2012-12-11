using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;

public class GainSelfPowerAndPassive : MonoBehaviour, CommonSkill {
	
	Transform aider;
	public PassiveType mode;
	public PowerType[] Mode;
	public int[] Value;
	public Dictionary<PowerType, int> PowerList; 
	int skillRate;
	
	// Use this for initialization
	void Start () {
		aider = transform.parent.parent;
		PowerList = new Dictionary<PowerType, int>();
		for(int i=0;i<Mode.Length;i++){
			PowerList.Add(Mode[i],Value[i]);
		}
	}
	
	public void InsertSelection (Transform map)
	{
		throw new System.NotImplementedException ();
	}
	
	public IList GetSelectionRange ()
	{
		throw new System.NotImplementedException ();
	}
	
	public void Execute ()
	{
		aider.GetComponent<CharacterPassive>().PassiveDict[mode] = true;
		foreach(var pair in PowerList){
			switch(pair.Key){
				case PowerType.Critical:
					aider.GetComponent<CharacterProperty>().BuffCriticalHit += pair.Value;
					break;
				case PowerType.Damage:
					aider.GetComponent<CharacterProperty>().Damage += pair.Value;
					break;
				case PowerType.Hp:
					aider.GetComponent<CharacterProperty>().Hp += pair.Value;
					break;
				case PowerType.SkillRate:
					aider.GetComponent<CharacterProperty>().BuffSkillRate += pair.Value;
					break;
				case PowerType.MoveRange:
					aider.GetComponent<CharacterProperty>().BuffMoveRange += pair.Value;
					break;
				case PowerType.AttackRange:
					aider.GetComponent<CharacterProperty>().BuffAtkRange += pair.Value;
					break;
			}
		}
	}
}
