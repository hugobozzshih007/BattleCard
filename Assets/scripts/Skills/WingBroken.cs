using UnityEngine;
using System.Collections;
using MapUtility; 
using BuffUtility;

public class WingBroken : MonoBehaviour, CommonSkill {
	Transform aider;
	public PowerType mode;
	public int Value;
	// Use this for initialization
	void Start () {
		aider = transform.parent.parent;
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
		aider.GetComponent<CharacterPassive>().PassiveDict[PassiveType.Flying] = false;
		BuffCalculation bCal = new BuffCalculation(aider);
		switch(mode){
			case PowerType.Critical:
				aider.GetComponent<BuffList>().ExtraDict[BuffType.CriticalHit] += Value;
				bCal.UpdateBuffValue();
				break;
			case PowerType.Damage:
				aider.GetComponent<BuffList>().ExtraDict[BuffType.Attack] += Value;
				bCal.UpdateBuffValue();
				break;
			case PowerType.Hp:
				aider.GetComponent<BuffList>().ExtraDict[BuffType.Defense] += Value;
				bCal.UpdateBuffValue();
				break;
			case PowerType.SkillRate:
				aider.GetComponent<BuffList>().ExtraDict[BuffType.SkillRate] += Value;
				bCal.UpdateBuffValue();
				break;
			case PowerType.MoveRange:
				aider.GetComponent<BuffList>().ExtraDict[BuffType.MoveRange] += Value;
				bCal.UpdateBuffValue();
				break;
			case PowerType.AttackRange:
				aider.GetComponent<BuffList>().ExtraDict[BuffType.AttackRange] += Value;
				bCal.UpdateBuffValue();
				break;
		}
	}
	
}
