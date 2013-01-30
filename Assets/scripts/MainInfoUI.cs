using UnityEngine;
using System.Collections;

public class MainInfoUI : MonoBehaviour {
	public Texture2D InfoWindow, UnderBar,Black, ManaA, ManaB, RoundCounterImg, Dead, criticalImg;
	bool ShowUI = false;
	bool ShowTarget = false;
	public bool MainFadeIn = false;
	public bool TargetFadeIn = false;
	public bool DelayFadeOut = false; 
	public Font Title, Number;
	public bool Critical = false;
	public bool CriticalRight = false;
	public int fadeOutSpeed = 15;
	Transform leftChess, rightChess, playerA, playerB;
	Rect leftInfoWin, rightInfoWin, leftChessPos, rightChessPos, mirrorRect, posLeftTitle, posRightTitle, posLeftContent, posRightContent, posAChessList, posBChessList;
	public int playerSide;
	GUIStyle titleStyle, manaStyle, roundStyle;
	GUIStyle[] numberStyle = new GUIStyle[3];
	float iconWidth = 20.0f;
	InfoUI iconVault; 
	int allMaps;
	RoundCounter rc;
	float _mainAlpha, _targetAlpha;
	int delayCounter = 600;
	// Use this for initialization
	void Start () {
		leftChess = null;
		rightChess = null;
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
		manaStyle.fontSize = 36;
		manaStyle.normal.textColor = new Color(1.0f,1.0f,1.0f,0.5f);
		numberStyle[0] = new GUIStyle();
		numberStyle[1] = new GUIStyle();
		numberStyle[2] = new GUIStyle();
		numberStyle[0].font = Number;
		numberStyle[1].font = Number;
		numberStyle[2].font = Number;
		numberStyle[0].normal.textColor = new Color(0.5f,0.5f,0.5f,1.0f);
		numberStyle[1].normal.textColor = new Color(0.5f,0.5f,0.5f,1.0f);
		numberStyle[2].normal.textColor = new Color(0.5f,0.5f,0.5f,1.0f);
		numberStyle[0].fontSize = 24;
		numberStyle[1].fontSize = 30;
		numberStyle[2].fontSize = 44;
		iconVault = transform.GetComponent<InfoUI>();
		rc = transform.GetComponent<RoundCounter>();
		roundStyle = new GUIStyle(manaStyle);
		roundStyle.fontSize = 48;
		roundStyle.alignment = TextAnchor.MiddleCenter;
		roundStyle.normal.textColor = new Color(1.0f,1.0f,1.0f,0.8f);
	}
	
