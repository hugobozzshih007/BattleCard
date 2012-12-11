using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;

public class GainSelfPassive : MonoBehaviour, CommonSkill {

	public PassiveType mode;
	int skillRate;
	Transform aider;
	
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
		aider.GetComponent<CharacterPassive>().PassiveDict[mode] = true;
	}
}
