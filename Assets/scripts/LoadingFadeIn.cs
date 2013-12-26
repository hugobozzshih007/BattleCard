using UnityEngine;
using System.Collections;

public class LoadingFadeIn : MonoBehaviour {
	
	public Texture2D LoadingImage, LoadingBeat;
	Rect loadRect = new Rect();
	float _screenAlpha = 0.0f;
	bool showLoadingScreen = false;
	bool screenFadeIn = false;
	bool showLoading = false;
	float timeSeg = 0.0f; 
	float timeToFade = 1.0f;
	string levelName = ""; 
	
	// Use this for initialization
	void Start () {
		loadRect = new Rect(1058.0f/1280.0f*Screen.width, 648.0f/720.0f*Screen.height, 160, 26);
	}
	
	public void ActivateLoading(string name){
		levelName = name;  
		screenFadeIn = true; 
		showLoadingScreen = true;
	}
	
	// Update is called once per frame
	void Update () {
		ScreenFadeIn();
	}
	
	void ScreenFadeIn(){
		if(screenFadeIn){
			timeSeg+=Time.deltaTime/timeToFade;
			_screenAlpha = Mathf.Lerp(0.0f,1.0f,timeSeg);
			if(_screenAlpha>=0.9f){
				screenFadeIn = false;  
				showLoading = true;
				timeSeg = 0.0f;
				if(levelName!="")
					Application.LoadLevel(levelName);
			}
		}
	}
	
	void OnGUI(){
		if(showLoadingScreen){
			GUI.depth = 0;
			GUI.color = new Color(1.0f, 1.0f, 1.0f, _screenAlpha);
			GUI.DrawTexture(new Rect(0,0,Screen.width, Screen.height), LoadingImage);
			if(showLoading){
				GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
				GUI.DrawTexture(loadRect, LoadingBeat);
			}
		}
	}
}
