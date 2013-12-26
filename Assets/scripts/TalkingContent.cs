using UnityEngine;
using System.Collections;

[AddComponentMenu("Data/TalkingContent")]

public class TalkingContent : MonoBehaviour {
	
	public string[] AttackWords = new string[3]; 
	public string[] SkillWords = new string[3];
	
	// Use this for initialization
	void Start () {
	
	}
	
	public string GetAtkWords(){
		string atkWord = "";  
		int num = Random.Range(0, 2);
		atkWord = AttackWords[num];
		return atkWord;
	}
		
	public string GetSkillWords(){
		string skillWord = "";  
		int num = Random.Range(0, 2);
		skillWord = SkillWords[num];
		return skillWord;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
