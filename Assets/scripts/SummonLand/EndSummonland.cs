using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndSummonland : MonoBehaviour {
	public Transform Plane_Color;
	public bool AWin, BWin; 
	public float EndCamHeight; 
	//public int VictoryRate = 70;
	bool AWon, BWon,DrawGame;   
	GeneralSelection currentSel; 
	BuffInfoUI buffUI;
	RoundCounter currentRC;
	IList leftMaps = new List<Transform>();
	public bool inGame = true;
	bool inDelay = false;
	bool moveCam = false;  
	Vector3 EndCam= new Vector3(0.0f, 56.0f, -39.0f);
	Vector3 currentCamPos = new Vector3();
	const float CamMovingTime = 1.0f;
	const float EndCamFOV = 60.0f;
	float currentFOV = 0.0f;
	float s = 0.0f;
	int delayCounter = 0; 
	int totalDelay = 5;
	int t =0;
	int winSide;
	string stage = "";
	MainInfoUI mInfoUI;
	StatusMachine sMachine;
	WinningUI wUI;
	bool excuted = false;
	// Use this for initialization
	void Start () {
		sMachine = GameObject.Find("StatusMachine").transform.GetComponent<StatusMachine>();
		AWon = BWon = DrawGame = AWin = BWin =false;
		currentRC = Camera.mainCamera.GetComponent<RoundCounter>();
		currentSel = Camera.mainCamera.GetComponent<GeneralSelection>();
		buffUI = Camera.mainCamera.GetComponent<BuffInfoUI>();
		mInfoUI = Camera.mainCamera.GetComponent<MainInfoUI>();
		wUI = transform.GetComponent<WinningUI>();
		excuted = false;
		EndCam = new Vector3(0.0f, EndCamHeight, -39.0f);
	}
	
	// Update is called once per frame
	void Update () {
		if(sMachine.InitGame){
			if(currentRC.PlayerATerritory.Count==0 || BWin){
				BWon = true; AWon = false;
				winSide = 2;
				leftMaps = currentRC.GetWhiteTerritory();
				stage = "yellowwins";
				currentCamPos = Camera.mainCamera.transform.position;
				currentFOV = Camera.mainCamera.fieldOfView;
				moveCam = true;
				sMachine.GameEnd = true;
				sMachine.InGame = false;
				sMachine.InBusy = true;
				sMachine.InitGame = false;
				inGame = false;
			}
			if(currentRC.PlayerBTerritory.Count==0 || AWin){
				AWon = true; BWon = false;
				winSide = 1;
				leftMaps = currentRC.GetWhiteTerritory();
				stage = "redwins";
				currentCamPos = Camera.mainCamera.transform.position;
				currentFOV = Camera.mainCamera.fieldOfView;
				sMachine.GameEnd = true;
				sMachine.InGame = false;
				sMachine.InBusy = true;
				sMachine.InitGame = false;
				moveCam = true;
				inGame = false;
			}
			if(mInfoUI.LeftRounds == 0){
				//if(!currentSel.npcMode){
					int diff = currentRC.PlayerATerritory.Count - currentRC.PlayerBTerritory.Count;
					if(diff > 0){
						AWon = true; BWon = false;
						winSide = 1;
						IList allMap = new List<Transform>();
						foreach(Transform m in currentRC.AllTerritory){
							allMap.Add(m);
						}
						foreach(Transform m in currentRC.PlayerATerritory){
							if(allMap.Contains(m))
								allMap.Remove(m);
						}
						leftMaps = allMap;
						stage = "redwins";
						currentCamPos = Camera.mainCamera.transform.position;
						currentFOV = Camera.mainCamera.fieldOfView;
						sMachine.GameEnd = true;
						sMachine.InGame = false;
						sMachine.InBusy = true;
						sMachine.InitGame = false;
						moveCam = true;
						inGame = false;
					}else if(diff < 0){
						AWon = false; BWon = true;
						winSide = 2;
						IList allMap = new List<Transform>();
						foreach(Transform m in currentRC.AllTerritory){
							allMap.Add(m);
						}
						foreach(Transform m in currentRC.PlayerBTerritory){
							if(allMap.Contains(m))
								allMap.Remove(m);
						}
						leftMaps = allMap;
						stage = "yellowwins";
						currentCamPos = Camera.mainCamera.transform.position;
						currentFOV = Camera.mainCamera.fieldOfView;
						sMachine.GameEnd = true;
						sMachine.InGame = false;
						sMachine.InBusy = true;
						sMachine.InitGame = false;
						moveCam = true;
						inGame = false;
					}else if(diff == 0){
						AWon = false; BWon = false;
						DrawGame = true; 
						inGame = false;
						sMachine.GameEnd = true;
						sMachine.InGame = false;
						sMachine.InBusy = true;
						sMachine.InitGame = false;
					}
			}
		}else{
			if(!DrawGame){
				//Change Color plane's alpha value
				
				if(!inDelay){
					if(t<leftMaps.Count){
						Transform currentEmptyMap = leftMaps[t] as Transform;
						//currentEmptyMap.GetComponent<PlaneShadows>().ChangeShadowMaterial(winSide);
						//Transform realMap = currentEmptyMap.GetComponent<Identy>().ShowMap;
						if(t==(leftMaps.Count-1)){
							if(AWon){
								if(Plane_Color){
									Plane_Color.renderer.enabled = false;
									GameObject.Find("ProceduralMats").GetComponent<ProceduralControl>().FinalWinSet();
								}
							}else{
								
							}		
						}
						t+=1;
						inDelay = true;
					}else{
						if(!excuted){
							if(AWon)
								wUI.SetWinningUI(1);
							else if(BWon)
								wUI.SetWinningUI(2);
							else if(DrawGame)
								wUI.SetWinningUI(3);
							excuted = true;
						}
					}
				}else{
					delayCounter+=1;
					if(delayCounter>=totalDelay){
						inDelay = false;
						delayCounter = 0;
					}
				}
			}else{
			}
		}
		
		if(moveCam){
			s += Time.deltaTime / CamMovingTime;
			Vector3 camPos = Vector3.Lerp(currentCamPos, EndCam, s);
			float fov = Mathf.Lerp(currentFOV, EndCamFOV, s);
			Camera.main.transform.position = camPos;
			Camera.main.fieldOfView = fov;
			if(Camera.mainCamera.fieldOfView >= EndCamFOV-0.01){
				moveCam = false;
				s = 0.0f;
			}
		}
	}
}
