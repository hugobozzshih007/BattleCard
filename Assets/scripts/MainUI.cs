using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MapUtility;
using BuffUtility;

public class MainUI : MonoBehaviour {
	
	public Transform CurrentChess = null;
	public Texture2D MoveTex, AttackTex, DefenseTex, SummonTex, SkillTex, EndTurnTex, SubWindow;
	public bool MainGuiShow;
	public bool MainGuiFade;
	public bool SubGuiFade;
	public bool Cancel = false;
	public bool IgnoreMainUI = false;
	bool moveShow;
	bool summonShow;
	bool attackShow;
	bool skillShow;
	bool goBack;
	bool currentCommand;
	float _mainAlpha;
	float _subAlpha;
	//public bool InCmd = false;
	public Font MainFont;
	public Font NormalFont;
	GUIStyle mainStyle = new GUIStyle(); 
	GUIStyle subStyle;
	Rect posMoveBt, posAttackBt, posDefenseBt, posSummonBt, posEndTurnBt, posSkillBt;
	const float leftMargin = 20.0f;
	const float topMargin = 50.0f;
	const float btSize = 55.0f;
	float rightMargin = Screen.width-20.0f;
	float bottomMargin = Screen.height-20.0f;
	float segment = 0.0f;
	float guiAlpha = 0.0f;
	Vector2 mousePos;
	RoundCounter players;
	selection currentSelect; 
	InfoUI infoUI;
	NumIconVault numIcon;
	MainInfoUI chessUI;
	InitStage init; 
	
	public bool InTutorial, InSecondTutor;
	
	// Use this for initialization
	void Start () {
		currentSelect = this.GetComponent<selection>();
		segment = btSize;
		posMoveBt =new Rect(leftMargin,topMargin,btSize,btSize);
		posSummonBt = new Rect(leftMargin,topMargin+segment,btSize,btSize); 
		posAttackBt = new Rect(leftMargin,topMargin+segment*2,btSize,btSize);
		posSkillBt = new Rect(leftMargin,topMargin+segment*3,btSize,btSize);
		posDefenseBt = new Rect(leftMargin,topMargin+segment*4,btSize,btSize);
		posEndTurnBt = new Rect(leftMargin,topMargin+segment*5,btSize,btSize); 
		MainGuiShow = false;
		mousePos = new Vector2();
		mainStyle.font = MainFont;
		mainStyle.normal.textColor = Color.white;
		mainStyle.fontSize = 20;
		subStyle = new GUIStyle(mainStyle);
		subStyle.fontSize = 16;
		subStyle.normal.textColor = new Color(.8f,.8f,.8f,1.0f);
		subStyle.onHover.textColor = Color.white;
		players = this.GetComponent<RoundCounter>();
		numIcon = this.GetComponent<NumIconVault>();
		infoUI = this.GetComponent<InfoUI>();
		chessUI = this.GetComponent<MainInfoUI>();
		InSecondTutor = false;
		init = GameObject.Find("InitStage").transform.GetComponent<InitStage>();
	}
	
	void fadeInMain(){
		MainGuiShow = true;
		_mainAlpha = Mathf.Lerp(_mainAlpha,1,Time.deltaTime*15);
		Cancel =false;
	}
	
	void fadeOutMain(){
		_mainAlpha = Mathf.Lerp(_mainAlpha,0,Time.deltaTime*15); 
		if(_mainAlpha <= 0.1f){
			MainGuiShow = false;
			if(CurrentChess!=null){
				if(CurrentChess.GetComponent<CharacterProperty>().TurnFinished && !IgnoreMainUI)
					chessUI.MainFadeIn = false;
			}
		}
	}
	
	void fadeInSub(){
		_subAlpha = Mathf.Lerp(_subAlpha,1,Time.deltaTime*15);
	}
	
