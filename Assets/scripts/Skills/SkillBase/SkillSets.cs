using UnityEngine;
using System.Collections;

public class SkillSets : MonoBehaviour {
	
	public Transform[] Skills;
	
	// Use this for initialization
	void Start () {
		Transform skill = transform.FindChild("Skills");
		int childNum = skill.childCount;
				
		Skills = new Transform[childNum];
		for(int i=0;i<childNum;i++){
			Skills[i] = skill.GetChild(i);
		}
	}
	
	// Update is called once per frame
	void Update () {
	}
}
