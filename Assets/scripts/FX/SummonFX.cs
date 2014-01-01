using UnityEngine;
using System.Collections;
using MapUtility;
using System.Collections.Generic;

[AddComponentMenu("Sliding FX/Summon")]

public class SummonFX : MonoBehaviour {
	bool startDie = false;
	float timeT = 0.0f; 
	float timeDie = 0.0f;
	float fadeInTime = 2.0f;
	float timeToDie = 1.5f;
	bool summonIn = false;
	bool delayDeath = false;
	float delayTimeToDeath = 1.5f;
	float delaySeg = 0.0f;

	Color red = new Color(1.0f,155.0f/255.0f,155.0f/255.0f,1.0f);
	Color yellow = new Color(1.0f,245.0f/255.0f,90.0f/255.0f,1.0f);
	PointCalculation pCal;
	Dictionary<string, string> oldShaderDict = new Dictionary<string, string>();
	NpcPlayer npc;
	PlaceSummoner pSummoner;
	CharacterProperty cp;
	RoundUI rUI ;
	MainInfoUI infoUI;
	StatusMachine sMachine;
	bool soundInPlayed = false;
	bool soundOutPlayed = false;
	SystemSound sSound; 
	GeneralSelection currentSel; 
	// Use this for initialization
	void Start () {
		pSummoner = GameObject.Find("InitStage").transform.GetComponent<PlaceSummoner>();
		npc = GameObject.Find("NpcPlayer").GetComponent<NpcPlayer>();
		cp = transform.GetComponent<CharacterProperty>();
		rUI = Camera.mainCamera.GetComponent<RoundUI>();
		infoUI = Camera.mainCamera.transform.GetComponent<MainInfoUI>();
		sMachine = GameObject.Find("StatusMachine").GetComponent<StatusMachine>();
		pCal = GameObject.Find("EndStage").GetComponent<PointCalculation>();
		sSound = GameObject.Find("SystemSound").GetComponent<SystemSound>();
		currentSel = Camera.main.GetComponent<GeneralSelection>();
	}
	
	public void ActivateSummonFX(){
		if(RecordOldShaderList(transform)){
			if(transform.GetComponent<CharacterProperty>().Player==1)
				MapHelper.SetObjTransparent(transform,red,0.0f);
			else
				MapHelper.SetObjTransparent(transform,yellow,0.0f);
			timeT = 0.0f;
			summonIn = true;
		}
		
	}

	public void StartDelayDeath(float delayTime){
		delayTimeToDeath = delayTime;
		delayDeath = true; 
		delaySeg = 0.0f;
	}

	public void ActivateDying(){
		if(RecordOldShaderList(transform)){
			Color col = GetSideColor();
			MapHelper.SetObjTransparent(transform,col,1.0f);
			timeDie = 0.0f;
			startDie = true;
		}
	}
	//for new NGUI system
	void GenerateHUDInfo(){
		GameObject uiInfo = NGUITools.AddChild(GameObject.Find("main_info_panel"), infoUI.GetQuickInfo(cp.Player)); 
		uiInfo.gameObject.layer = 8;
		uiInfo.GetComponent<UIFollowTarget>().target = MapHelper.FindAnyChildren(transform, "UIAnchor"); 
		uiInfo.GetComponent<UIFollowTarget>().gameCamera = Camera.main;
		uiInfo.GetComponent<UIFollowTarget>().uiCamera = GameObject.Find("UICamera").camera; 
		cp.QuickInfoRealUI = uiInfo.transform;
	}
	//for new NGUI system
	void KillHUDInfo(){
		if(cp.QuickInfoRealUI){
			Destroy(cp.QuickInfoRealUI.gameObject, 1.0f);
			cp.QuickInfoRealUI = null;
		}
	}
	

	bool RecordOldShaderList(Transform gf){
		oldShaderDict.Clear();
		List<Transform> models = new List<Transform>();
		Transform model = gf.FindChild("Models");
		if(model.childCount>0){
			for(int i=0; i<model.childCount; i++){
				if(model.GetChild(i).GetComponent<SkinnedMeshRenderer>()!=null){
					models.Add(model.GetChild(i));
				}
			}
		}
		foreach(Transform m in models){
			//print(gf.name+" : "+m.name); 
			if(!oldShaderDict.ContainsKey(m.name))
				oldShaderDict.Add(m.name, m.renderer.material.shader.name);
		}
		return true;
	}
	
