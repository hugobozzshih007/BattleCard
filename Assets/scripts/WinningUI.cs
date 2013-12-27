using UnityEngine;
using System.Collections;

public class WinningUI : MonoBehaviour {
	public Texture2D RedWon, YelWon, DrawGame, YouWin, YouLose;
	Texture2D showRound; 
	public bool StartUI = false;
	public bool UIfinished = true;
	public bool Wait = false;
	public bool showUI;
	public Font numFont; 
	bool FadeInUI;
	bool runUI = false;
	bool showPlay = false;
	float _Alpha = 0.0f;
	GUIStyle rUI;
	int winner;
	Rect startRect, midRect, posRect; 
	Rect pointRect;
	float moveSpeed = 1.0f;
	float timeToShow = 0.5f;
	float timeToDelay = 1.2f;
	float diffWidth = 0.0f; 
	int delayCounter = 120;
	float t = 0.0f;
	Vector2 mousePos = new Vector2();
	GeneralSelection currentSel; 
	EndSummonland endStage; 
	StatusMachine sMachine;
	PointCalculation pCal; 
	// Use this for initialization
	void Start () {
		rUI = new GUIStyle();
		rUI.alignment = TextAnchor.MiddleCenter;
		pCal = transform.GetComponent<PointCalculation>();
		startRect = new Rect(0.0f-RedWon.width/2.0f, Screen.height/2.0f- RedWon.height/2.0f, RedWon.width, RedWon.height);
		posRect = new Rect(startRect);
		midRect = new Rect(Screen.width/2.0f-startRect.width/2.0f, startRect.height,startRect.width,startRect.height);
		pointRect = new Rect(midRect.x,  Screen.height/2.0f + 100, 300, 50);
		endStage = transform.GetComponent<EndSummonland>();
		sMachine = GameObject.Find("StatusMachine").transform.GetComponent<StatusMachine>();
		currentSel = Camera.main.GetComponent<GeneralSelection>();
		rUI.normal.textColor = Color.white;
		rUI.font = numFont; 
		rUI.fontSize = 24;
	}
	
	public void SetWinningUI(int whoWon){
		if(!currentSel.npcMode){
			if(whoWon == 1){
				winner = 1;
				showRound = RedWon;
			}else if(whoWon == 2){
				winner = 2;
				showRound = YelWon;
				
			}else{
				winner = 3;
				showRound = DrawGame;
			}
		}else{
			if(whoWon == 1){
				winner = 1;
				showRound = YouWin;
			}else if(whoWon == 2){
				winner = 2;
				showRound = YouLose;
				pCal.ComWin = true;
			}else{
				winner = 3;
				showRound = DrawGame;
			}
		}
		runUI = true; 
		FadeInUI = true;
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
			showPlay = true;
			t = 0.0f;
		}
	}
	
	// Update is called once per frame
	void Update () {
		mousePos.x = Input.mousePosition.x;
		mousePos.y = Screen.height-Input.mousePosition.y;
		if(runUI){
			if(FadeInUI)
				FadeIn();
		}
	}
	
	void OnGUI(){
		GUI.depth = 0;
		GUI.color = new Color(1.0f,1.0f,1.0f,_Alpha);
		GUI.backgroundColor = Color.clear;
		if(showUI){
			GUI.DrawTexture(posRect, showRound);
			if(currentSel.npcMode && winner == 1){
				GUI.color = new Color(1.0f,1.0f,1.0f,1.0f);
			}
		}
		if(showPlay){
			GUI.Label(new Rect(posRect.x, posRect.y + 400, 512, 30), "Press Any Key to Continue...", rUI);
			if(Input.anyKeyDown){
				//play again
				pCal.ActivateScoreLayout(); 
				showPlay = false;
				showUI = false;
			}
		}
	}
}
