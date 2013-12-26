using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hologram : MonoBehaviour {
    public float ClipVariance = 5.0f;
    public float RimVariance = 0.5f;
	public float lightflicker = 0.3f;
	
	public Renderer HoloRenderer;
	
    public Light FlickerLight;	
    private float clippower = 0.0f;
    private float rimpower = 0.0f;
    private float intensity = 0.0f;
	private Material[] HoloMaterials = null;
	
    void Start()
    {

    }
	
	void Awake()
	{
		HoloMaterials = HoloRenderer.materials;
		
    	clippower = HoloMaterials[0].GetFloat("_ClipPower");
    	rimpower = HoloMaterials[0].GetFloat("_RimPower");
		if (FlickerLight != null)
			intensity = FlickerLight.intensity;		
	}
	
	public void EnableScanlines(bool enabled)
	{
		clippower = 301.0f;
	}
	
	
	// Update is called every frame, if the
	// MonoBehaviour is enabled.
	void Update () {
		// make hologram flicker
		float newclip =(clippower-(ClipVariance/2)) + (Random.value * ClipVariance);
		float rimrandom = Random.value;
		float rimchange = rimrandom * RimVariance; 
		float newrim = rimpower-(RimVariance/2);
		if (newclip < 0) newclip = 0;
		if (newrim < 0) newrim = 0;
		
		foreach(Material HoloMaterial in HoloMaterials)
		{
			HoloMaterial.SetFloat("_RimPower",newrim);
			HoloMaterial.SetFloat("_ClipPower",newclip);
		}
		
		// make light flicker
		if (FlickerLight != null)
		{
			if (rimchange < 0)
			{	
				FlickerLight.intensity = intensity - (intensity * lightflicker * rimrandom);
			}
			else
			{
				FlickerLight.intensity = intensity + (intensity * lightflicker * rimrandom);
			}
		}
	}
	
	void OnDestroy()
	{
		foreach(Material HoloMaterial in HoloMaterials)
		{
			Destroy(HoloMaterial);
		}
	}
}
