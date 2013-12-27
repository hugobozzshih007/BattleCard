using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;

public class MainInfoUI : MonoBehaviour {
	public Texture2D InfoWindow, UnderBar,Black, ManaA, ManaB, RoundCounterImg, Dead, criticalImg, TalkingCol, AlphaBut;
	public Camera ProgressCam; 
	public bool StopSkillRender = true;
	Camera[] skillCams; 
	RenderTexture[] progressTex;
	bool ShowUI = false;
	bool ShowTarget = false;
	public bool MainFadeIn = false;
	public bool TargetFadeIn = false;
	public bool DelayFadeOut = false; 
	public Font Title, Number;
	public bool Critical = false;
	public bool CriticalRight = false;
	public bool Talking = false;
	public bool TalkingRight = false;
	public int fadeOutSpeed = 15;
	public int LimitedRounds = 30; 
	public int LeftRounds = 1;
	Transform leftChess, rightChess, playerA, playerB;
	Rect leftInfoWin, rightInfoWin, leftChessPos, rightChessPos, mirrorRect, posLeftTitle, posRightTitle, posLeftContent, posRightContent, posAChessList, posBChessList, RightTalking, LeftTalking, talkContent,rightTalkContent;
	public int playerSide;
	GUIStyle titleStyle, manaStyle, roundStyle, subStyle, talkingStyle;
	GUIStyle[] numberStyle = new GUIStyle[5];
	float iconWidth = 20.0f;
	InfoUI iconVault; 
	int allMaps;
	RoundCounter rc;
	GeneralSelection currentSel;  
	float _mainAlpha, _targetAlpha, _castAlpha;
	int delayCounter = 200;
	Vector2 mousePos;
	bool fadeIn = true; 
	bool fadeOut = false;
	Rect mainWindow; 
	public Color RedSide, YelSide;
	StatusMachine sMachine;
	string talkingContent = "Shit!";
	FollowCam fCam;
	SystemSound sysSound;
	
	
	// Use this for initialization
	void Start () {
		fCam = transform.GetComponent<FollowCam>();
		iconVault = transform.GetComponent<InfoUI>();
		rc = transform.GetComponent<RoundCounter>();
		sMachine = GameObject.Find("StatusMachine").transform.GetComponent<StatusMachine>();
		sysSound = GameObject.Find("SystemSoundB").transform.GetComponent<SystemSound>();
		mainWindow = new Rect(0,0,Screen.width,Screen.height-60);
		leftChess = null;
		rightChess = null;
		LeftTalking = new Rect(222,Screen.height-180-128, 265,128);
		RightTalking = new Rect(Screen.width-222-LeftTalking.width, LeftTalking.y,LeftTalking.width, LeftTalking.height);
		rightTalkContent = new Rect(RightTalking.x, RightTalking.y +15, RightTalking.width-30, RightTalking.height-20);
		talkContent = new Rect(LeftTalking.x+35, LeftTalking.y +15, LeftTalking.width-60, LeftTalking.height-20);
		leftInfoWin = new Rect(0,Screen.height-170,500,154); 
		rightInfoWin = new Rect(Screen.width-leftInfoWin.width, leftInfoWin.y,500,154);
		leftChessPos = new Rect(0,Screen.height-310, 250,250);
		rightChessPos = new Rect(Screen.width-leftChessPos.width, leftChessPos.y,250,250);
		mirrorRect = new Rect(0,0,-1,1);
		posLeftTitle = new Rect(250.0f,Screen.height-leftInfoWin.height-15,250,30); 
		posRightTitle = new Rect(Screen.width - rightInfoWin.width+30, posLeftTitle.y,posLeftTitle.width,posLeftTitle.height);
		posLeftContent = new Rect(posLeftTitle.x, posLeftTitle.y+45,iconWidth,iconWidth);
		posRightContent = new Rect(Screen.width - rightInfoWin.width+30,posLeftContent.y,iconWidth,iconWidth);
		posAChessList = new Rect(105,Screen.height-55,40,40);
		posBChessList = new Rect(Screen.width-155,Screen.height-55,40,40);
		titleStyle = new GUIStyle();
		titleStyle.font = Title;
		titleStyle.fontSize = 28;
		titleStyle.normal.textColor = Color.white;
		manaStyle = new GUIStyle();
		manaStyle.font = Number;
		manaStyle.fontSize = 32;
		manaStyle.normal.textColor = new Color(1.0f,1.0f,1.0f,0.5f);
		manaStyle.alignment = TextAnchor.MiddleCenter;
		numberStyle[0] = new GUIStyle();
		numberStyle[1] = new GUIStyle();
		numberStyle[2] = new GUIStyle();
		numberStyle[3] = new GUIStyle();
		numberStyle[0].font = Number;
		numberStyle[1].font = Number;
		numberStyle[2].font = Number;
		numberStyle[3].font = Number;
		numberStyle[0].normal.textColor = new Color(0.8f,0.8f,0.8f,1.0f);
		numberStyle[1].normal.textColor = new Color(0.8f,0.8f,0.8f,1.0f);
		numberStyle[2].normal.textColor = new Color(0.8f,0.8f,0.8f,1.0f);
		numberStyle[3].normal.textColor = Color.white;
		numberStyle[0].fontSize = 24;
		numberStyle[1].fontSize = 30;
		numberStyle[2].fontSize = 44;
		numberStyle[3].fontSize = 22;
		numberStyle[3].alignment = TextAnchor.MiddleCenter;
		numberStyle[4] = new GUIStyle(numberStyle[3]);
		numberStyle[4].fontSize = 16;
		subStyle = new GUIStyle(numberStyle[0]);
		subStyle.fontSize = 24;
		subStyle.normal.textColor = new Color(.8f,.8f,.8f,1.0f);
		subStyle.onHover.textColor = Color.white;
		//numberStyle[1].alignment = TextAnchor.MiddleCenter;
		roundStyle = new GUIStyle(manaStyle);
		roundStyle.fontSize = 48;
		roundStyle.alignment = TextAnchor.MiddleCenter;
		roundStyle.normal.textColor = new Color(1.0f,1.0f,1.0f,0.8f);
		talkingStyle = new GUIStyle(numberStyle[3]);
		talkingStyle.normal.textColor = new Color(0.3f,0.3f,0.3f,1.0f);;
		talkingStyle.fontSize = 16;
		talkingStyle.wordWrap = true;
		talkingStyle.alignment = TextAnchor.MiddleCenter;
		currentSel = transform.GetComponent<GeneralSelection>();
		LeftRounds = 1;
		rc.SetPlayerChesses();
	}
	
