using UnityEngine;
using System.Collections;
[RequireComponent(typeof(AudioSource))]
public class Basic_UI : MonoBehaviour {
	public AudioClip ClickF, ClickB; 
	public Texture2D PageTitle, StageA, StageB, StageC, StageD, StageE, StageF, Dot, Return, Locked, LeftArrow, RightArrow, Black, Enter; 
	public bool BasicUIShow = true;
	Rect pageTitleRect, stageTitleRect, returnRect, overRect, lockRect, leftRect, rightRect, selectRect, enterRect; 
	Vector2 mousePos; 
	bool[] showStage = new bool[6];
	int stageState = 1; 
	int subStageState = 1;
	SwitchStages sStages;
	StageStates sStates;
	public Font gFont; 
	CameraPathBezierAnimator currentPath; 
	public StageProperty CurrentStage;
	UICamControl uiCam; 
	bool justEnter = false;
	bool subStageMode = false;
	GUIStyle gStyle = new GUIStyle();
	bool firstSet = true;
	// Use this for initialization
	void Start () {
		Application.targetFrameRate = 60;
		gStyle.alignment = TextAnchor.MiddleCenter;
		gStyle.hover.background = Black;
		gStyle.fontSize = 50;
		gStyle.font = gFont;
		gStyle.normal.textColor = Color.white; 
		selectRect = new Rect(Screen.width/2-PageTitle.width/2, 0.8f*Screen.height, PageTitle.width, PageTitle.height);
		pageTitleRect = new Rect(10.0f, 2.0f, 270, 57);
		stageTitleRect = new Rect(Screen.width*0.048f, Screen.height*0.0975f, StageA.width, StageA.height);
		returnRect = new Rect(Screen.width*0.928f, Screen.height*0.129f, Return.width, Return.height);
		overRect = new Rect(returnRect.x-3, returnRect.y-3, returnRect.width+6, returnRect.height+6);
		leftRect = new Rect(selectRect.x-LeftArrow.width-27,Screen.height*0.83f, LeftArrow.width, LeftArrow.height);
		rightRect = new Rect(selectRect.x+selectRect.width+27,Screen.height*0.83f, RightArrow.width, RightArrow.height);
		lockRect = new Rect(Screen.width/2-Locked.width/2, selectRect.y+25, Locked.width, Locked.height);
		enterRect = new Rect(Screen.width/2-Enter.width/2, lockRect.y, Enter.width, Enter.height);
		mousePos = new Vector2();
		sStages = this.GetComponent<SwitchStages>();
		sStates = this.GetComponent<StageStates>();
		uiCam = this.GetComponent<UICamControl>();
		currentPath = uiCam.CamMainPath[0].GetComponent<CameraPathBezierAnimator>();
		for(int i=0;i<6;i++){
			showStage[i] = false;
		}
		showStage[0] = true;
	}
	
	void PlayClick(bool foward){
		if(foward){
			audio.PlayOneShot(ClickF, 1.0f);
		}else{
			audio.PlayOneShot(ClickB, 1.0f);
		}
	}
	
	// Update is called once per frame
	void Update () {
		mousePos.x = Input.mousePosition.x;
		mousePos.y = Screen.height-Input.mousePosition.y;
	}
	
	public void ShowStageUI(int stage){
		for(int i=0;i<6;i++){
			showStage[i] = false;
		}
		showStage[stage-1] = true;
	}
	
	void ChangeStageState(bool mode){
		if(!mode){
			stageState -= 1; 
			if(stageState < 1)
				stageState = 6;
		}else{
			stageState += 1;
			if(stageState > 6)
				stageState = 1; 
		}
	}
	
	void ActivateMainland(int state){
		if(state >0 && state <4){
			sStages.ActivateAll(sStages.Mainland); 
			sStages.DeActivateAll(sStages.MainlandSepia);
		}else if(state == 4){
			sStages.ActivateAll(sStages.Mainland);
			sStages.DeActivateAll(sStages.MainlandSepia);
			sStages.ActivateAll(sStages.Stage[2]);
			sStages.DeActivateAll(sStages.StageSepia[2]);
		}else{
			sStages.ActivateAll(sStages.MainlandSepia); 
			sStages.DeActivateAll(sStages.Mainland);
		}
	}
	
