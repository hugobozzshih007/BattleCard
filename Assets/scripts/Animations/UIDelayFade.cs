using UnityEngine;
using System.Collections;

public class UIDelayFade : MonoBehaviour {
	public float TimeToDelay = 3.0f;
	float timeSeg = 0.0f; 
	bool startToDelay = false; 
	// Use this for initialization
	void Start () {
	}

	public void StartDelay(){
		startToDelay = true;
	}

	// Update is called once per frame
	void Update () {
		if(startToDelay){
			timeSeg+= Time.deltaTime/TimeToDelay;
			if(timeSeg>=0.98f){
				startToDelay = false;
				timeSeg = 0.0f; 
				transform.GetComponent<TweenAlpha>().PlayReverse();
			}
		}
	}
}