	public void TalkingShit(string content, bool npc){
		if(npc)
			TalkingRight = true;
		else 
			Talking = true;
		
		talkingContent = content;
	}
	
	public void SomeoneTaking(Transform talker, string content, bool npc){
		InsertChess(talker);
		TalkingRight = false; 
		Talking = false;
		if(npc)
			TalkingRight = true;
		else 
			Talking = true;
		
		talkingContent = content;
		MainFadeIn = true;
	}
	
	public void FadeOutUI(){
		MainFadeIn = false;
	}
	
	void Awake(){
		if(Network.connections.Length>0){
			if(Network.peerType == NetworkPeerType.Server){
				playerSide = 1;
			}else if(Network.peerType == NetworkPeerType.Client){
				playerSide = 2;
			}
		}else{
			playerSide = 1;
		}
		
	}
	
	void fadeInMain(){
		ShowUI = true;
		_mainAlpha = Mathf.Lerp(_mainAlpha,1,Time.deltaTime*15);
		//Cancel =false;
	}
	
	void fadeInTarget(){
		ShowTarget = true;
		_targetAlpha = Mathf.Lerp(_targetAlpha,1,Time.deltaTime*15);
		//Cancel =false;
	}
	
	void fadeOutMain(){
		if(DelayFadeOut){
			delayCounter -= 1;
			if(delayCounter <=0)
				_mainAlpha = Mathf.Lerp(_mainAlpha,0,Time.deltaTime*fadeOutSpeed);
		}else{
			_mainAlpha = Mathf.Lerp(_mainAlpha,0,Time.deltaTime*fadeOutSpeed);
		}
		if(_mainAlpha <= 0.1f){
			ShowUI = false;
			fadeOutSpeed = 15;
			delayCounter = 200;
			DelayFadeOut = false;
			
			if(!currentSel.NpcPlaying){
				Critical = false;
				Talking = false;
				leftChess = null;
			}else{
				CriticalRight = false;
				TalkingRight = false;
				rightChess = null;
			}
		}
	}
	
