using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BeamController : MonoBehaviour
{
	public List<Beam> BeamObjects = new List<Beam>();
	public AudioSource EffectSource;
	public AudioClip EffectSound;
	public bool PlaySound;
	public float EffectLength = 3.0f;
	
	void Start ()
	{
		// If none are set, then we will dynamically find the components
		if (BeamObjects.Count == 0)
		{
			BeamObjects.AddRange(GetComponentsInChildren<Beam>());
		}
	}
	
	/// <summary>
	/// Starts the effect.
	/// </summary>
	/// <returns>
	/// The length of the effect. If a sound is assigned, then the length
	/// of the sound is returned. Otherwise, the EffectLength var is used.
	/// </returns>
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
	
	// Note: Do not destroy if you are going to beam this guy in later.
	public void BeamOut(bool Destroy)
	{
		float length = StartEffect();
		foreach(Beam b in BeamObjects)
		{
			b.BeamOut(length,Destroy);
		}
	}
	
	public void BeamIn()
	{
		float length = StartEffect ();
		foreach(Beam b in BeamObjects)
		{
			b.BeamIn(length);
		}
	}
}

