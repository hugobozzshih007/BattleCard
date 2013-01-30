using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility; 

public class CharacterPassive : MonoBehaviour {
	
	public PassiveType[] PassiveAbility;
	public Dictionary<PassiveType, bool> PassiveDict;
	public bool StartDie = false;
	float timeToDie = 1.5f;
	CharacterProperty cp;
	const float seg = 0.003f;
	int flyTimes = 400;
	int t = 0;
	int f = 0;
	float r = 0.0f;
	bool flying = false;
	RoundUI rUI;
	MainInfoUI infoUI;
	MainUI mUI;
	// Use this for initialization
	void Start () {
		cp = transform.GetComponent<CharacterProperty>();
		PassiveDict = new Dictionary<PassiveType, bool>();
		foreach(PassiveType passive in Enum.GetValues(typeof(PassiveType))){
			PassiveDict.Add(passive, false);
		}
		if((PassiveAbility!=null) && (PassiveAbility.Length>0)){
			foreach(PassiveType p in PassiveAbility){
				PassiveDict[p] = true;
			}
		}
		rUI = Camera.mainCamera.GetComponent<RoundUI>();
		infoUI = Camera.mainCamera.transform.GetComponent<MainInfoUI>();
		mUI = Camera.mainCamera.transform.GetComponent<MainUI>();
	}
	
	void Update(){
		if(!cp.death && PassiveDict[PassiveType.Flying]){
			if(!flying){
				if(f<800){
					transform.Translate(0.0f,seg,0.0f);
					f+=1;
					if(f==flyTimes){
						f=0;
						flying = true;
					}
				}
			}else{
				if(t<flyTimes){
					transform.Translate(0.0f,seg,0.0f);
					t+=1;
				}else if(t>=flyTimes){
					transform.Translate(0.0f,-seg,0.0f);
					t+=1;
					if(t>=flyTimes*2)
						t=0;
				}
			}
		}
		if(StartDie){
			r+=Time.deltaTime/timeToDie;
			float alpha = Mathf.Lerp(1.0f,0.0f,r);
			Color oldCol = transform.renderer.material.color;
			oldCol.a = alpha;
			transform.renderer.material.color = oldCol;
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
				cp.death = true;
				StartDie = false;
				rUI.Wait = false;
				infoUI.MainFadeIn = false;
				infoUI.TargetFadeIn = false;
				mUI.MainGuiFade = false;
				mUI.SubGuiFade = false;
				cp.Ready = false;
				cp.WaitRounds = cp.StandByRounds;
				r = 0.0f;
			}
		}
	}
}
