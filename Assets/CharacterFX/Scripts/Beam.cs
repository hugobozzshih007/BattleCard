using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Beam : MonoBehaviour {
	
	public AudioSource EffectSource;
	public AudioClip EffectSound;
	public Renderer SourceRenderer;
	private Material[] EffectMaterials;	
	public float EffectLength = 0.0f;
	
	
	// Start is called just before any of the
	// Update methods is called the first time.
	void Start () {
		EffectMaterials = SourceRenderer.materials;
	}
	
	private void SetMaterialParms(float amount)
	{
		foreach(Material m in EffectMaterials)
		{
			m.SetFloat("_DisintegrateAmount",amount);
		}	
	}
	
	private IEnumerator doBeamOut(bool Destroy)
	{
				
		// if we supply our own sound, use it.
		if (EffectSource != null && EffectSound != null)
		{
			EffectLength = EffectSound.length;
			EffectSource.PlayOneShot(EffectSound);
		}		

		float LengthLeft = EffectLength;
		
		while(LengthLeft > 0.0f)
    	{
			float pos = 1.0f - (LengthLeft / EffectLength);
			SetMaterialParms(pos);
        	yield return null;
			LengthLeft -= Time.deltaTime;
    	}
		SetMaterialParms(1.01f);
		if (Destroy)
		{
			GameObject.Destroy(this.gameObject);
		}
	}
	
	private IEnumerator doBeamIn()
	{
		// if we supply our own sound, use it.
		if (EffectSource != null && EffectSound != null)
		{
			EffectLength = EffectSound.length;
			EffectSource.PlayOneShot(EffectSound);
		}		

		float LengthLeft = EffectLength;
				
		while(LengthLeft > 0.0f)
    	{
			float pos =  (LengthLeft / EffectLength);
			SetMaterialParms(pos);
        	yield return null;
			LengthLeft -= Time.deltaTime;
    	}
		
		SetMaterialParms(0.0f);
	}	
	
	public void BeamOut(bool doDestroy)
	{
		StartCoroutine(doBeamOut(doDestroy));
	}
	
	public void BeamIn()
	{
		StartCoroutine(doBeamIn());
	}	

	public void BeamOut(float Length, bool doDestroy)
	{
		EffectLength = Length;
		BeamOut (doDestroy);
	}
	
	public void BeamIn(float Length)
	{
		EffectLength = Length;
		BeamIn();
	}	
	
	public void OnDestroy()
	{
		foreach(Material m in EffectMaterials)
		{
			Destroy (m);
		}
	}
}

public class example : MonoBehaviour {
    void Awake() {
        renderer.material.color = Color.red;
    }
}