	void FadeIn(){
		if(fadeIn){
			_castAlpha = Mathf.Lerp(_castAlpha,1.0f,Time.deltaTime*6.0f);
			if(_castAlpha>=0.97f){
				fadeOut = true;
				fadeIn = false;
			}
		}
	}
	
	void FadeOut(){
		if(fadeOut){
			_castAlpha = Mathf.Lerp(_castAlpha,0.0f,Time.deltaTime*6.0f);
			if(_castAlpha<=0.02f){
				fadeOut = false;
				fadeIn = true;
			}
		}
	}
	
	public void InitCameras(){
		skillCams = new Camera[rc.AllChesses.Count];
		for(int i=0; i<skillCams.Length; i++){
			skillCams[i] = Instantiate(ProgressCam, new Vector3(0,150,0), Quaternion.identity) as Camera;
		}
		progressTex = new RenderTexture[rc.AllChesses.Count];
		for(int i=0; i<progressTex.Length; i++){
			progressTex[i] = new RenderTexture(64, 64, 24);
		}
		for(int i=0; i<rc.AllChesses.Count; i++){
			skillCams[i].targetTexture =progressTex[i]; 
		}
		int t=0;
		foreach(Transform gf in rc.PlayerAChesses){
			SkillCDBarEffect gfSkillCam = skillCams[t].GetComponent<SkillCDBarEffect>();
			gfSkillCam.TopTexture = gf.GetComponent<CharacterProperty>().SmallIcon;
			gfSkillCam.m_angle = 0.0f;
			t+=1;
		}
		int s=0; 
		foreach(Transform gf in rc.PlayerBChesses){
			SkillCDBarEffect gfSkillCam = skillCams[t+s].GetComponent<SkillCDBarEffect>();
			gfSkillCam.TopTexture = gf.GetComponent<CharacterProperty>().SmallIcon;
			gfSkillCam.m_angle = 0.0f;
			s+=1;
		}
	}
	
	void fadeOutTarget(){
		if(DelayFadeOut){
			delayCounter -= 1;
			if(delayCounter <=0)
				_targetAlpha = Mathf.Lerp(_targetAlpha,0,Time.deltaTime*fadeOutSpeed);	
		}else{
			_targetAlpha = Mathf.Lerp(_targetAlpha,0,Time.deltaTime*fadeOutSpeed); 
		}
		if(_targetAlpha <= 0.1f){
			ShowTarget = false;
			fadeOutSpeed = 15;
			delayCounter = 200;
			DelayFadeOut = false;
			if(!currentSel.NpcPlaying){
				rightChess = null;
				CriticalRight = false;
			}else{
				leftChess = null;
				Critical = false;
			}
		}
	}
	
	public void InsertChess(Transform chess){
		CharacterProperty cp = chess.GetComponent<CharacterProperty>();
		if(cp.Player == playerSide){
			leftChess = chess;
		}else{
			rightChess = chess;
		}
	}
	
	public void InsertTargetChess(Transform chess){
		if(!currentSel.NpcPlaying)
			rightChess = chess;
		else
			leftChess = chess; 
	}
	
	public void SetChessNull(int side){
		if(side==1){
			leftChess = null;
		}else{
			rightChess = null;
		}
	}
	public void SetChessesNull(){
		leftChess = null;
		rightChess = null;
	}
	
	// Update is called once per frame
	void Update () {
		mousePos.x = Input.mousePosition.x;
		mousePos.y = Screen.height-Input.mousePosition.y;
		
		if(MainFadeIn)
			fadeInMain();
		else
			fadeOutMain();
		
		if(TargetFadeIn)
			fadeInTarget();
		else
			fadeOutTarget();
	
		
		FadeIn();
		FadeOut();
	}
	
