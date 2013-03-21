using UnityEngine;
using System.Collections;

public class SetTeamTemp : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Camera.mainCamera.GetComponent<RoundCounter>().SetPlayerChesses();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
