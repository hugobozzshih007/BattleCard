using UnityEngine;
using System.Collections;

public class KeepAward : MonoBehaviour {
	public string Award;
	public Transform EndAward; 
	// Use this for initialization
	void Start () {
	
	}
	
	void Awake () {
		DontDestroyOnLoad(transform.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	}
}
