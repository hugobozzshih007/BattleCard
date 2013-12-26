using UnityEngine;
using System.Collections;

[AddComponentMenu("SoundFX/Character Anim Sound")]

public class AnimSoundFX : MonoBehaviour {
	
	public AudioClip NormalAtk, SpecialAtk, SkillSound, WalkSound; 
	
	// Use this for initialization
	void Start () {
		
	}
	
	public void PlayNormalAtk(){
		audio.loop = false;
		audio.clip = NormalAtk;
		audio.Play();
	}
	
	public void PlaySpecialAtk(){
		audio.loop = false;
		audio.clip = SpecialAtk;
		audio.Play();
	}
	
	public void PlayWalk(){
		audio.loop = true;
		audio.clip = WalkSound;
		audio.Play();
	}
	
	public void PlaySkill(){
		audio.loop = false;
		audio.clip = SkillSound;
		audio.Play();
	}
	
	/*
	public void PlayVoiceSelect(){
		int len = transform.parent.GetComponent<CharacterSelect>().Voice_select.Length;
		int i = Random.Range(0, len);
		audio.clip = transform.parent.GetComponent<CharacterSelect>().Voice_select[i];
		if(audio.clip != null)
			audio.Play();
	}*/
	
	// Update is called once per frame
	void Update () {
	
	}
}
