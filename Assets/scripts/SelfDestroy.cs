using UnityEngine;
using System.Collections;

public class SelfDestroy : MonoBehaviour {
	public float Seconds = 3.0f;
	// Use this for initialization
	void Start () {
		Destroy(transform.gameObject, Seconds);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
