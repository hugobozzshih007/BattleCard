using UnityEngine;
using System.Collections;

public class SetTeam : MonoBehaviour {
	GuardianStorage gfStore;
	PlaceSummoner ps;
	// Use this for initialization
	void Start () {
		ps = transform.GetComponent<PlaceSummoner>();
		gfStore = GameObject.Find("PlayerData").transform.GetComponent<GuardianStorage>();
		if(gfStore!=null){
			Transform summoner = Instantiate(gfStore.SelectedSummoner, ps.SummonerA.position,  ps.SummonerA.rotation) as Transform;
			CharacterProperty sp = summoner.GetComponent<CharacterProperty>();
			sp.Summoner = true;
			sp.InitPlayer = 1;
			sp.Player = 1;
			sp.Death = false;
			sp.TurnFinished = false;
			int num = gfStore.SelectedGFs.Count; 
			if(num>0){
				sp.soldiers = new Transform[num];
				gfStore.SelectedGFs.CopyTo(sp.soldiers,0);
				gfStore.SelectedGFs.Clear();
			}
			foreach(Transform gf in sp.soldiers){
				CharacterProperty gfp = gf.GetComponent<CharacterProperty>();
				gfp.Summoner = false;
				gfp.Death = true;
			}
			summoner.parent = ps.SummonerA;   
			Camera.mainCamera.GetComponent<RoundCounter>().SetPlayerChesses();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
