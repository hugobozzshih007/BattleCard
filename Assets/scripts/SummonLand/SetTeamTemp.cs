using UnityEngine;
using System.Collections;

public class SetTeamTemp : MonoBehaviour {
	
	GuardianStorage gfStore;
	PlaceSummoner ps;
	
	// Use this for initialization
	void Start () {
		ps = transform.GetComponent<PlaceSummoner>();
		//Camera.mainCamera.GetComponent<RoundCounter>().SetPlayerChesses();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
