using UnityEngine;
using System.Collections;

public class PointCalculation : MonoBehaviour {
	public Texture2D ScoreLayout, Star, EmptyStar;
	public Font NumFont; 
	Rect rectScoreLay; 
	Rect pointRect; 
	Rect[] scores = new Rect[6];  
	Rect[] stars = new Rect[3];
	GUIStyle[] numStyle = new GUIStyle[4];
	int totalMapsNum; 
	int normalKilledNum = 0;
	int leaderKilledNum = 0;
	int towerKilledNum = 0;
	const int mapScore = 100;
	const int leaderKillScore = 300; 
	const int towerKillScore = 250; 
	const int killScore = 150;  
	const int turnScore = 250; 
	const int midTurns = 1;
	const int topTurns = 3; 
	int midKScore = 1050;
	int topKScore = 2100;
	int lowestScore; 
	int midScore;  
	int highScore;
	int finalScore = 0;
	int leftTurns = 0;  
	int territoryCounts = 0;
	public bool ComWin = false;
	bool showUI = false;
	// Use this for initialization
	void Start () {
		for(int i = 0; i<3; i++){
			stars[i] = new Rect(62+(Star.width+6)*i,394, Star.width, Star.height);
			scores[i] = new Rect(224,110+(23)*i,18,20);
		}
		
		scores[3] = new Rect(191,231,30,25);
		scores[4] = new Rect(191,288,30,25);
		scores[5] = new Rect(172,348,120,40);
		
		pointRect = new Rect((715.0f/1280.0f)*Screen.width, 8, 276, 26);
		
		numStyle[0] = new GUIStyle();
		numStyle[0].normal.textColor = Color.white;
		numStyle[0].font = NumFont;
		numStyle[0].fontSize = 18;  
		numStyle[0].alignment = TextAnchor.UpperLeft;
		
		numStyle[1] = new GUIStyle(numStyle[0]);
		numStyle[1].fontSize = 24;
		
		numStyle[2] = new GUIStyle(numStyle[0]);
		numStyle[2].fontSize = 36;
		
		numStyle[3] = new GUIStyle(numStyle[0]);
		numStyle[3].fontSize = 20;
		numStyle[3].alignment = TextAnchor.UpperCenter;
			
		rectScoreLay = new Rect(Screen.width/2-ScoreLayout.width/2, Screen.height/2-ScoreLayout.height/2, ScoreLayout.width, ScoreLayout.height);
		totalMapsNum = GameObject.Find("Maps").transform.GetChildCount();
		float half = (float)totalMapsNum*0.5f;
		lowestScore = Mathf.RoundToInt(half*(float)mapScore);
		midScore = Mathf.RoundToInt((float)totalMapsNum*0.7f*(float)mapScore + midTurns*turnScore + midKScore);
		highScore = Mathf.RoundToInt((float)totalMapsNum*0.9f*(float)mapScore + topTurns*turnScore + topKScore);
	}
	
	public void AddDeadNum(Transform gf){
		selection sel = Camera.main.GetComponent<selection>();
		CharacterProperty gfp = gf.GetComponent<CharacterProperty>();
		if(sel.npcMode && gfp.Player == 2){
			if(gfp.Summoner)
				leaderKilledNum += 1;
			else if(gfp.Tower)
				towerKilledNum += 1;
			else
				normalKilledNum += 1;
		}
	}
	
	public void ActivateScoreLayout(){
		finalScore = GetFinalPoint(1);
		leftTurns = GetLeftCounts();
		territoryCounts = GetTerritoryCounts(1);
		showUI = true;
	}
	
	int GetLeftCounts(){
		MainInfoUI infoUI = Camera.mainCamera.GetComponent<MainInfoUI>();
		return infoUI.LeftRounds;
	}
	
	int GetTerritoryCounts(int side){
		int counts = 0;
		RoundCounter rc = Camera.mainCamera.GetComponent<RoundCounter>();
		if(side == 1){
			counts = rc.PlayerATerritory.Count;
		}else{
			counts = rc.PlayerBTerritory.Count;
		}
		return counts;
	}
	
	public int GetPoint(int side){
		int counts = 0;
		int killPoints = normalKilledNum*killScore + leaderKilledNum*leaderKillScore + towerKilledNum*towerKillScore;
		int mapPoints = GetTerritoryCounts(side)*mapScore;
		counts = killPoints + mapPoints;
		return counts;
	}
	
	public int GetFinalPoint(int side){
		int counts = 0;
		int killPoints = normalKilledNum*killScore + leaderKilledNum*leaderKillScore + towerKilledNum*towerKillScore;
		int mapPoints = GetTerritoryCounts(side)*mapScore;
		int turnPoints = GetLeftCounts()*turnScore;
		counts = killPoints + mapPoints + turnPoints;
		return counts;
	}
	
	void DrawStars(int score){
		if(score < midScore && score > lowestScore){
			GUI.DrawTexture(stars[0], Star);
			GUI.DrawTexture(stars[1], EmptyStar);
			GUI.DrawTexture(stars[2], EmptyStar);
		}else if(score >= midScore && score < highScore){
			GUI.DrawTexture(stars[0], Star);
			GUI.DrawTexture(stars[1], Star);
			GUI.DrawTexture(stars[2], EmptyStar);
		}else if(score >= highScore){
			GUI.DrawTexture(stars[0], Star);
			GUI.DrawTexture(stars[1], Star);
			GUI.DrawTexture(stars[2], Star);
		}else{
			GUI.DrawTexture(stars[0], EmptyStar);
			GUI.DrawTexture(stars[1], EmptyStar);
			GUI.DrawTexture(stars[2], EmptyStar);
		}
	}
	
	void ShowScores(bool show){
		if(show){
			GUI.BeginGroup(rectScoreLay);
			GUI.DrawTexture(new Rect(0,0,rectScoreLay.width, rectScoreLay.height), ScoreLayout); 
			if(!ComWin){
				GUI.Label(scores[0], leaderKilledNum.ToString(), numStyle[0]);
				GUI.Label(scores[1], towerKilledNum.ToString(), numStyle[0]);
				GUI.Label(scores[2], normalKilledNum.ToString(), numStyle[0]);
				
				GUI.Label(scores[3], territoryCounts.ToString(), numStyle[1]);
				GUI.Label(scores[4], leftTurns.ToString(), numStyle[1]);
				
				GUI.Label(scores[5], finalScore.ToString(), numStyle[2]);
				DrawStars(finalScore);
			}else{
				GUI.Label(scores[0], "0", numStyle[0]);
				GUI.Label(scores[1], "0", numStyle[0]);
				GUI.Label(scores[2], "0", numStyle[0]);
				
				GUI.Label(scores[3], "0", numStyle[1]);
				GUI.Label(scores[4], "0", numStyle[1]);
				
				GUI.Label(scores[5], "0", numStyle[2]);
				DrawStars(0);
			}
			
			GUI.EndGroup ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnGUI(){
		GUI.depth = 0; 
		GUI.backgroundColor = Color.clear;
		if(!showUI){
			GUI.Label(pointRect, "SCORE: "+GetPoint(1).ToString(),numStyle[3]);
		}
		ShowScores(showUI);
		if(showUI){
			if(Input.GetKeyDown(KeyCode.Return)){
				Application.LoadLevel("DemoOpening");
			}
		}
	}
}
