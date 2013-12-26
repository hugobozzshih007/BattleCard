using UnityEngine;
using System.Collections;
using MapUtility;

public class ActStealControl : MonoBehaviour {
	public string SkillName;
	Transform stealingControl;  
	// Use this for initialization
	void Start () {
		stealingControl = MapHelper.FindAnyChildren(transform.parent, SkillName);
	}
	
	void ExcuteRealControl(){
		stealingControl.GetComponent<CtrlStealing>().ActivateControl();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