	void CreateContent(Transform chess){
		int seg = 3;
		int texWidthA = 25;
		int texWidthB = 35;
		int texWidthC = 15;
		Rect startRect = new Rect();
		if(chess == leftChess)
			startRect = posLeftContent;
		else
			startRect = posRightContent;
		
		CharacterProperty cp = chess.GetComponent<CharacterProperty>();
		CharacterPassive cpp = chess.GetComponent<CharacterPassive>();
		GUI.DrawTexture(startRect, iconVault.GetIcon(BuffType.MoveRange,chess));
		Rect secCol = new Rect(startRect.x+iconWidth+seg,startRect.y-10,25,30);
		GUI.Label(secCol,cp.BuffMoveRange.ToString(),numberStyle[1]);
		Rect thirdCol = new Rect(secCol.x+texWidthA,startRect.y,iconWidth,iconWidth); 
		GUI.DrawTexture(thirdCol,iconVault.GetIcon(BuffType.AttackRange,chess));
		Rect forthCol = new Rect(thirdCol.x+iconWidth+seg,secCol.y,25,30);
		GUI.Label(forthCol,cp.BuffAtkRange.ToString(),numberStyle[1]);
		Rect fifthCol = new Rect(forthCol.x+texWidthA, startRect.y, iconWidth, iconWidth);
		GUI.DrawTexture(fifthCol,iconVault.GetIcon(BuffType.Attack,chess));
		Rect sixthCol = new Rect(fifthCol.x+iconWidth+seg, startRect.y-24,30,44);
		if(cp.Damage<=0)
			cp.Damage = 0;
		GUI.Label(sixthCol, cp.Damage.ToString(),numberStyle[2]);
		Rect seventhCol = new Rect(sixthCol.x+texWidthB,startRect.y, iconWidth, iconWidth);
		GUI.DrawTexture(seventhCol,iconVault.GetIcon(BuffType.Hp,chess));
		Rect eightCol = new Rect(seventhCol.x+iconWidth+seg, sixthCol.y,50,44);
		if(cp.Hp<=0)
			cp.Hp = 0;
		GUI.Label(eightCol, cp.Hp.ToString(),numberStyle[2]);
		Rect ninthCol = new Rect(startRect.x, startRect.y+35, iconWidth,iconWidth);
		GUI.DrawTexture(ninthCol, iconVault.GetIcon(BuffType.CriticalHit,chess));
		Rect tenthCol = new Rect(secCol.x, ninthCol.y-4,texWidthC*3,24);
		if(cp.BuffCriticalHit<=0)
			cp.BuffCriticalHit = 0;
		GUI.Label(tenthCol,cp.BuffCriticalHit.ToString(),numberStyle[0]);
		Rect elevthCol = new Rect(tenthCol.x+tenthCol.width, ninthCol.y, iconWidth,iconWidth);
		GUI.DrawTexture(elevthCol, iconVault.GetIcon(BuffType.Defense,chess));
		Rect twelvthCol = new Rect(elevthCol.x+iconWidth+seg,tenthCol.y,texWidthC*3,24);
		if(cp.ModifiedDefPow<=0)
			cp.ModifiedDefPow = 0;
		GUI.Label(twelvthCol,cp.ModifiedDefPow.ToString(),numberStyle[0]);
		Rect posPass = new Rect(twelvthCol.x+twelvthCol.width,elevthCol.y,iconWidth,iconWidth);
		if(cpp.PassiveAbility.Length>0){
			int sg = 0; int sq = 20;
			foreach(var pt in cpp.PassiveDict){
				if(pt.Value){
					GUI.DrawTexture(new Rect(posPass.x+sq*sg+seg*(sg-1),posPass.y,sq,sq),iconVault.GetPassiveTexture(pt.Key));
					sg+=1;
				}
			}
		}
		if(cp.Hp <=0){
			GUI.DrawTexture(new Rect(startRect.x,startRect.y-50,220,110),Dead);
		}
		if(Critical){
			GUI.DrawTexture(new Rect(posLeftContent.x,posLeftContent.y-70,180,90),criticalImg);
		}
		if(CriticalRight){
			GUI.DrawTexture(new Rect(posRightContent.x,posRightContent.y-70,180,90),criticalImg);
		}
		if(Talking){
			GUI.DrawTexture(LeftTalking, TalkingCol);
			GUI.TextArea(talkContent, talkingContent, 100,talkingStyle);
		}
		if(TalkingRight){
			GUI.DrawTextureWithTexCoords(RightTalking, TalkingCol,mirrorRect);
			GUI.TextArea(rightTalkContent, talkingContent, 100,talkingStyle);
		}
	}
	
