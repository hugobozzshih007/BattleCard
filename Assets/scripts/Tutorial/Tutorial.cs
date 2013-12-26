using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {
	public LayerMask MaskMap;
	public LayerMask MaskCharacter;
	RoundCounter rc;
	PlaceSummoner pSummoner;
	Transform summoner;
	public Texture2D texContinue;
	Rect rectContinue; 
	MainInfoUI chessUI;
	bool showContinue = false;
	bool fadeIn, fadeOut;
	float _alpha = 0.0f; 
	Transform root, rootB; 
	selection currentSel;
	float castLength = 80.0f;
	Transform hitMap = null;
	Transform hitCharacter = null;
	// Use this for initialization
	void Start () {
		fadeIn = true;
		fadeOut = false;
		root = GameObject.Find("unit_start_point_A").transform;
		rootB = GameObject.Find("unit_start_point_B").transform;
		rc = Camera.mainCamera.GetComponent<RoundCounter>();
		currentSel = Camera.mainCamera.GetComponent<selection>();
		pSummoner = GameObject.Find("InitStage").GetComponent<PlaceSummoner>();
		summoner = pSummoner.SummonerA;
		chessUI = Camera.main.GetComponent<MainInfoUI>();
	}
	
	void Selecting(){
		if(Input.GetMouseButtonDown(0)){
			Ray rayA = Camera.mainCamera.ScreenPointToRay(Input.mousePosition);
			Ray rayB = Camera.mainCamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitA;
			RaycastHit hitB;
			if(Physics.Raycast(rayA, out hitA, castLength,MaskMap)){
				hitMap = hitA.transform;
			}
			if(Physics.Raycast(rayB, out hitB, castLength,MaskCharacter)){
				hitCharacter = hitB.transform;
				//print(hitCharacter.name);
			}
		}
	}
	
	public Transform GetHitMap(){
		
		return hitMap;
	}
	public Transform GetHitCharacter(){
		return hitCharacter;
	}
	
	public void InitTutorial(int stage){
		switch(stage){
			case 1:
				transform.GetComponent<TutorStageA>().StartPause(1.0f);
				break;
			case 2:
				transform.GetComponent<TutorStageB>().StartPause(1.0f);
				break;
			case 3:
				transform.GetComponent<TutorStageC>().StartPause(1.0f);
				break;
			case 4:
				transform.GetComponent<TutorStageD>().StartPause(1.0f);
				break;
			case 5:
				transform.GetComponent<TutorStageE>().StartPause(1.0f);
				break;
			default:
				break;
		}
		
	}
	
	public void ResetMap(int side){
		if(side == 1){
			rc.PlayerATerritory.Clear();
			rc.PlayerATerritory.Add(root);
		}else if(side == 2){
			rc.PlayerBTerritory.Clear();
			rc.PlayerBTerritory.Add(rootB);
		}else if(side == 0){
			rc.PlayerATerritory.Clear();
			rc.PlayerATerritory.Add(root);
			rc.PlayerBTerritory.Clear();
			rc.PlayerBTerritory.Add(rootB);
		}
		currentSel.updateTerritoryMat();
	}
	
	public void ShowContinue(bool show){
		if(show)
			showContinue = true;
		else
			showContinue = false;
	}
	
	public Rect GetContinueRect(){
		Rect rect = new Rect(450.0f/1280.0f*Screen.width, 500.0f/720.0f*Screen.height, 20, 20);
		return rect;
	}
	
	public int GetCurrentStage(){
		return 0; 
	}
	public void AfterMove(){
		summoner = rc.playerA;
		CharacterProperty ap = summoner.GetComponent<CharacterProperty>();
		ap.Attacked = false; 
	}
	
	public void ResetPlayerA(Transform gf){
		CharacterProperty gfp = gf.GetComponent<CharacterProperty>(); 
		gfp.CmdTimes = 3;
		gfp.TurnFinished = false;
		gfp.Attacked = false;
	}
	
	void FadeIn(){
		if(fadeIn){
			_alpha = Mathf.Lerp(_alpha,1.0f,Time.deltaTime*6.0f);
			if(_alpha>=0.97f){
				fadeOut = true;
				fadeIn = false;
			}
		}
	}
	
	void FadeOut(){
		if(fadeOut){
			_alpha = Mathf.Lerp(_alpha,0.0f,Time.deltaTime*6.0f);
			if(_alpha<=0.02f){
				fadeOut = false;
				fadeIn = true;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		Selecting();
		if(showContinue){
			FadeIn(); 
			FadeOut();
		}
	}
	
	void OnGUI(){
		GUI.depth = 0; 
		GUI.backgroundColor = Color.clear;
		if(showContinue){
			GUI.color = new Color(1.0f,1.0f,1.0f,_alpha);
			
			GUI.DrawTexture(GetContinueRect(), texContinue);
		}
	}
}
