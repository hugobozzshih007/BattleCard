using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuardianStorage : MonoBehaviour {
	public Transform[] GFs = new Transform[12];
	public IList Guardians = new List<Transform>();
	public IList SkillVault = new List<Transform>();
	public Transform SelectedSummoner; 
	public IList SelectedGFs = new List<Transform>();
	public IList UnSelectedGFs = new List<Transform>();
	public Transform FirstLeader;
	// Use this for initialization
	void Start () {
		Guardians.Add(FirstLeader);
		Transform skill = FirstLeader.FindChild("Skills").GetChild(0);
		SkillVault.Add(skill);
		foreach(Transform gf in GFs){
			Guardians.Add(gf);
			Transform gSkill = gf.FindChild("Skills").GetChild(0);
			SkillVault.Add(gSkill);
		}
		
	}
	
	public void SetTeamUp(Transform A, Transform B){
		CharacterProperty summonerAP = A.transform.GetComponent<CharacterProperty>();
		CharacterProperty summonerBP = B.transform.GetComponent<CharacterProperty>();
		summonerAP.soldiers = new Transform[6];
		summonerBP.soldiers = new Transform[6];
		SelectedGFs.CopyTo(summonerAP.soldiers,0);
		if(UnSelectedGFs.Count <= 6)
			UnSelectedGFs.CopyTo(summonerBP.soldiers,0);
		else{
			for(int i=0; i<6; i++){
				summonerBP.soldiers[i] = (Transform)UnSelectedGFs[i];
			}
		}
	}
	
	public void AddingAward(Transform award){
		Guardians.Add(award);
		Transform skill = award.FindChild("Skills").GetChild(0);
		SkillVault.Add(skill);
	}
	
	void Awake(){
		if(Application.loadedLevelName == "team_editor"){
			DontDestroyOnLoad(transform.gameObject);
		}
	}
	// Update is called once per frame
	void Update () {
		
	}
}