	void fadeOutSub(){
		
		_subAlpha = Mathf.Lerp(_subAlpha,0,Time.deltaTime*15);
		
		if(_subAlpha <= 0.1f){
			moveShow = false;
			attackShow = false;
			skillShow = false;
			summonShow = false;
			if(CurrentChess!=null && CurrentChess.gameObject.layer==11 && currentSelect.Playing && !CurrentChess.GetComponent<CharacterProperty>().TurnFinished)
				MainGuiFade = true;
		}
	}
	/*
	bool attackable(Transform chess){
		MoveCharacter mc = transform.GetComponent<MoveCharacter>();
		bool able = false;
		if(!mc.MoveMode){
			AttackCalculation atc = new AttackCalculation(chess);
			able = (atc.GetAttableTarget(atc.Attacker).Count>0);
		}else{
			able = false;
		}
		return able;
	}
	*/
	// Update is called once per frame
	void Update () {
		mousePos.x = Input.mousePosition.x;
		mousePos.y = Screen.height-Input.mousePosition.y;
		if(!currentSelect.Playing){
			MainGuiFade = false;
			SubGuiFade = false;
		}
		if(MainGuiFade)
			fadeInMain();
		else
			fadeOutMain();
		
		if(SubGuiFade)
			fadeInSub();
		else{
			fadeOutSub();
		}
			
		//mainGuiFade = currentSelect.guiShow;
		//if(CurrentChess!=null)
		//	TurnFinished(CurrentChess);
	}
	
	Rect CreateSkillBt(Transform skill, int seg){
		SkillProperty sProperty = skill.GetComponent<SkillProperty>();
		Texture2D manaCost = numIcon.GetManaTexture(sProperty.SkillCost);
		Rect manaRect = new Rect(btSize+leftMargin+5,topMargin+segment*3+50+30*seg,16,16);
		Rect skillNameRect = new Rect(btSize+leftMargin+22,topMargin+segment*3+50+30*seg,150,30);
		GUI.DrawTexture(manaRect,manaCost);
		GUI.Label(skillNameRect,sProperty.SkillName,subStyle);
		return new Rect(manaRect.x,manaRect.y,manaRect.width+skillNameRect.width,skillNameRect.height);
	}
	
	Rect CreateGFBt(Transform gf, int seg){
		CharacterProperty gfp = gf.GetComponent<CharacterProperty>();
		Texture2D manaCost = numIcon.GetManaTexture(gfp.summonCost);
		Texture2D cdRound = numIcon.GetCDTexture(gfp.WaitRounds);
		Rect manaRect = new Rect(btSize+leftMargin,topMargin+segment+50+30*seg,16,16);
		Rect cdRect = new Rect(btSize+leftMargin+17,topMargin+segment+50+30*seg,16,16);
		Rect gfNameRect = new Rect(btSize+leftMargin+34,topMargin+segment+50+30*seg,150,30);
		GUI.DrawTexture(manaRect,manaCost);
		GUI.DrawTexture(cdRect, cdRound);
		GUI.Label(gfNameRect,gfp.NameString,subStyle);
		return new Rect(manaRect.x,manaRect.y,manaRect.width+gfNameRect.width,gfNameRect.height);
	}
	
	void CreateSummonInfo(Transform gf){
	}
	
	void ShowSkillInfo(Transform skill){
		SkillProperty sProperty = skill.GetComponent<SkillProperty>();
		int width = sProperty.info.Length*16;
		Rect infoRect = new Rect(10,6,width,26);
		GUI.Label(infoRect,sProperty.info,subStyle);
	}
	
	public void DefenseCmd(Transform chess){
		CharacterProperty chessProperty = chess.GetComponent<CharacterProperty>();
		chessProperty.Defensed = true;
		chessProperty.CmdTimes-=1;
		if(!currentSelect.npcMode)
			networkView.RPC("UpdateCmdTimes", RPCMode.Others,chess.name,chessProperty.CmdTimes);
		currentSelect.defenseCommand(chess);
		if(!currentSelect.NpcPlaying)
			currentSelect.CancelCmds();
		//update buff calculation and map material
		currentSelect.updateTerritoryMat();
		//currentSelect.updateAllCharactersPowers();
		currentSelect.DefenseNetwork();
		TurnFinished(chess, true);
	}
	
