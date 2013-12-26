using UnityEngine;
using System.Collections;

public class SRunningEvet : MonoBehaviour {
	Transform handSword, backSword;  
	// Use this for initialization
	void Start () {
		handSword = transform.GetChild(0);
		backSword = transform.GetChild(1);
	}
	
	void ShowHandSword(){
		handSword.renderer.enabled = true;
		backSword.renderer.enabled = false;
	}
	
	void HideHandSword(){
		handSword.renderer.enabled = false;
		backSword.renderer.enabled = true;
	}
	
}
