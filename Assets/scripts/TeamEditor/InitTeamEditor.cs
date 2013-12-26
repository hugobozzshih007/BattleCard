using UnityEngine;
using System.Collections;

public class InitTeamEditor : MonoBehaviour {
	GameObject award; 
	public bool inEditor = true; 
	// Use this for initialization
	void Start () {
		inEditor = true;
		award = GameObject.Find("Award").gameObject;
		if(award!=null){
			Destroy(award,1.0f);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
