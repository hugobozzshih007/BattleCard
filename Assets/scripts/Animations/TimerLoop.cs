using UnityEngine;
using System.Collections;

public class TimerLoop : MonoBehaviour {
	public RenderTexture DefTexture;
	Vector2 butSize = new Vector2(150.0f, 150.0f);
	Transform currentHit; 
	float t = 0.0f;  
	float time = 1.0f; 
	float targetAngle = 360.0f;
	float castLength = 80.0f;
	float startAngle = 0.0f;
	bool startTimer = false; 
	bool guiShow = false;
	Vector3 screenPos;
	bool showMoveCmd = false;
	Rect cmdRect;
	GeneralSelection currentSel; 
	HealthCircleEffect hce; 
	RoundUI rUI;
	// Use this for initialization
	void Start () {
		hce = transform.GetComponent<HealthCircleEffect>();
		rUI = Camera.mainCamera.GetComponent<RoundUI>();
		currentSel = Camera.mainCamera.GetComponent<GeneralSelection>();
	}
	
	void StartTimer(){
		t=.0f;
		startTimer = true;
	}
	
	void GetSelectedChess(){
		if(Input.GetMouseButton(0) && currentSel.Playing){
			Ray ray = Camera.mainCamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, castLength)){
				currentHit = hit.transform;
				if(hit.transform == currentSel.ChessInSelection && !currentSel.ChessInSelection.GetComponent<CharacterProperty>().TurnFinished){
					guiShow = true;
					startTimer = true;
				}else{
					guiShow = false;
					startTimer = false;
					t = .0f;
				}
			}else{
				guiShow = false;
				startTimer = false;
				t = .0f;
			}
		}else{
			guiShow = false;
			startTimer = false;
			t = .0f;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(currentHit!=null){
			screenPos = Camera.main.WorldToScreenPoint(currentHit.position);
			screenPos.y = Screen.height - screenPos.y;
		}
		GetSelectedChess();
		if(startTimer){
			t += Time.deltaTime/time;
			float trueAngle = Mathf.Lerp(startAngle, targetAngle, t);
			hce.m_angle = trueAngle;
			if(trueAngle >= 359.5f){
				t = .0f;
				if(currentSel.ChessInSelection!=null && !currentSel.ChessInSelection.GetComponent<CharacterProperty>().Defensed && !rUI.showUI)
					currentSel.DefenseCmd(currentSel.ChessInSelection);
				startTimer = false;
				guiShow = false;
			}
		}
		if(t>0.0f){
			showMoveCmd = false;
		}
		if(Input.GetMouseButtonUp(0) && !rUI.showUI){
			if(showMoveCmd && currentSel.ChessInSelection.gameObject.layer == 11 && !currentSel.selectMode){
				//currentSel.CancelCmds();
				currentSel.ChessInSelection.GetComponent<CharacterProperty>().Defensed = false;
				currentSel.moveCommand(currentHit);
				currentSel.attackCommand(currentHit);
			}
		}
	}
	
	void OnGUI(){
		GUI.depth = 1;
		GUI.backgroundColor = Color.clear;
		GUI.color = new Color(1.0f,1.0f,1.0f,.85f);
		if(guiShow){
			showMoveCmd = true;
			currentSel.CleanMapsMat();
			if(!currentHit.GetComponent<CharacterProperty>().Defensed && !rUI.showUI){
				cmdRect = new Rect(screenPos.x-butSize.x/2, screenPos.y-butSize.y/2.0f, butSize.x, butSize.y);
				GUI.DrawTexture(cmdRect, DefTexture);
			}
		}else{
			showMoveCmd = false;
		}
	}
}
