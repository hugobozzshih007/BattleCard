using UnityEngine;
using System.Collections;

public class AwardTitle : MonoBehaviour {
	public Texture2D Award_Title;
	Rect titleRect = new Rect(285.0f,315.0f,731.0f,48.0f);
	int t = 0; 
	float _textAlpha = 0;
	int delayCounter = 500;
	bool fadeOut = false;
	bool showUI = true;
	// Use this for initialization
	void Start () {
	
	}
	
	void FadeIn(){
		_textAlpha = Mathf.Lerp(_textAlpha,1,Time.deltaTime*2);
		if(_textAlpha>=0.99f){
			fadeOut = true;
		}
	}
	
	void FadeOut(){
		delayCounter -= 1;
		if(delayCounter == 250){
			transform.GetComponent<FadeOut>().Pause = true;
		}
		if(delayCounter<=0){
			_textAlpha = Mathf.Lerp(_textAlpha,0,Time.deltaTime*1);
		}
		if(_textAlpha <= 0.01f){
			showUI = false;
			delayCounter = 600;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(!fadeOut)
			FadeIn();
		else
			FadeOut();
	}
	
	void OnGUI(){
		GUI.depth = 1;
		GUI.color = new Color(1.0f,1.0f,1.0f,_textAlpha);
		GUI.backgroundColor = Color.clear;
		if(showUI){
			GUI.DrawTexture(titleRect, Award_Title);
		}
	}
}
