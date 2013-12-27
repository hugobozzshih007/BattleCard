using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BGMusic : MonoBehaviour {
	public AudioClip Opening, Initial_Loop, Init_To_Start, Start_Loop, Start_To_Climax, Start_To_Init, Climax_Loop, Climax_To_Start;
	GeneralSelection currentSel; 
	RoundCounter currentRC;  
	StatusMachine sMachine;
	BuffInfoUI buffInfo;
	int start_loop_times = 0;
	int climax_loop_times = 0;
	IList musicList = new List<AudioClip>();
	bool firstPlay = true; 
	bool changeState = false;
	int initRate, startRate, climaxRate, SuperRate; 
	float audioVolume = 0.0f;
	public const float limitVol = 0.3f;
	enum StageState{
		InitMoment,
		StartMoment, 
		ClimaxMoment,
		SuperMoment,
	}
	
	// Use this for initialization
	void Start () {
		currentSel = transform.parent.GetComponent<GeneralSelection>();
		currentRC = transform.parent.GetComponent<RoundCounter>();
		buffInfo = transform.parent.GetComponent<BuffInfoUI>();
		sMachine = GameObject.Find("StatusMachine").transform.GetComponent<StatusMachine>();
		initRate = 0;
		startRate = 3;
		climaxRate = 7;
		SuperRate = 10;
		musicList.Add(Opening);
		musicList.Add(Initial_Loop);
	}
	
	void MusicFadeOut() {
	    if(audioVolume > 0.02f)
	    {
	        audioVolume -= 0.06f * Time.deltaTime;
	        audio.volume = audioVolume;
	    }
	}
	
	void MusicFadeIn() {
	    if (audioVolume < limitVol) {
	        audioVolume += 0.07f * Time.deltaTime;
	        audio.volume = audioVolume;
	    }
	}
	
	StageState GetCurrentState(){
		StageState ss = StageState.InitMoment;
		int people = 0;  
		foreach(Transform gf in currentRC.AllChesses){
			if(!gf.GetComponent<CharacterProperty>().death){
				people+=1;
			}
		}
		
		if(people <= initRate){
			ss = StageState.InitMoment;
		}else if(people > initRate && people <= climaxRate){
			ss = StageState.StartMoment;
		}else if(people > climaxRate && people <= SuperRate){
			ss = StageState.ClimaxMoment;
		}else{
			ss = StageState.SuperMoment;
		}
		
		return ss;
	}
	
	void AddMusicList(StageState ss){
		AudioClip item = null;
		if(musicList.Count > 0)
			item = (AudioClip)musicList[musicList.Count-1];
		
		switch(ss){
			case StageState.InitMoment:
				musicList.Add(Initial_Loop);
				break;
			case StageState.StartMoment:
				if(start_loop_times <=4){
					if(item == Initial_Loop || item == Start_To_Init){
						musicList.Add(Init_To_Start);
					}else if(item == Climax_Loop){
						musicList.Add(Climax_To_Start);	
					}
					musicList.Add(Start_Loop);
					start_loop_times += 1;
				}else{
					musicList.Add(Start_To_Init);
					start_loop_times = 0;
				}
				
				break;
			case StageState.ClimaxMoment:
				if(climax_loop_times <=2){
					if(item == Start_Loop || item == Climax_To_Start){
						musicList.Add(Start_To_Climax);
					}
					musicList.Add(Climax_Loop);
					climax_loop_times += 1;	
				}else{
					musicList.Add(Climax_To_Start);
					climax_loop_times = 0;
				}
				break;
			case StageState.SuperMoment:
				musicList.Add(Climax_To_Start);
				musicList.Add(Start_Loop);
				musicList.Add(Start_To_Climax);
				musicList.Add(Climax_Loop);
				break;
				
		}
	}
	
	void PlayMusic(){
		audio.clip =(AudioClip)musicList[0];
		audio.loop = false;
		if(audio.clip != null)
			audio.Play();
	}
	
	// Update is called once per frame
	void Update () {
		if(sMachine.InitGame){
			MusicFadeIn();
			if(firstPlay){
				PlayMusic();
				firstPlay = false;
			}
			if(!audio.isPlaying){
				if(musicList.Count>1)
					musicList.RemoveAt(0);
				PlayMusic();
				if(musicList.Count<=1){
					AddMusicList(GetCurrentState());
				}
			}
		}
		if(sMachine.GameEnd){
			MusicFadeOut();
		}
	}
}


