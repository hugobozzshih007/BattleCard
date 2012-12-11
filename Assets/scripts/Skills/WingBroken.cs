using UnityEngine;
using System.Collections;

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
		switch(mode){
			case PowerType.Critical:
				aider.GetComponent<CharacterProperty>().BuffCriticalHit += Value;
				break;
			case PowerType.Damage:
				aider.GetComponent<CharacterProperty>().Damage += Value;
				break;
			case PowerType.Hp:
				aider.GetComponent<CharacterProperty>().Hp += Value;
				break;
			case PowerType.SkillRate:
				aider.GetComponent<CharacterProperty>().BuffSkillRate += Value;
				break;
			case PowerType.MoveRange:
				aider.GetComponent<CharacterProperty>().BuffMoveRange += Value;
				break;
			case PowerType.AttackRange:
				aider.GetComponent<CharacterProperty>().BuffAtkRange += Value;
				break;
		}
	}
	
}
