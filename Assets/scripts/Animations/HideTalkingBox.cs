using UnityEngine;
using System.Collections;

public class HideTalkingBox : MonoBehaviour {
	// Use this for initialization
	void Start () {
	
	}

	public void HideMySelf(){
		if(transform.parent.GetComponent<UISprite>().color.a == 0.0f){
			transform.GetComponent<TweenAlpha>().PlayReverse();
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
