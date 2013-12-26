using UnityEngine;
using System.Collections;

public class RandomSounds : MonoBehaviour {
	
	public AudioClip[] SoundFX;  
	
	
	void Awake(){
		int index = Random.Range(0, SoundFX.Length);
		audio.clip = SoundFX[index];
		if(audio.clip!=null)
			audio.Play();
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
