using UnityEngine;
using System.Collections;

public class InitTeamEditor : MonoBehaviour {
	GameObject award; 
	// Use this for initialization
	void Start () {
		award = GameObject.Find("Award").gameObject;
		if(award!=null){
			Destroy(award,1.0f);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
