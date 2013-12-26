using UnityEngine;
using System.Collections;

public class UICamControl : MonoBehaviour {
	
	public GameObject[] CamMainPath = new GameObject[6];
	public GameObject[] CamSubPath = new GameObject[6];
	public GameObject Water; 
		
	// Use this for initialization
	void Start () {
		Water.GetComponent<Water>().SetToRefractive();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