	void SummonUI(int side){
		//
		Rect leftSide = new Rect();
		Rect rightSide = new Rect();
		Transform player = playerA; 
		Color leftColor = Color.red;
		Color rightColor = Color.yellow;
		if(side == 1){
			leftSide = posAChessList;
			rightSide = posBChessList;
			leftColor = RedSide;
			rightColor = YelSide;
			player = playerA;
		}else if(side ==2){
			leftColor = RedSide;
			rightColor = YelSide;
			leftSide = posBChessList;
			rightSide = posAChessList;
			player = playerB;
		}
		
		int seg = 3; int iWidth = 50; int t = 0;
		foreach(Transform gf in rc.PlayerAChesses){
			//prepare start
			CharacterProperty gfp = gf.GetComponent<CharacterProperty>();
			Transform gfSkill = gf.GetComponent<SkillSets>().Skills[0];
			SkillProperty gfSp = gfSkill.GetComponent<SkillProperty>();
			float progress = (float)(gfp.StandByRounds - gfp.WaitRounds) / (float)gfp.StandByRounds * 360.0f;
			Rect buts = new Rect();
			if(side==1)
				buts = new Rect(leftSide.x+t*(seg+iWidth),leftSide.y,iWidth,iWidth);
			else
				buts = new Rect(leftSide.x-t*(seg+iWidth),leftSide.y,iWidth,iWidth);
			// end prepare
			GUI.color = Color.white;
			if(!gfp.death && gfSp.SkillReady && gfp.CmdTimes>0)
				GUI.enabled = true;
			else
				GUI.enabled = false;
			
			
			if(GUI.Button(buts,gfp.SmallIcon,manaStyle)){
				sysSound.PlaySound(SysSoundFx.CommandClick);
			}
				
			
			if(buts.Contains(mousePos)){
				if(!gfp.death && leftSide == posAChessList){
					iconVault.ShowSkillInfo(gf, subStyle);
					currentSel.RenderSkillRange(gf);
					if(gfSp.SkillReady){
						//GUI.DrawTexture(new Rect(buts.x-2, buts.y-2, buts.width+4, buts.height+4), gfp.SmallIcon);
						GUI.Label(new Rect(buts.x-4, buts.y+2, buts.width+4, buts.height-4),"SKILL", numberStyle[3]);
					}
				}else{
					if(StopSkillRender)
						currentSel.CleanSkillMapMat();
				}
				
				if(leftSide == posAChessList && currentSel.Playing && GUI.enabled && Input.GetMouseButtonUp(0)){
					//activate skill
					//currentSel.CleanSkillMapMat();
					sysSound.PlaySound(SysSoundFx.CommandClick);
					StopSkillRender = false;
					currentSel.chess = gf;
					CastSkills(gfSkill, gf);
				}
				
				if(Input.GetMouseButtonUp(1) && !gfp.death){
					fCam.timeSeg = 0.0f;
					fCam.CamFollowMe(gf);
				}
			}
			
			GUI.enabled = true;
			Rect waitingRect = new Rect(buts.x+30, buts.y+30, 20, 20);
			if(!gfp.death){
				if(!gfSp.SkillReady)
					GUI.Label(waitingRect, gfSp.WaitingRounds.ToString(),numberStyle[3]);
				else if(gfp.InSelection && gfp.CmdTimes>0){
					GUI.color = new Color(1.0f,1.0f,1.0f,_castAlpha);
					GUI.Label(waitingRect, "up",numberStyle[4]);
				}
			}
			if(gfp.death){
				skillCams[t].GetComponent<SkillCDBarEffect>().colorSide = leftColor;
				skillCams[t].GetComponent<SkillCDBarEffect>().m_angle = progress;
				if(progress==360.0f){
					GUI.color = new Color(1.0f,1.0f,1.0f,_castAlpha);
				}
				GUI.DrawTexture(buts, progressTex[t]);
				if(gfp.Ready && buts.Contains(mousePos) && leftSide == posAChessList){
					iconVault.CreatedGFRollOver(gf,subStyle);
					GUI.DrawTexture(new Rect(buts.x-2, buts.y-2, buts.width+4, buts.height+4), gfp.SmallIcon);
					
					if(!currentSel.reviveMode){
						GUI.color = Color.clear;
						if(GUI.Button(buts, AlphaBut) && currentSel.Playing){
							sysSound.PlaySound(SysSoundFx.CommandClick);
							currentSel.CancelCmds();
							// summon command
							currentSel.summonCommand(player,gf);
							//update network UI
							currentSel.SummonNetwork();
						}
					}
				}
			}
			t+=1;
		}
		int s = 0;
		foreach(Transform gf in rc.PlayerBChesses){
			//prepare
			CharacterProperty gfp = gf.GetComponent<CharacterProperty>();
			Transform gfSkill = gf.GetComponent<SkillSets>().Skills[0];
			SkillProperty gfSp = gfSkill.GetComponent<SkillProperty>();
			float progress = (float)(gfp.StandByRounds - gfp.WaitRounds) / (float)gfp.StandByRounds * 360.0f;
			Rect buts = new Rect();
			if(side==1)
				buts = new Rect(rightSide.x-s*(seg+iWidth),rightSide.y,iWidth,iWidth);
			else
				buts = new Rect(rightSide.x+s*(seg+iWidth),rightSide.y,iWidth,iWidth);
			//end prepare
			GUI.color = Color.white;
			if(!gfp.death && gfSp.SkillReady && gfp.CmdTimes>0)
				GUI.enabled = true;
			else
				GUI.enabled = false;
			
			if(GUI.Button(buts,gfp.SmallIcon,manaStyle)){
				sysSound.PlaySound(SysSoundFx.CommandClick);
			}
				
			
			if(buts.Contains(mousePos)){
				if(!gfp.death && rightSide == posAChessList){
					iconVault.ShowSkillInfo(gf, subStyle);
					currentSel.RenderSkillRange(gf);
					
					if(gfSp.SkillReady){
						//GUI.DrawTexture(new Rect(buts.x-2, buts.y-2, buts.width+4, buts.height+4), gfp.SmallIcon);
						GUI.Label(new Rect(buts.x-4, buts.y+2, buts.width+4, buts.height-4),"SKILL", numberStyle[3]);
					}
				}else{
					if(StopSkillRender)
						currentSel.CleanSkillMapMat();
				}
				
				if(rightSide == posAChessList && currentSel.Playing && GUI.enabled && Input.GetMouseButton(0)){
					//activate skill
					StopSkillRender = false;
					currentSel.chess = gf;
					CastSkills(gfSkill, gf);
				}
				
				if(Input.GetMouseButtonUp(1)&& !currentSel.npcMode){
					fCam.timeSeg = 0.0f;
					fCam.CamFollowMe(gf);
				}
			}
			
			GUI.enabled = true;
			Rect waitingRect = new Rect(buts.x+30, buts.y+30, 20, 20);
			if(!gfp.death){
				if(!gfSp.SkillReady)
					GUI.Label(waitingRect, gfSp.WaitingRounds.ToString(),numberStyle[3]);
				else if(gfp.InSelection && gfp.CmdTimes>0){
					GUI.color = new Color(1.0f,1.0f,1.0f,_castAlpha);
					GUI.Label(waitingRect, "up",numberStyle[4]);
				}
			}
			
			if(gfp.death){
				skillCams[t+s].GetComponent<SkillCDBarEffect>().colorSide = rightColor;
				skillCams[t+s].GetComponent<SkillCDBarEffect>().m_angle = progress;
				if(progress==360.0f){
					GUI.color = new Color(1.0f,1.0f,1.0f,_castAlpha);
				}
				GUI.DrawTexture(buts, progressTex[t+s]);
				if(gfp.Ready && buts.Contains(mousePos) && rightSide == posAChessList){
					iconVault.CreatedGFRollOver(gf,subStyle);
					GUI.DrawTexture(new Rect(buts.x-2, buts.y-2, buts.width+4, buts.height+4), gfp.SmallIcon);
					if(!currentSel.reviveMode){
						GUI.color = Color.clear;
						if(GUI.Button(buts, AlphaBut) && currentSel.Playing){
							sysSound.PlaySound(SysSoundFx.CommandClick);
							currentSel.CancelCmds();
							// summon command
							currentSel.summonCommand(player,gf);
							//update network UI
							currentSel.SummonNetwork();
						}
					}
				}
			}
			s+=1;
		}
		
		if(mainWindow.Contains(mousePos) && StopSkillRender){
			currentSel.CleanSkillMapMat();
		}
	
	}
	
	
	void CastSkills(Transform skill, Transform gf){
		//MainUI mUI = transform.GetComponent<MainUI>();
		currentSel.CancelCmds();
		currentSel.SkillUINetwork();
		if(!skill.GetComponent<SkillProperty>().NeedToSelect){
			skill.GetComponent<SkillProperty>().GetRealSkillRate();
			skill.GetComponent<SkillProperty>().PassSkillRate = MapHelper.Success(skill.GetComponent<SkillProperty>().SkillRate);
			skill.GetComponent<SkillProperty>().ActivateSkill();
			gf.GetComponent<CharacterProperty>().Activated = true;
			skill.GetComponent<SkillProperty>().DefaultCDRounds();
			gf.GetComponent<CharacterProperty>().CmdTimes -= 1;
			GameObject.Find("StatusMachine").GetComponent<StatusMachine>().InBusy = true;
			currentSel.AnimStateNetWork(gf,AnimVault.AnimState.skill);
			//mUI.TurnFinished(gf, true);
			//update network
			currentSel.SkillCmdNetwork(gf,skill);
		}else{
			currentSel.skillCommand(skill);
		}
	}
	
	
	void OnGUI(){
		GUI.depth = 2;
		GUI.color = new Color(1.0f,1.0f,1.0f,_mainAlpha);
		if(sMachine.InitGame){
			if(ShowUI){
				if(leftChess!=null){
					CharacterProperty lcp = leftChess.GetComponent<CharacterProperty>();
					GUI.DrawTexture(leftInfoWin,InfoWindow);
					GUI.DrawTexture(leftChessPos,lcp.BigIcon);
					GUI.Label(posLeftTitle,lcp.NameString,titleStyle);
					CreateContent(leftChess);
				}
				if(rightChess!=null){
					CharacterProperty rcp = rightChess.GetComponent<CharacterProperty>();
					GUI.DrawTexture(rightInfoWin,InfoWindow);
					GUI.DrawTextureWithTexCoords(rightChessPos,rcp.BigIcon,mirrorRect,true);
					GUI.Label(posRightTitle,rcp.NameString,titleStyle);
					CreateContent(rightChess);
				}
			}
			
			GUI.color = new Color(1.0f,1.0f,1.0f,_targetAlpha);
			
			if(ShowTarget){
				if(!currentSel.NpcPlaying){
					if(rightChess!=null){
						CharacterProperty rcp = rightChess.GetComponent<CharacterProperty>();
						GUI.DrawTexture(rightInfoWin,InfoWindow);
						GUI.DrawTextureWithTexCoords(rightChessPos,rcp.BigIcon,mirrorRect,true);
						GUI.Label(posRightTitle,rcp.NameString,titleStyle);
						CreateContent(rightChess);
					}
				}else{
					if(leftChess!=null){
						CharacterProperty lcp = leftChess.GetComponent<CharacterProperty>();
						GUI.DrawTexture(leftInfoWin,InfoWindow);
						GUI.DrawTexture(leftChessPos,lcp.BigIcon);
						GUI.Label(posLeftTitle,lcp.NameString,titleStyle);
						CreateContent(leftChess);
					}
				}
			}
			
			GUI.color = new Color(1.0f,1.0f,1.0f,1.0f);
			playerA = rc.playerA;
			playerB = rc.playerB;
			GUI.DrawTexture(new Rect(0,Screen.height-60,Screen.width,60),UnderBar);
			GUI.DrawTexture(new Rect(0,Screen.height-60,100,60),Black);
			GUI.DrawTexture(new Rect(Screen.width-100,Screen.height-60,100,60),Black);
			if(playerSide==1){
				GUI.DrawTexture(new Rect(10,Screen.height-68,78,78),ManaA);
				GUI.DrawTexture(new Rect(Screen.width-90,Screen.height-68,78,78),ManaB);
				GUI.Box(new Rect(31, Screen.height - 50, 30,40),rc.PlayerATerritory.Count.ToString(), manaStyle);
				GUI.Box(new Rect(Screen.width - 68, Screen.height - 50, 30,40),rc.PlayerBTerritory.Count.ToString(), manaStyle);
			}else if(playerSide==2){
				GUI.DrawTexture(new Rect(10,Screen.height-68,78,78),ManaB);
				GUI.DrawTexture(new Rect(Screen.width-90,Screen.height-68,78,78),ManaA);
				GUI.Box(new Rect(31, Screen.height - 50, 30,40),rc.PlayerBTerritory.Count.ToString(), manaStyle);
				GUI.Box(new Rect(Screen.width - 68, Screen.height - 50, 30,40),rc.PlayerATerritory.Count.ToString(), manaStyle);
			}
			LeftRounds = LimitedRounds-rc.roundCounter;
			GUI.DrawTexture(new Rect(Screen.width/2-150,Screen.height-104,300,114),RoundCounterImg);
			GUI.Box(new Rect(Screen.width/2-37.5f, Screen.height-73,75,50),LeftRounds.ToString(),roundStyle);
			SummonUI(playerSide);
		}
	}
}
