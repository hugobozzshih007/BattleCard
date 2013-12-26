using UnityEngine;
using System.Collections;

public class SystemSound : MonoBehaviour {
	int calledTime = 0;
	Transform oldGF;  
	public AudioClip 
		GameOpening, Buff, CommandClick, CommandOver, RoundPlayer, RoundComputer, RoundPlayer1, RoundPlayer2,CommandCancel, DefenseWait, Defense, SummonIn, SummonOut, OpenPrize, EatPrize;
	// Use this for initialization
	void Start () {
		oldGF = null;
	}
	
	AudioClip GetAudioClip(SysSoundFx sFx){
		AudioClip sClip = null;
		switch(sFx){
			case SysSoundFx.Buff:
				sClip = Buff;
				break;
			case SysSoundFx.CommandClick:
				sClip = CommandClick;
				break;
			case SysSoundFx.CommandOver:
				sClip = CommandOver;
				break;
			case SysSoundFx.CommandCancel:
				sClip = CommandCancel;
				break;
			case SysSoundFx.Defense:
				sClip = Defense;
				break;
			case SysSoundFx.DefenseWait:
				sClip = DefenseWait;
				break;
			case SysSoundFx.EatPrize:
				sClip = EatPrize;
				break;
			case SysSoundFx.OpenPrize:
				sClip = OpenPrize;
				break;
			case SysSoundFx.RoundPlayer:
				sClip = RoundPlayer;
				break;
			case SysSoundFx.RoundPlayer1:
				sClip = RoundPlayer1;
				break;
			case SysSoundFx.RoundPlayer2:
				sClip = RoundPlayer2;
				break;
			case SysSoundFx.RoundComputer:
				sClip = RoundComputer;
				break;
			case SysSoundFx.SummonIn:
				sClip = SummonIn;
				break;
			case SysSoundFx.SummonOut:
				sClip = SummonOut;
				break;
			case SysSoundFx.GameOpening:
				sClip = GameOpening;
				break;
			default:
				sClip = null;
				break;
		}
		return sClip;
	}
	
	public void PlaySound(SysSoundFx sFx){
		audio.clip = GetAudioClip(sFx);
		audio.loop = false;
		if(audio.clip != null)
			audio.Play();
	}
	
	public void PlaySound(){
		audio.loop = false;
		if(audio.clip != null)
			audio.Play();
	}
	
	public void PlayVoiceSelect(Transform gf){
		int len = gf.GetComponent<CharacterSelect>().Voice_select.Length;
		int i = Random.Range(0, len);
		audio.clip = gf.GetComponent<CharacterSelect>().Voice_select[i];
		if(audio.clip != null)
			audio.Play();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

public enum SysSoundFx{
	Buff,
	CommandClick,
	CommandOver,
	CommandCancel,
	EatPrize,
	DefenseWait, 
	Defense,
	OpenPrize,
	RoundPlayer,
	RoundComputer,
	RoundPlayer1,
	RoundPlayer2,
	SummonIn, SummonOut, GameOpening,
}