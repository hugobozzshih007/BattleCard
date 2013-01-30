using UnityEngine;
using System.Collections;

public class AutoDestruct : MonoBehaviour {
	
	Transform fxCarrier;
	public float DestructTime;
	
	// Use this for initialization
	void Awake () {
		fxCarrier = transform.FindChild("virtical");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