	void Awake(){
		if(Network.peerType == NetworkPeerType.Server){
			playerSide = 1;
		}else if(Network.peerType == NetworkPeerType.Client){
			playerSide = 2;
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
			leftChess = null;
			fadeOutSpeed = 15;
			delayCounter = 600;
			DelayFadeOut = false;
			Critical = false;
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
			rightChess = null;
			fadeOutSpeed = 15;
			delayCounter = 600;
			DelayFadeOut = false;
			CriticalRight = false;
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
			rightChess = chess;
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
		if(MainFadeIn)
			fadeInMain();
		else
			fadeOutMain();
		
		if(TargetFadeIn)
			fadeInTarget();
		else
			fadeOutTarget();
	
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
		GUI.DrawTexture(seventhCol,iconVault.GetIcon(BuffType.Defense,chess));
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
		GUI.DrawTexture(elevthCol, iconVault.GetIcon(BuffType.SkillRate,chess));
		Rect twelvthCol = new Rect(elevthCol.x+iconWidth+seg,tenthCol.y,texWidthC*3,24);
		if(cp.BuffSkillRate<=0)
			cp.BuffSkillRate = 0;
		GUI.Label(twelvthCol,cp.BuffSkillRate.ToString(),numberStyle[0]);
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
	}
	
	
	void OnGUI(){
		GUI.color = new Color(1.0f,1.0f,1.0f,_mainAlpha);
		
		if(ShowUI){
			if(leftChess!=null){
				CharacterProperty lcp = leftChess.GetComponent<CharacterProperty>();
				GUI.Label(posLeftTitle,lcp.NameString,titleStyle);
				GUI.DrawTexture(leftInfoWin,InfoWindow);
				GUI.DrawTexture(leftChessPos,lcp.BigIcon);
				CreateContent(leftChess);
			}
		}
		
		GUI.color = new Color(1.0f,1.0f,1.0f,_targetAlpha);
		
		if(ShowTarget){
			if(rightChess!=null){
				CharacterProperty rcp = rightChess.GetComponent<CharacterProperty>();
				GUI.Label(posRightTitle,rcp.NameString,titleStyle);
				GUI.DrawTexture(rightInfoWin,InfoWindow);
				GUI.DrawTextureWithTexCoords(rightChessPos,rcp.BigIcon,mirrorRect,true);
				CreateContent(rightChess);
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
			GUI.Box(new Rect(40, Screen.height - 50, 30,40),playerA.GetComponent<ManaCounter>().Mana.ToString(), manaStyle);
			GUI.Box(new Rect(Screen.width - 62, Screen.height - 50, 30,40),playerB.GetComponent<ManaCounter>().Mana.ToString(), manaStyle);
		}else if(playerSide==2){
			GUI.DrawTexture(new Rect(10,Screen.height-68,78,78),ManaB);
			GUI.DrawTexture(new Rect(Screen.width-90,Screen.height-68,78,78),ManaA);
			GUI.Box(new Rect(40, Screen.height - 50, 30,40),playerB.GetComponent<ManaCounter>().Mana.ToString(), manaStyle);
			GUI.Box(new Rect(Screen.width - 62, Screen.height - 50, 30,40),playerA.GetComponent<ManaCounter>().Mana.ToString(), manaStyle);
		}
		GUI.DrawTexture(new Rect(Screen.width/2-150,Screen.height-104,300,114),RoundCounterImg);
		GUI.Box(new Rect(Screen.width/2-37.5f, Screen.height-73,75,50),rc.roundCounter.ToString(),roundStyle);
		//
		if(playerSide==1){
			int seg = 3; int iWidth = 50; int t = 0;
			foreach(Transform gf in rc.PlayerAChesses){
				CharacterProperty gfp = gf.GetComponent<CharacterProperty>();
				if(!gfp.death)
					GUI.enabled = true;
				else
					GUI.enabled = false;
				GUI.Button(new Rect(posAChessList.x+t*(seg+iWidth),posAChessList.y,iWidth,iWidth),gfp.SmallIcon,manaStyle);
				t+=1;
			}
			int s = 0;
			foreach(Transform gf in rc.PlayerBChesses){
				CharacterProperty gfp = gf.GetComponent<CharacterProperty>();
				if(!gfp.death)
					GUI.enabled = true;
				else
					GUI.enabled = false;
				GUI.Button(new Rect(posBChessList.x-s*(seg+iWidth),posBChessList.y,iWidth,iWidth),gfp.SmallIcon,manaStyle);
				s+=1;
			}
		}else if(playerSide==2){
			int seg = 3; int iWidth = 50; int t = 0;
			foreach(Transform gf in rc.PlayerBChesses){
				CharacterProperty gfp = gf.GetComponent<CharacterProperty>();
				if(!gfp.death)
					GUI.enabled = true;
				else
					GUI.enabled = false;
				GUI.Button(new Rect(posAChessList.x+t*(seg+iWidth),posAChessList.y,iWidth,iWidth),gfp.SmallIcon,manaStyle);
				t+=1;
			}
			int s = 0;
			foreach(Transform gf in rc.PlayerAChesses){
				CharacterProperty gfp = gf.GetComponent<CharacterProperty>();
				if(!gfp.death)
					GUI.enabled = true;
				else
					GUI.enabled = false;
				GUI.Button(new Rect(posBChessList.x-s*(seg+iWidth),posBChessList.y,iWidth,iWidth),gfp.SmallIcon,manaStyle);
				s+=1;
			}
		}
	}
}