	public void TurnFinished(Transform chess, bool ifNPC){
		CharacterProperty cp = chess.GetComponent<CharacterProperty>();
		/*
		if(cp.Moved && cp.Activated && cp.Attacked)
			cp.TurnFinished = true;
		*/
		if(cp.CmdTimes<1)
			cp.TurnFinished = true;
		
		if(!cp.TurnFinished){
			//MainGuiFade = true;
		}else{
			//MainGuiFade = false;
			if(CurrentChess!=null){
				CurrentChess.gameObject.layer = 10;
				currentSelect.MoveToLayer(CurrentChess,10);
				if(ifNPC)
					currentSelect.EndTurnNetwork();
			}
		}
	}
	
	
	//Main GUI goes here
	void OnGUI(){
		GUI.depth = 1;
		GUI.backgroundColor = Color.clear;
		GUI.color = new Color(1.0f,1.0f,1.0f,_mainAlpha);
		
		if(MainGuiShow){
			if(currentSelect.chess!=null)
				CurrentChess = currentSelect.chess;
			if(CurrentChess!=null){
				CharacterProperty chessProperty = CurrentChess.GetComponent<CharacterProperty>();
				// move button
				if(/*chessProperty.Moved ||*/ chessProperty.CmdTimes < 1)
					GUI.enabled = false;
				else
					GUI.enabled = true;
				if(GUI.Button(posMoveBt,MoveTex)){
					MainGuiFade = false;
					SubGuiFade = true;
					moveShow = true;
					currentSelect.moveCommand(CurrentChess);
					currentSelect.MoveCommandNetwork();
					if(InTutorial){
						init.ShowMoveCmd = false;
						GameObject.Find("InitStage").transform.GetComponent<Tutorial>().AfterMove();
						init.ShowMap = true;
					}
				}
				GUI.enabled = true;
				//roll over move button
				if(/*!chessProperty.Moved && */ !InTutorial && chessProperty.CmdTimes < 0){
					if(posMoveBt.Contains(mousePos)){
						GUI.DrawTexture(posMoveBt,MoveTex);
						GUI.Label(new Rect(btSize+leftMargin+5,topMargin+16,100,30),"move", mainStyle); 
					}
				}
				// summon button
				if(!chessProperty.Summoner || init.stage==1)
					GUI.enabled = false;
				else
					GUI.enabled = true;
				
				if(GUI.Button(posSummonBt, SummonTex)){
					MainGuiFade = false;
					SubGuiFade = true;
					summonShow = true;
					currentSelect.CleanMapsMat();
				}
				GUI.enabled = true;
				//roll over summon button
				if(chessProperty.Summoner && init.stage!=1){
					if(posSummonBt.Contains(mousePos)){
						GUI.DrawTexture(posSummonBt,SummonTex);
						GUI.Label(new Rect(btSize+leftMargin+5,topMargin+segment+16,100,30),"summon", mainStyle); 
					}
				}
				// Attack button
				if(chessProperty.Attacked || !MapHelper.Attackable(CurrentChess) || chessProperty.CmdTimes < 1)
					GUI.enabled = false;
				else
					GUI.enabled = true;
				
				if(GUI.Button(posAttackBt, AttackTex)){
					MainGuiFade = false;
					SubGuiFade = true;
					attackShow = true;
					currentSelect.attackCommand(CurrentChess);
					currentSelect.AttackCommandNetwork();
					if(InSecondTutor){
						init.ShowAtk = false;
					}
				}
				GUI.enabled = true;
				//roll over attack button
				if(!chessProperty.Attacked && MapHelper.Attackable(CurrentChess)){
					if(posAttackBt.Contains(mousePos)){
						GUI.DrawTexture(posAttackBt,AttackTex);
						GUI.Label(new Rect(btSize+leftMargin+5,topMargin+segment*2+16,100,30),"attack", mainStyle); 
					}
				}
				// skill button
				if(chessProperty.Activated || init.stage==1 || chessProperty.CmdTimes<1)
					GUI.enabled = false;
				else
					GUI.enabled = true;
				if(GUI.Button(posSkillBt, SkillTex)){
					MainGuiFade = false;
					SubGuiFade = true;
					skillShow = true;
				}
				GUI.enabled = true;
				//roll over skill button
				if(!chessProperty.Activated && init.stage!=1 && chessProperty.CmdTimes>0){
					if(posSkillBt.Contains(mousePos)){
						GUI.DrawTexture(posSkillBt,SkillTex);
						GUI.Label(new Rect(btSize+leftMargin+5,topMargin+segment*3+16,100,30),"skill", mainStyle); 
					}
				}
				//Defense button
				
				if(init.ShowBuff || InSecondTutor || chessProperty.CmdTimes < 1)
					GUI.enabled = false;
				else
					GUI.enabled = true;
				
				if(GUI.Button(posDefenseBt, DefenseTex)){
					DefenseCmd(CurrentChess);
					if(init.ShowDef){
						init.ShowDef = false;
						InTutorial = false;
						InSecondTutor = true;
					}
				}
				GUI.enabled = true;
				//roll over defense button
				if(chessProperty.CmdTimes>0){
					if(posDefenseBt.Contains(mousePos)){
							GUI.DrawTexture(posDefenseBt,DefenseTex);
							GUI.Label(new Rect(btSize+leftMargin+5,topMargin+segment*4+16,100,30),"defence", mainStyle); 
						}
				}
				// End turn button
				if(InTutorial)
					GUI.enabled = false;
				else 
					GUI.enabled = true;
				
				if(GUI.Button(posEndTurnBt, EndTurnTex)){
					MainGuiFade = false;
					//SubGuiFade = true;
					chessProperty.Activated = true;
					chessProperty.Attacked = true;
					chessProperty.Moved = true;
					chessProperty.CmdTimes = 0;
					chessProperty.TurnFinished = true;
					CurrentChess.gameObject.layer = 10;
					currentSelect.MoveToLayer(CurrentChess,10);
					currentSelect.EndTurnNetwork();
				}
				
				GUI.enabled = true;
				
				//roll over end turn button
				if(!InTutorial){
					if(posEndTurnBt.Contains(mousePos)){
						GUI.DrawTexture(posEndTurnBt,EndTurnTex);
						GUI.Label(new Rect(btSize+leftMargin+5,topMargin+segment*5+13,100,30),"end turn", mainStyle); 
					}
				}
				
			}
		}
		GUI.color = new Color(1.0f,1.0f,1.0f,_subAlpha);
		
		if(moveShow){
			GUI.DrawTexture(posMoveBt,MoveTex);
			GUI.Label(new Rect(btSize+leftMargin+5,topMargin+16,100,30),"move", mainStyle);
			if(Input.GetMouseButtonDown(0) && posMoveBt.Contains(mousePos)){
				Cancel = true;
				MainGuiFade = true;
				SubGuiFade = false;
				TurnFinished(CurrentChess, true);
			}
		}
		if(attackShow){
			GUI.DrawTexture(posAttackBt,AttackTex);
			GUI.Label(new Rect(btSize+leftMargin+5,topMargin+segment*2+16,100,30),"attack", mainStyle);
			if(Input.GetMouseButtonDown(0) && posAttackBt.Contains(mousePos)){
				Cancel = true;
				MainGuiFade = true;
				SubGuiFade = false;
				TurnFinished(CurrentChess, true);
			}
		}
		if(summonShow){
			Rect posSummonList = new Rect(posSummonBt.x+posSummonBt.width/2, posSummonBt.y+posSummonBt.height/2,200.0f,200.0f);
			GUI.DrawTexture(posSummonList,SubWindow);
			GUI.DrawTexture(posSummonBt,SummonTex);
			GUI.Label(new Rect(btSize+leftMargin+5,topMargin+segment+16,100,30),"summon", mainStyle);
			if(Input.GetMouseButtonDown(0) && posSummonBt.Contains(mousePos)){
				Cancel = true;
				MainGuiFade = true;
				SubGuiFade = false;
			}
			IList soldiers = new List<Transform>();
			if(CurrentChess.GetComponent<CharacterProperty>().Player == 1){
				foreach(Transform gf in players.PlayerAChesses){
					if(!gf.GetComponent<CharacterProperty>().Summoner)
						soldiers.Add(gf);
				}
			}else if(CurrentChess.GetComponent<CharacterProperty>().Player == 2){
				foreach(Transform gf in players.PlayerBChesses){
					if(!gf.GetComponent<CharacterProperty>().Summoner)
						soldiers.Add(gf);
				}
			}
			int seg = 0;
			foreach(Transform gf in soldiers){
				CharacterProperty gfp = gf.GetComponent<CharacterProperty>();
				if(gfp.Ready && (gfp.summonCost<=currentSelect.player.GetComponent<ManaCounter>().Mana) && gfp.death)
					GUI.enabled = true;
				else
					GUI.enabled = false;
				Rect posGFBt = CreateGFBt(gf,seg);
				if(posGFBt.Contains(mousePos)){
					infoUI.CreatedGFRollOver(gf,subStyle);
				}
				if(GUI.Button(posGFBt,"",subStyle)){
					currentSelect.summonCommand(CurrentChess,gf);
					//update network UI
					currentSelect.SummonNetwork();
				}
				seg+=1;
			}
		}
		if(skillShow){
			Rect posSkillsBt = new Rect(posSkillBt.x+posSkillBt.width/2, posSkillBt.y+posSkillBt.height/2,200.0f,100.0f);
			GUI.DrawTexture(posSkillsBt,SubWindow);
			GUI.DrawTexture(posSkillBt,SkillTex);
			GUI.Label(new Rect(btSize+leftMargin+5,topMargin+segment*3+16,100,30),"skill", mainStyle);
			if(Input.GetMouseButtonDown(0) && posSkillBt.Contains(mousePos)){
				Cancel = true;
				MainGuiFade = true;
				SubGuiFade = false;
			}
			Transform[] skills = CurrentChess.GetComponent<SkillSets>().Skills;
			int seg =0;
			if(skills.Length>0){
				foreach(Transform skill in skills){
					if(currentSelect.player.GetComponent<ManaCounter>().Mana>=skill.GetComponent<SkillProperty>().SkillCost)
						GUI.enabled = true;
					else
						GUI.enabled = false;
					Rect skillBtRect = CreateSkillBt(skill,seg);
					if(skillBtRect.Contains(mousePos)){
						ShowSkillInfo(skill);
					}
					if(GUI.Button(skillBtRect,"",subStyle)){
						currentSelect.SkillUINetwork();
						if(!skill.GetComponent<SkillProperty>().NeedToSelect){
							skill.GetComponent<SkillProperty>().GetRealSkillRate();
							skill.GetComponent<SkillProperty>().PassSkillRate = MapHelper.Success(skill.GetComponent<SkillProperty>().SkillRate);
							skill.GetComponent<SkillProperty>().ActivateSkill();
							currentSelect.player.GetComponent<ManaCounter>().Mana -= skill.GetComponent<SkillProperty>().SkillCost;
							CurrentChess.GetComponent<CharacterProperty>().Activated = true;
							CurrentChess.GetComponent<CharacterProperty>().CmdTimes -= 1;
							currentSelect.AnimStateNetWork(CurrentChess,AnimVault.AnimState.skill);
							SubGuiFade = false;
							TurnFinished(CurrentChess, true);
							//update network
							currentSelect.SkillCmdNetwork(CurrentChess,skill);
						}else{
							currentSelect.skillCommand(skill);
						}
					}
					seg +=1;
					GUI.enabled = true;
					
				}
			}
		}
	}
	
	[RPC]
	void UpdateCmdTimes(string chessName, int times){
		Transform chess = GameObject.Find(chessName).transform;
		chess.GetComponent<CharacterProperty>().CmdTimes = times;
	}
}
