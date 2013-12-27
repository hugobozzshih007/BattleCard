using UnityEngine;
using System.Collections;

public class FadeOut : MonoBehaviour {
	public Texture2D NextBut;
	
	const float fadeOutTime = 5.0f;
	const float fogEndDist = 28.0f;
	const float lightSpot = 2.8f;
	const float lightDirA = 0.15f;
	const float lightDirB = 0.3f;
	Transform spotLight, dirLightA, dirLightB;
	
	Rect nextRect = new Rect(1105.0f,613.0f,122.0f,52.0f);
	
	bool fadeOut = false;
	bool showGUI = false;
	float t; 
	const int pauseTime = 500;
	int count = 0;
	public bool Pause = false;
	
	Vector2 mousePos;
	// Use this for initialization
	void Start () {
		spotLight = GameObject.Find("Spotlight").transform;
		dirLightA = GameObject.Find("dirLightA").transform;
		dirLightB = GameObject.Find("dirLightB").transform;
		spotLight.GetComponent<Light>().intensity = 0.0f;
		dirLightA.GetComponent<Light>().intensity = 0.0f;
		dirLightB.GetComponent<Light>().intensity = 0.0f;
		Pause = false;
		t = 0.0f;
		RenderSettings.fog = true;
		RenderSettings.fogMode = FogMode.Linear;
		RenderSettings.fogColor = Color.black;
		RenderSettings.fogStartDistance = 0.0f;
		RenderSettings.fogEndDistance = fogEndDist;
	}
	
	// Update is called once per frame
	void Update () {
		mousePos.x = Input.mousePosition.x;
		mousePos.y = Screen.height-Input.mousePosition.y;
		if(Pause){
			count++;
			if(count >= pauseTime){
				count = 0;
				Pause = false;
				fadeOut = true;
			}
		}
		if(fadeOut){
			t+=Time.deltaTime/fadeOutTime;
			float dist = Mathf.Lerp(0.0f,fogEndDist,t);
			float spot = Mathf.Lerp(0.0f, lightSpot, t);
			float dirA = Mathf.Lerp(0.0f, lightDirA, t);
			float dirB = Mathf.Lerp(0.0f, lightDirB, t);
			spotLight.GetComponent<Light>().intensity = spot;
			dirLightA.GetComponent<Light>().intensity = dirA;
			dirLightB.GetComponent<Light>().intensity = dirB;
			//RenderSettings.fogEndDistance = dist;
			if(spotLight.GetComponent<Light>().intensity >= lightSpot){
				fadeOut = false;
				t=0.0f;
				showGUI = true;
			}
		}
	}
	
	void OnGUI(){
		GUI.backgroundColor = Color.clear;
		if(showGUI){
			if(GUI.Button(nextRect, NextBut)){
				Application.LoadLevel(2);
			}
			if(nextRect.Contains(mousePos)){
				Rect over = new Rect(1103.0f, 612.0f, 126.0f, 54.0f);
				GUI.DrawTexture(over, NextBut);
			}
		}
	}
}