	void ShowLocked(int state){
		if(sStates.StageList[state-1].Locked){
			GUI.DrawTexture(lockRect, Locked);
		}else{
			if(GUI.Button(enterRect, Enter, gStyle)){
				PlayClick(true);
				currentPath.mode = CameraPathBezierAnimator.modes.once;
				currentPath.Seek(0.0f);
				currentPath.PlayToPoint(1);
				ActivateMainland(state);
				subStageMode = true;
				justEnter = true;
			}
		}
	}
	
	void ShowSubStageLocked(int state){
		GUI.Label(stageTitleRect,(string)CurrentStage.SubLevelNames[state-1], gStyle); 
		if(CurrentStage.SubLevel[state-1]){
			if(GUI.Button(enterRect, Enter, gStyle)){
				PlayClick(true);
			}
		}else{
			if(GUI.Button(enterRect, Locked, gStyle)){
				PlayClick(false);
			}
		}
	}
	
	void SetCurrentPath(){
		CameraPathBezierAnimator cpa =  uiCam.CamMainPath[stageState-1].GetComponent<CameraPathBezierAnimator>();
		currentPath = cpa; 
	}
	
	void SetCurrentStage(){
		CurrentStage = (StageProperty)sStates.StageList[stageState-1];
	}
	
	void GoPreviousSubStage(){
		int currentPoint = currentPath.GetCurrentPoint();
		int previousPoint = currentPoint-1; 
		if(previousPoint<0)
			previousPoint = 0; 
		if(currentPoint!= previousPoint)
			currentPath.PlayToPoint(previousPoint);
			
		subStageState -= 1;
	}
	
	void GoNextSubStage(){
		int currentPoint = currentPath.GetCurrentPoint();
		int totalPoint = currentPath.transform.GetChildCount();
		int nextPoint = currentPoint + 1; 
		if(nextPoint > (totalPoint-1))
			nextPoint = totalPoint - 1;
		if(currentPoint!= nextPoint)
			currentPath.PlayToPoint(nextPoint);
			
		subStageState += 1;
	}
	
	void OnGUI(){
		GUI.backgroundColor = Color.black;
		GUI.depth = 0; 
		if(BasicUIShow){
			GUI.DrawTexture(pageTitleRect, PageTitle);
			if(!subStageMode){
				if(!currentPath.isPlaying){
					if(GUI.Button(leftRect,LeftArrow, gStyle)){
						PlayClick(true);
						ChangeStageState(false);
						ShowStageUI(stageState);
						sStages.SwitchStage(stageState-1);
						SetCurrentPath();
						SetCurrentStage();
					}
					if(GUI.Button(rightRect,RightArrow, gStyle)){
						PlayClick(true);
						ChangeStageState(true);
						ShowStageUI(stageState);
						sStages.SwitchStage(stageState-1);
						SetCurrentPath();
						SetCurrentStage();
					}
					ShowLocked(stageState);
				}
				if(showStage[0]){
					GUI.DrawTexture(stageTitleRect, StageA);
				}
				if(showStage[1]){
					GUI.DrawTexture(stageTitleRect, StageB);
				}
				if(showStage[2]){
					GUI.DrawTexture(stageTitleRect, StageC);
				}
				if(showStage[3]){
					GUI.DrawTexture(stageTitleRect, StageD);
				}
				if(showStage[4]){
					GUI.DrawTexture(stageTitleRect, StageE);
				}
				if(showStage[5]){
					GUI.DrawTexture(stageTitleRect, StageF);
				}
			}else{
				if(!currentPath.isPlaying){
					int totalPoint = currentPath.transform.GetChildCount();
					int num = currentPath.GetCurrentPoint();
					if(num!=0 && !justEnter){
						if(GUI.Button(leftRect,LeftArrow, gStyle)){
							PlayClick(true);
							GoPreviousSubStage();
						}
					}
					if(num != (totalPoint-1)){
						if(GUI.Button(rightRect,RightArrow, gStyle)){
							PlayClick(true);
							GoNextSubStage();
							justEnter = false;
						}
					}
					ShowSubStageLocked(subStageState);
				}
			}
			if(returnRect.Contains(mousePos)){
				if(GUI.Button(overRect, Return,gStyle)){
					if(subStageMode){
						PlayClick(false);
						currentPath.Seek(0);
						currentPath.ResetCurrentPoint();
						subStageMode = false;
						ActivateMainland(5);
						sStages.SwitchStage(stageState-1);
						subStageState = 1;
					}else{
					}
				}
			}else{
				GUI.Button(returnRect, Return);
			}
			
			
		}
	}
}
