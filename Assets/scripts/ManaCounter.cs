using UnityEngine;
using System.Collections;

public class ManaCounter : MonoBehaviour {
	public int Mana = 0;
	
	// Use this for initialization
	void Start () {
		if(transform.GetComponent<CharacterProperty>().Player==1)
			Mana = 2;
		else
			Mana = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
