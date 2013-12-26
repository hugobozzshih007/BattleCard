using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Tactic AI/Character Tactics")]

public class CharacterTactics : MonoBehaviour {
	public Tactics[] Chess_Tactic;
	public IList TacticList= new List<Tactics>();
	public string SkillTacticName; 
	Transform currentSkill;
	// Use this for initialization
	void Start () {
		currentSkill = transform.FindChild("Skills").GetChild(0);
		SkillProperty sp =currentSkill.GetComponent<SkillProperty>();
		TacticList.Add(sp.SkillTactic);
		foreach(Tactics t in Chess_Tactic){
			TacticList.Add(t);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
