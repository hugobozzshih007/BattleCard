using UnityEngine;
using System.Collections;

public class RoundUI : MonoBehaviour {
	
	public Texture2D RedRound, YelRound;
	Texture2D showRound; 
	public bool StartUI = false;
	public bool UIfinished = false;
	public bool Wait = false;
	bool showUI, inDelay;
	bool FadeInUI;
	bool runUI = false;
	float _Alpha = 0.0f;
	GUIStyle rUI;
	Rect startRect, midRect, mid2NDRect, endRect, posRect; 
	float moveSpeed = 1.0f;
	float timeToShow = 0.5f;
	float timeToDelay = 1.2f;
	float diffWidth = 0.0f; 
	int delayCounter = 120;
	float t = 0.0f;
	
	
	// Use this for initialization
	void Start () {
		rUI = new GUIStyle();
		rUI.alignment = TextAnchor.MiddleCenter;
		startRect = new Rect(0.0f-RedRound.width/2.0f, Screen.height/2.0f- RedRound.height/2.0f, RedRound.width, RedRound.height);
		posRect = new Rect(startRect);
		midRect = new Rect(Screen.width/2.0f-startRect.width/2.0f, startRect.height,startRect.width,startRect.height);
		mid2NDRect = new Rect(midRect.x +50.0f, midRect.y, midRect.width, midRect.height);
		endRect = new Rect(Screen.width+RedRound.width/2.0f, startRect.height,startRect.width,startRect.height);
		inDelay = false;
	}
	
	public void SetRoundUI(Color side){
		if(side == Color.red)
			showRound = RedRound;
		else if(side == Color.yellow)
			showRound = YelRound;
		runUI = true; 
		FadeInUI = true;
		UIfinished = false;
	}
	
	void FadeIn(){
		showUI = true;
		t+=Time.deltaTime/timeToShow;
		_Alpha = Mathf.Lerp(_Alpha,1,Time.deltaTime*10);
		float diff = Mathf.Lerp(startRect.x,midRect.x,t);
		posRect = new Rect(diff, startRect.y, startRect.width,startRect.height);
		float diffX = midRect.x - posRect.x;
		
		if(diffX<=0.01f){
			FadeInUI = false;
			inDelay = true;
			t = 0.0f;
		}
	}
	
	void FadeOut(){
		if(inDelay){
			t+=Time.deltaTime/timeToDelay;
			_Alpha = 1.0f;
			float diff = Mathf.Lerp(midRect.x, mid2NDRect.x, t);
			posRect = new Rect(diff, startRect.y, startRect.width,startRect.height);
			float diffX = mid2NDRect.x - posRect.x;
			if(diffX<=0.01f){
				inDelay = false;
				t = 0.0f;
			}
		}else{
			t+=Time.deltaTime/timeToShow;
			_Alpha = Mathf.Lerp(_Alpha,0,Time.deltaTime*10);
			float diff = Mathf.Lerp(mid2NDRect.x, endRect.x, t);
			posRect = new Rect(diff, startRect.y, startRect.width,startRect.height);
			float diffX = endRect.x - posRect.x;
			if(diffX <= 0.05f){
				showUI = false;
				delayCounter = 120;
				diffWidth = 0.0f;
				runUI = false;
				UIfinished = true;
				t = 0.0f;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		if(runUI && !Wait){
			if(FadeInUI)
				FadeIn();
			else
				FadeOut();
		}
	}
	
	void OnGUI(){
		GUI.depth = 0;
		GUI.color = new Color(1.0f,1.0f,1.0f,_Alpha);
		GUI.backgroundColor = Color.clear;
		if(showUI){
			GUI.DrawTexture(posRect, showRound);
		}
	}
}
