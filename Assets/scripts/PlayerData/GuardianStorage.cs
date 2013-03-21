using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuardianStorage : MonoBehaviour {
	public IList Guardians = new List<Transform>();
	public IList SkillVault = new List<Transform>();
	public Transform SelectedSummoner; 
	public IList SelectedGFs = new List<Transform>();
	public Transform FirstLeader;
	// Use this for initialization
	void Start () {
		Guardians.Add(FirstLeader);
		Transform skill = FirstLeader.FindChild("Skills").GetChild(0);
		SkillVault.Add(skill);
	}
	
	public void AddingAward(Transform award){
		Guardians.Add(award);
		Transform skill = award.FindChild("Skills").GetChild(0);
		SkillVault.Add(skill);
	}
	
	void Awake(){
		DontDestroyOnLoad(transform.gameObject);
	}
	// Update is called once per frame
	void Update () {
		
	}
}