	Color GetSideColor(){
		Color sideCol = Color.white;
		if(cp.Player == 1){
			sideCol = red;
		}else{
			sideCol = yellow;
		}
		return sideCol;
	}
	
	// Update is called once per frame
	void Update () {
		if(delayDeath){
			delaySeg += Time.deltaTime/delayTimeToDeath;  
			if(delaySeg >= 0.99f){
				CommonFX cFX = Camera.mainCamera.GetComponent<CommonFX>();
				CharacterSelect cSelect = transform.GetComponent<CharacterSelect>();
				Transform map = cSelect.getMapPosition();
				Vector3 FxPos = new Vector3(map.transform.position.x,map.transform.position.y+0.1f,map.transform.position.z);
				Transform fx = Object.Instantiate(cFX.DeadOut,FxPos,Quaternion.identity) as Transform;
				Object.Destroy(GameObject.Find(fx.name).gameObject, 3.0f);
				ActivateDying();
				delaySeg = 0.0f;
				delayDeath = false;
			}
		}

		if(summonIn){
			if(!soundInPlayed){
				sSound.PlaySound(SysSoundFx.SummonIn);
				soundInPlayed = true;
			}
			timeT+=Time.deltaTime/fadeInTime;
			float alpha = Mathf.Lerp(0.0f,1.0f,timeT);
			Color oldCol = GetSideColor();
			oldCol.a = alpha;
			
			List<Transform> models = new List<Transform>();
			Transform model = this.transform.FindChild("Models");
			if(model.childCount>0){
				for(int i=0; i<model.childCount; i++){
					if(model.GetChild(i).GetComponent<SkinnedMeshRenderer>()!=null){
						models.Add(model.GetChild(i));
					}
				}
			}
			
			if(models.Count>0){
				foreach(Transform m in models){
					m.renderer.material.SetColor("_Color",oldCol);
				}
			}
			
			if(alpha>0.99){
				if(MapHelper.SetObjOldShader(this.transform,oldShaderDict,1.0f)){
					summonIn = false;
					npc.InPause = true;
				}
				if(!sMachine.TutorialMode){
					if(pSummoner.InitialB){
						pSummoner.TalkB = true;  
						pSummoner.InitialB = false;
					}
						
					if(pSummoner.InitialA){
						pSummoner.TalkA = true;
						pSummoner.InitialA = false;
					}
				}else{
				}
				GenerateHUDInfo();
				//GenerateHUDText();
				soundInPlayed = false;
				sMachine.InBusy = false;
				npc.npcReviveMode = false;
			}
		}
		if(startDie){
			if(!soundOutPlayed){
				sSound.PlaySound(SysSoundFx.SummonOut);
				soundOutPlayed = true;
			}
			timeDie+=Time.deltaTime/timeToDie;
			float alpha = Mathf.Lerp(1.0f,0.0f,timeDie);
			Color oldCol = GetSideColor();
			oldCol.a = alpha;
			List<Transform> models = new List<Transform>();
			Transform model = transform.FindChild("Models");
			if(model.childCount>0){
				for(int i=0; i<model.childCount; i++){
					if(model.GetChild(i).GetComponent<SkinnedMeshRenderer>()!=null){
						models.Add(model.GetChild(i));
					}
				}
			}
			if(models.Count>0){
				foreach(Transform m in models){
					m.renderer.material.color = oldCol;
				}
			}		
			if(alpha<=0.05){
				if(pCal != null)
					pCal.AddDeadNum(transform);
				cp.Death = true;
				startDie = false;
				rUI.Wait = false;
				if(!sMachine.TutorialMode){
					// NGUI
					infoUI.DeactivateInfoUI(1);
				}
				//NGUI
				infoUI.DeactivateInfoUI(2);

				cp.Ready = false;
				cp.WaitRounds = cp.StandByRounds;
				timeDie = 0.0f;
				transform.position = new Vector3(0.0f,1000.0f,0.0f);
				//restore material
				MapHelper.SetObjOldShader(this.transform,oldShaderDict,1.0f);
				GameObject.Find("StatusMachine").GetComponent<StatusMachine>().InBusy = false;
				KillHUDInfo();

				//Initial Champ
				cp.Ready = false;
				cp.WaitRounds = cp.StandByRounds;
				currentSel.TurnFinished(transform, false);

				if(Network.connections.Length==0){
					Transform npcPlayer = GameObject.Find("NpcPlayer").transform;
					NpcPlayer npc = npcPlayer.GetComponent<NpcPlayer>();
					npc.InPause = true;
				}
				soundOutPlayed = false;
			}
		}
	}
}
