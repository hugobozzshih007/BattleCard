using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TeamLayout : MonoBehaviour {
	public Texture2D BackGround, ReadyBut, RemoveBut;
	public LayerMask MaskPillar;
	public LayerMask MaskCharacter;
	public Font StringStyle; 
	public Rect PillarRect = new Rect(250.0f,56.0f, 779.0f, 416.0f);
	float castLength = 80.0f;
	Vector2 mousePos; 
	IList rectList = new List<Rect>();
	IList textureList = new List<Texture2D>();
	bool changingGF = true;
	GuardianUIData[] guardianArray; 
	GuardianUIData currentGF, oldGF;
	Transform currentPillar, oldPillar, returnGF;
	Rect bigIconRect = new Rect(33.0f/1280.0f*Screen.width,99.0f/720.0f*Screen.height,198.0f,198.0f);
	const float gfWidth = 80.0f;
	const float gfHeight = 80.0f;
	const float interWidth = 90.0f;
	const float interHeight = 80.0f;
	float startX = 280.0f/1280.0f*Screen.width; 
	float startY = 525.0f/720.0f*Screen.height;  
	Rect currentRect = new Rect(); 
	Rect bgRect = new Rect(.0f,.0f, Screen.width, Screen.height);
	bool dragging = false;
	Transform playerData, hittedPillar;
	int dataLength;
	Transform pillars;
	bool placing = true;
	bool pause = false;
	float timeSeg = 0.0f; 
	float pauseTime = 1.0f;
	Rect noWhere = new Rect(-50.0f, -130.0f, 50.0f, 130.0f);
	Rect nameRect = new Rect(33.0f/1280.0f*Screen.width, 312.0f/720.0f*Screen.height, 198.0f, 22.0f);
	Rect infoStartRect = new Rect(50.0f/1280.0f*Screen.width, 420.0f/720.0f*Screen.height, 168.0f, 20.0f);
	Rect[] infoRects = new Rect[9];
	Rect[] staticRects = new Rect[9];
	Rect[] passiveRects = new Rect[5];
	Rect[] teamRects = new Rect[7];
	Rect equipRect = new Rect(1075.0f/1280.0f*Screen.width, 360.0f/720.0f*Screen.height, 168.0f, 20.0f);
	Rect costRect = new Rect(1075.0f/1280.0f*Screen.width, 380.0f/720.0f*Screen.height, 168.0f, 20.0f);
	Rect costNumRect = new Rect(1190.0f/1280.0f*Screen.width, 380.0f/720.0f*Screen.height, 20.0f, 20.0f);
	Rect readyRect = new Rect(888.0f/1280.0f*Screen.width, 640.0f/720.0f*Screen.height, 122.0f, 30.0f);
	Rect removeRect, keepRect;
	InfoUI iconVault; 
	GUIStyle gStyle = new GUIStyle();
	GUIStyle bStyle = new GUIStyle();
	GUIStyle cStyle = new GUIStyle();
	GuardianStorage guardians;
	string[] teamNames =new string[7];
	bool ifPlaced = false;
	Transform[] skillArray;
	SystemSound sSoundOver;
	SystemSound sSoundClick;
	SystemSound sSoundOpen;
	SystemSound sSoundGame;
	LoadingFadeIn lf;
	// Use this for initialization
	void Start () {
		lf = GameObject.Find("LoadingScreen").GetComponent<LoadingFadeIn>();
		iconVault = transform.GetComponent<InfoUI>();
		pillars = GameObject.Find("Pillars").transform;
		Rect zeroGFRect = new Rect(startX, startY, gfWidth, gfHeight);
		print("this is level, "+Application.loadedLevelName);
		playerData = GameObject.Find("PlayerData").transform;
		if(playerData!=null){
			guardians = playerData.GetComponent<GuardianStorage>();
			dataLength = guardians.Guardians.Count;
			guardianArray = new GuardianUIData[dataLength];
			skillArray = new Transform[guardians.SkillVault.Count];
			guardians.SkillVault.CopyTo(skillArray,0);
			for(int i=0; i<dataLength; i++){
				Transform gf = guardians.Guardians[i] as Transform;
				guardianArray[i] = new GuardianUIData(gf);
				//for leader
				if(guardianArray[i].Chess.GetComponent<CharacterProperty>().Summoner)
					guardianArray[i].Placed = true;
				Rect gfRect = new Rect();
				if(i>7)
					gfRect = new Rect(startX+interWidth*(i-8), startY+interHeight, gfWidth, gfHeight);
				else
					gfRect = new Rect(startX+interWidth*i, startY, gfWidth, gfHeight);
				guardianArray[i].DragRect = gfRect;  
				guardianArray[i].OriginRect = gfRect;
				guardianArray[i].TrueOriginRect = gfRect;
			}
			for(int i=0; i<7; i++){
				teamRects[i] = new Rect(1080.0f/1280.0f*Screen.width, 126.0f/720.0f*Screen.height+26.0f/720.0f*Screen.height*i, 168.0f, 20.0f);
			}
		}
		gStyle.alignment = TextAnchor.MiddleCenter;
		gStyle.font = StringStyle;
		gStyle.fontSize = 20;
		gStyle.normal.textColor = Color.white;
		
		bStyle.alignment = TextAnchor.UpperLeft;
		bStyle.font = StringStyle;
		bStyle.fontSize = 18;
		bStyle.normal.textColor = Color.white;
		
		cStyle.alignment = TextAnchor.UpperLeft;
		cStyle.font = StringStyle;
		cStyle.fontSize = 16;
		cStyle.normal.textColor = new Color(0.75f,0.75f,0.75f,1.0f);
		cStyle.wordWrap = true;
		
		for(int i=0; i<infoRects.Length; i++){
			infoRects[i] = new Rect(infoStartRect.x, infoStartRect.y+28.0f*i, infoStartRect.width, infoStartRect.height);
			staticRects[i] = new Rect(176.0f, infoStartRect.y+28.0f*i, 40.0f, 20.0f);
		}
		for(int i=0; i<5; i++){
			passiveRects[i] = new Rect(156.0f+i*22.0f, infoRects[8].y, 20.0f, 20.0f);
		}
		UpdateTeamSet();
		
		sSoundOver = GameObject.Find("SystemSoundOver").GetComponent<SystemSound>();
		sSoundClick = GameObject.Find("SystemSoundClick").GetComponent<SystemSound>();
		sSoundOpen = GameObject.Find("SystemSoundOpen").GetComponent<SystemSound>();
		sSoundGame = GameObject.Find("SystemSoundGame").GetComponent<SystemSound>();
		
		oldGF = new GuardianUIData(guardians.FirstLeader);
	}
	
	bool CheckFrontPillar(string pillar){
		bool check = false;
		if(pillar=="pillar04" || pillar=="pillar05" || pillar=="pillar06")
			check = true;
		return check;
	}
	
	void ChangeLayer(Transform chess, int layer){
		if(chess!=null){
			chess.gameObject.layer = layer;
			if(chess.childCount>0){
				for(int i=0; i<chess.childCount; i++){
					chess.GetChild(i).gameObject.layer = layer;
					if(chess.GetChild(i).childCount>0){
						for(int j=0; j<chess.GetChild(i).childCount; j++){
							chess.GetChild(i).GetChild(j).gameObject.layer = layer;
						}
					}
				}
			}
		}
	}
	void GetPillar(){
		if(PillarRect.Contains(mousePos)){
			Ray ray = Camera.mainCamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, castLength, MaskCharacter)){
				currentPillar = hit.transform;
				ChangeLayer(currentPillar, 11);
				
				if(oldPillar!=currentPillar){
					if(oldPillar!=null){
						if(CheckFrontPillar(oldPillar.name))
							ChangeLayer(oldPillar, 22);
						else
							ChangeLayer(oldPillar, 12);
					}
				}
			}else{
				if(currentPillar!=null){
					if(CheckFrontPillar(currentPillar.name))
						ChangeLayer(currentPillar, 22);
					else
						ChangeLayer(currentPillar, 12);
				}
				currentPillar = null;
			}
		}else{
			if(currentPillar!=null){
				if(CheckFrontPillar(currentPillar.name))
					ChangeLayer(currentPillar, 22);
				else
					ChangeLayer(currentPillar, 12);
			}
			currentPillar = null;
		}
	}
	
	void BasicInfo(GuardianUIData gfData){
		GUI.Label(infoRects[0],"Def Powers:", bStyle);
		GUI.Label(infoRects[1],"Damage:", bStyle);
		GUI.Label(infoRects[2],"Move Range:", bStyle);
		GUI.Label(infoRects[3],"Atk Range:", bStyle);
		GUI.Label(infoRects[4],"Skill Rate:", bStyle);
		GUI.Label(infoRects[5],"Critical Rate:", bStyle);
		GUI.Label(infoRects[6],"Hp:", bStyle);
		GUI.Label(infoRects[7],"CD Rounds:", bStyle);
		GUI.Label(infoRects[8],"Pessive:", bStyle);
		if(gfData!=null){
			CharacterProperty property = gfData.Chess.GetComponent<CharacterProperty>();
			CharacterPassive passive = gfData.Chess.GetComponent<CharacterPassive>();
			Texture2D bigIcon = property.BigIcon;
			GUI.DrawTexture(bigIconRect, bigIcon);
			GUI.Label(nameRect, property.NameString, gStyle);
			//statics
			GUI.Label(staticRects[0], property.defPower.ToString(), bStyle);
			GUI.Label(staticRects[1], property.atkPower.ToString(), bStyle);
			GUI.Label(staticRects[2], property.moveRange.ToString(), bStyle);
			GUI.Label(staticRects[3], property.atkRange.ToString(), bStyle);
			GUI.Label(staticRects[4], property.SkillRate.ToString(), bStyle);
			GUI.Label(staticRects[5], property.CriticalhitChance.ToString(), bStyle);
			GUI.Label(staticRects[6], property.MaxHp.ToString(), bStyle);
			GUI.Label(staticRects[7], property.StandByRounds.ToString(), bStyle);
			
			if(passive.PassiveAbility.Length>0){
				for(int i=0; i<passive.PassiveAbility.Length; i++){
					GUI.DrawTexture(passiveRects[i], iconVault.GetPassiveTexture(passive.PassiveAbility[i]));
				}
			}
			
			GUI.Label(equipRect, gfData.Skill.GetComponent<SkillProperty>().SkillName, bStyle);
			GUI.Label(costRect, "CD Rounds:", bStyle);
			GUI.Label(costNumRect, gfData.Skill.GetComponent<SkillProperty>().CoolDownRounds.ToString(), bStyle);
			GUI.TextArea(new Rect(costRect.x, costRect.y+150.0f,160.0f, 100.0f),gfData.Skill.GetComponent<SkillProperty>().info,500,cStyle);
		}
		if(playerData!=null){
			/*
			for(int i=0; i<guardians.SkillVault.Count; i++){
				//GUI.Label(skillRects[i], skillArray[i].GetComponent<SkillProperty>().SkillName, bStyle);
				//roll over info
				if(skillRects[i].Contains(mousePos)){
					GUI.Label(costRect, "CD Rounds:", bStyle);
					GUI.Label(costNumRect, skillArray[i].GetComponent<SkillProperty>().CoolDownRounds.ToString(), bStyle);
					GUI.TextArea(new Rect(costRect.x, costRect.y+28.0f,180.0f, 80.0f),skillArray[i].GetComponent<SkillProperty>().info,500,cStyle); 
				}
			}*/
		}
	}
	
	public void RemovePillarGF(Transform pillar){
		returnGF = pillar.GetComponent<PillarData>().GuardianForce;
		foreach(GuardianUIData gf in guardianArray){
			if(returnGF == gf.Chess){
				currentGF = gf;
				pillar.GetComponent<PillarData>().RemoveGuardian();
				gf.Placed = false;
				break;
			}
		}
		UpdateTeamSet();
	}
	
	void PillarSelection(){
		Ray ray = Camera.mainCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit, castLength, MaskPillar)){
			if(Input.GetMouseButtonDown(1)){
				sSoundClick.PlaySound();
				Transform pillar = hit.transform;
				print(pillar);
				if(pillar.GetComponent<PillarData>().WithGuardian){
					returnGF = pillar.GetComponent<PillarData>().GuardianForce;
					foreach(GuardianUIData gf in guardianArray){
						if(returnGF == gf.Chess){
							currentGF = gf;
							pillar.GetComponent<PillarData>().RemoveGuardian();
							gf.Placed = false;
							break;
						}
					}
				}
				UpdateTeamSet();
			}
		}
	}
	
	void UpdateTeamSet(){
		teamNames = new string[7];
		Transform pillars = GameObject.Find("Pillars").transform;
		IList gfNames = new List<string>(); 
		for(int i=0; i<pillars.childCount; i++){
			if(pillars.GetChild(i).GetComponent<PillarData>().WithGuardian || pillars.GetChild(i).GetComponent<PillarData>().LeadingPillar){
				  gfNames.Add(pillars.GetChild(i).GetComponent<PillarData>().GuardianForce.GetComponent<CharacterProperty>().NameString);
			}
		}
		gfNames.CopyTo(teamNames,0);
	}
	
	void DrawTeamSet(){
		for(int i=0; i<7; i++){
			GUI.Label(teamRects[i], (string)teamNames[i],bStyle);
		}
	}
	
	void PlaceTheChess(GuardianUIData guardian, Transform target){
		
		if(placing && !guardian.Placed && target!=null){
			PillarData pData = target.GetComponent<PillarData>();
			if(pData.WithGuardian){
				RemovePillarGF(target);
			}
			sSoundOpen.PlaySound(SysSoundFx.OpenPrize);
			Transform newChess = Instantiate(guardian.ChessDisplay, target.transform.position, Quaternion.identity) as Transform;
			ChangeLayer(newChess, 21);
			newChess.Translate(new Vector3(0.0f,1.5f,0.0f));
			pData.PlaceGuardian(guardian, newChess);
			pData.Display_Model = newChess;
			guardian.Placed = true;
			UpdateTeamSet();
		}
	}
	
	void ReadyToGO(){
		Transform leadingPillar = GameObject.Find("leadingPillar").transform;
		guardians.SelectedSummoner = leadingPillar.GetComponent<PillarData>().GuardianForce;
		Transform pillars = GameObject.Find("Pillars").transform;
		IList soilders = new List<Transform>();
		guardians.SelectedGFs.Clear();
		guardians.UnSelectedGFs.Clear();
		for(int i=1; i<pillars.childCount; i++){
			if(pillars.GetChild(i).GetComponent<PillarData>().WithGuardian){
				soilders.Add(pillars.GetChild(i).GetComponent<PillarData>().GuardianForce);
			}
		}
		if(soilders.Count>0){
			foreach(Transform gf in soilders){
				guardians.SelectedGFs.Add(gf);
			}
		}
				
		foreach(Transform gf in guardians.Guardians){
			if(!guardians.SelectedGFs.Contains(gf) && !gf.GetComponent<CharacterProperty>().Summoner)
				guardians.UnSelectedGFs.Add(gf);
		}
				
		lf.ActivateLoading("summon_land");
	}
	
	// Update is called once per frame
	void Update () {
		mousePos.x = Input.mousePosition.x;
		mousePos.y = Screen.height-Input.mousePosition.y;
		PillarSelection();
		if(Input.GetMouseButtonDown(0))
			dragging = true;
		if(Input.GetMouseButtonUp(0) && dragging)
			dragging = false;
		if(changingGF){
			for(int i=0;i<dataLength; i++){
				if(guardianArray[i].OriginRect.Contains(mousePos) && dragging){
					sSoundOver.PlaySound();
					currentGF = guardianArray[i];
					oldGF = currentGF;
					guardianArray[i].ShowGUI = false;
					changingGF = false;
					break;
				}else{
					guardianArray[i].ShowGUI = true;
				}
			}
		}
		if(!dragging && currentGF!=null){
			changingGF = true;
			PlaceTheChess(currentGF, currentPillar);
			currentGF = null;
		}
		if(dragging && currentGF!=null){
			currentGF.DragRect = new Rect(mousePos.x-gfWidth/2, mousePos.y-gfHeight/2, gfWidth, gfHeight);
			placing = true;
			GetPillar();
			oldPillar = currentPillar; 
		}
		if(!dragging){
			for(int i=0; i<pillars.childCount; i++){
				if(pillars.GetChild(i).gameObject.layer==11){
					
					if(CheckFrontPillar(pillars.GetChild(i).name))
						ChangeLayer(pillars.GetChild(i), 22);
					else
						ChangeLayer(pillars.GetChild(i), 12);
				}
			}
			placing = false;
		}
		foreach(GuardianUIData gData in guardianArray){
			if(gData.Placed){
				ifPlaced = true;
				break;
			}else{
				ifPlaced = false;
			}
		}
		
		if(pause){
			timeSeg += Time.deltaTime/pauseTime;
			if(timeSeg >= 0.9f){
				timeSeg = 0.0f; 
				pause = false; 
				ReadyToGO();
			}
		}
	}
	
	void OnGUI(){
		GUI.depth = 2;  
		GUI.backgroundColor = Color.clear;
		GUI.DrawTexture(bgRect, BackGround);
		//show guardians  
		GUI.depth = 1;
		if(dataLength>0){
			for(int i=0;i<dataLength; i++){
				if(guardianArray[i].ShowGUI && !guardianArray[i].Placed){
					GUI.enabled = true;
					guardianArray[i].OriginRect = guardianArray[i].TrueOriginRect;
					GUI.Button(guardianArray[i].OriginRect, guardianArray[i].Icon, gStyle);
				}else{
					GUI.enabled = false;
					GUI.Button(guardianArray[i].OriginRect, guardianArray[i].Icon, gStyle);
				}
			}
		}
		GUI.enabled = true;
		if(currentGF!=null && !currentGF.ShowGUI &&!currentGF.Placed){
			GUI.DrawTexture(currentGF.DragRect, currentGF.Icon);
		}
		BasicInfo(oldGF);
		DrawTeamSet();
		if(ifPlaced){
			if(GUI.Button(readyRect, ReadyBut)){
				sSoundGame.PlaySound();
				audio.Stop();
				pause = true;
			}
			if(readyRect.Contains(mousePos)){
				GUI.DrawTexture(new Rect(readyRect.x-1, readyRect.y-0.5f, readyRect.width+2, readyRect.height+1),ReadyBut);
			}
		}
	}
}

public class GuardianUIData {
	public Transform Chess; 
	public Transform ChessDisplay;
	public Transform Skill;
	public Rect DragRect = new Rect();
	public Rect OriginRect = new Rect();
	public Texture2D Icon; 
	public bool ShowGUI;
	public bool Placed; 
	public Rect TrueOriginRect = new Rect();
	public GuardianUIData(Transform chess){
		Chess = chess; 
		Icon = chess.GetComponent<CharacterProperty>().SmallIcon;
		ChessDisplay = chess.GetComponent<CharacterProperty>().DisplayModel;
		Skill = chess.FindChild("Skills").GetChild(0);
		ShowGUI = true;
		Placed = false;
	}
}