using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StoneController : MonoBehaviour 
{
	public List<Stone> StoneableObjects = new List<Stone>();
	public AudioSource EffectSource;
	public AudioClip EffectSound;
	public bool PlaySound;
	public float EffectLength = 3.0f;
    public bool  IsStone = false;
	
	// Use this for initialization
	void Start () 
	{
		if (StoneableObjects.Count == 0)
		{
			StoneableObjects.AddRange(GetComponentsInChildren<Stone>());
		}
	}
	
	private float StartEffect()
	{
		if (PlaySound)
		{
			if (EffectSource != null && EffectSound != null)
			{
				EffectSource.PlayOneShot(EffectSound);
				return EffectSound.length;
			}
		}
		return EffectLength;
	}	
	
	public void TurnToStone()
	{
	
		if (IsStone)
			return;
		IsStone = true;
		
		float length = StartEffect();
		foreach(Stone o in StoneableObjects)
		{
			o.TurnToStone(length);
		}
	}

	public void StoneToFlesh()
	{
		if (!IsStone)
			return;
		IsStone = false;

		float length = StartEffect();
		foreach(Stone o in StoneableObjects)
		{
			o.StoneToFlesh(length);
		}
	}
}