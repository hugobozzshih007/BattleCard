using UnityEngine;
using System.Collections;

public class StageLayout : MonoBehaviour {
	public Texture2D BackGround; 
	public Texture2D RollOver; 
	public Texture2D[] StageBt = new Texture2D[5];
	Rect bgRect = new Rect(0,0,Screen.width, Screen.height);
	Rect[] btRect = new Rect[5];
	Vector2 mousePos;
	StageSelection ss; 
	SystemSound sSoundOver;
	SystemSound sSoundClick;
	// Use this for initialization
	void Start () {
		btRect[0] = new Rect(61.0f/1280.0f*Screen.width, 227.0f/720.0f*Screen.height, RollOver.width, RollOver.height);
		for(int i=1; i<5; i++){
			btRect[i] = new Rect(btRect[0].x+(RollOver.width+46.0f)*i,btRect[0].y,RollOver.width, RollOver.height);
		}
		ss = GameObject.Find("StageSelection").GetComponent<StageSelection>();
		sSoundOver = GameObject.Find("SystemSoundOver").GetComponent<SystemSound>();
		sSoundClick = GameObject.Find("SystemSoundClick").GetComponent<SystemSound>();
	}
	
	// Update is called once per frame
	void Update () {
		mousePos.x = Input.mousePosition.x;
		mousePos.y = Screen.height-Input.mousePosition.y;
	}
	
	void OnGUI(){
		GUI.depth = 2; 
		GUI.backgroundColor = Color.clear; 
		GUI.DrawTexture(bgRect, BackGround);
		for(int i=0; i<5; i++){
			if(GUI.Button(btRect[i],StageBt[i])){
				sSoundClick.PlaySound(SysSoundFx.CommandClick);
				ss.SetStage(i+1);
			}
			if(btRect[i].Contains(mousePos)){
				GUI.DrawTexture(btRect[i],RollOver);
				GUI.DrawTexture(btRect[i],StageBt[i]);
				sSoundOver.PlaySound(SysSoundFx.CommandOver);
			}
		}
	}
}
