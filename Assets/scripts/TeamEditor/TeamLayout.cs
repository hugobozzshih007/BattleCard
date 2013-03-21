using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TeamLayout : MonoBehaviour {
	public Texture2D BackGround, ReadyBut, RemoveBut;
	public LayerMask MaskPillar;
	public Font StringStyle; 
	float castLength = 80.0f;
	Vector2 mousePos; 
	IList rectList = new List<Rect>();
	IList textureList = new List<Texture2D>();
	bool changingGF = true;
	GuardianUIData[] guardianArray; 
	GuardianUIData currentGF, oldGF;
	Transform currentPillar, oldPillar, returnGF;
	Rect bigIconRect = new Rect(33.0f,99.0f,198.0f,198.0f);
	const float gfWidth = 50.0f;
	const float gfHeight = 130.0f;
	const float interWidth = 58.0f;
	const float startX = 280.0f; 
	const float startY = 540.0f;  
	Rect currentRect = new Rect(); 
	Rect bgRect = new Rect(.0f,.0f, Screen.width, Screen.height);
	bool dragging = false;
	Transform playerData, hittedPillar;
	int dataLength;
	Transform pillars;
	bool placing = true;
	Rect noWhere = new Rect(-50.0f, -130.0f, 50.0f, 130.0f);
	Rect nameRect = new Rect(33.0f, 312.0f, 198.0f, 22.0f);
	Rect infoStartRect = new Rect(50.0f, 420.0f, 168.0f, 20.0f);
	Rect[] infoRects = new Rect[9];
	Rect[] staticRects = new Rect[9];
	Rect[] passiveRects = new Rect[5];
	Rect[] skillRects;
	Rect equipRect = new Rect(1080.0f, 510.0f, 160.0f, 20.0f);
	Rect costRect = new Rect(1058.0f, 360.0f, 168.0f, 20.0f);
	Rect costNumRect = new Rect(1120.0f, 360.0f, 20.0f, 20.0f);
	Rect readyRect = new Rect(888.0f, 640.0f, 122.0f, 30.0f);
	Rect removeRect, keepRect;
	InfoUI iconVault; 
	GUIStyle gStyle = new GUIStyle();
	GUIStyle bStyle = new GUIStyle();
	GUIStyle cStyle = new GUIStyle();
	GuardianStorage guardians;
	bool ifPlaced = false;
	Transform[] skillArray;
	// Use this for initialization
	void Start () {
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
			skillRects = new Rect[guardians.SkillVault.Count];
			for(int i=0; i<dataLength; i++){
				Transform gf = guardians.Guardians[i] as Transform;
				guardianArray[i] = new GuardianUIData(gf);
				Rect gfRect = new Rect(startX+interWidth*i, startY, gfWidth, gfHeight);
				guardianArray[i].DragRect = gfRect;  
				guardianArray[i].OriginRect = gfRect;
				guardianArray[i].TrueOriginRect = gfRect;
			}
			for(int i=0; i<guardians.SkillVault.Count; i++){
				skillRects[i] = new Rect(1080.0f, 140.0f+26.0f*i, 168.0f, 20.0f);
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
	}
	
	void ChangeLayer(Transform chess, int layer){
		if(chess!=null){
			chess.gameObject.layer = layer;
			if(chess.childCount>0){
				for(int i=0; i<chess.childCount; i++){
					chess.GetChild(i).gameObject.layer = layer;
				}
			}
		}
	}
	void GetPillar(){
		Ray ray = Camera.mainCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit, castLength)){
			currentPillar = hit.transform;
			ChangeLayer(currentPillar, 11);
			if(oldPillar!=currentPillar){
				ChangeLayer(oldPillar, 12);
			}
		}else{
			ChangeLayer(currentPillar, 12);
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
		GUI.Label(infoRects[6],"Summon Cost:", bStyle);
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
			GUI.Label(staticRects[6], property.summonCost.ToString(), bStyle);
			GUI.Label(staticRects[7], property.StandByRounds.ToString(), bStyle);
			
			if(passive.PassiveAbility.Length>0){
				for(int i=0; i<passive.PassiveAbility.Length; i++){
					GUI.DrawTexture(passiveRects[i], iconVault.GetPassiveTexture(passive.PassiveAbility[i]));
				}
			}
			
			GUI.Label(equipRect, gfData.Skill.GetComponent<SkillProperty>().SkillName, bStyle);
		}
		if(playerData!=null){
			for(int i=0; i<guardians.SkillVault.Count; i++){
				GUI.Label(skillRects[i], skillArray[i].GetComponent<SkillProperty>().SkillName, bStyle);
				//roll over info
				if(skillRects[i].Contains(mousePos)){
					GUI.Label(costRect, "Cost:", bStyle);
					GUI.Label(costNumRect, skillArray[i].GetComponent<SkillProperty>().SkillCost.ToString(), bStyle);
					GUI.TextArea(new Rect(costRect.x, costRect.y+28.0f,180.0f, 80.0f),skillArray[i].GetComponent<SkillProperty>().info,500,cStyle); 
				}
			}
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
	}
	
	void PillarSelection(){
		Ray ray = Camera.mainCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit, castLength)){
			if(Input.GetMouseButtonDown(1)){
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
			}
		}
	}
	
	void PlaceTheChess(GuardianUIData guardian, Transform target){
		
		if(placing && !guardian.Placed && target!=null){
			PillarData pData = target.GetComponent<PillarData>();
			if(!pData.WithGuardian){
				Transform newChess = Instantiate(guardian.ChessDisplay, target.transform.position, Quaternion.identity) as Transform;
				//newChess.Translate(new Vector3(0.0f,1.5f,0.0f));
				pData.PlaceGuardian(guardian, newChess);
				pData.Display_Model = newChess;
				guardian.Placed = true;
			}
		}
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
		if(ifPlaced){
			if(GUI.Button(readyRect, ReadyBut)){
				Transform leadingPillar = GameObject.Find("leadingPillar").transform;
				guardians.SelectedSummoner = leadingPillar.GetComponent<PillarData>().GuardianForce;
				Transform pillars = GameObject.Find("Pillars").transform;
				IList soilders = new List<Transform>();
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
				Application.LoadLevel("Stage_Mole_Archer");
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
		Icon = chess.GetComponent<CharacterProperty>().GuardianIcon;
		ChessDisplay = chess.GetComponent<CharacterProperty>().DisplayModel;
		Skill = chess.FindChild("Skills").GetChild(0);
		ShowGUI = true;
		Placed = false;
	}
}