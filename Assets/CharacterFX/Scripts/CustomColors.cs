using UnityEngine;
using System.Collections;

public class CustomColors : MonoBehaviour 
{
	public Renderer CustomRenderer;
	private Material[] CustomMaterials;
	
	// Get a reference to the material
	void Awake()
	{
		CustomMaterials = CustomRenderer.materials;	
	}
	
	// Clean up the duplicated material
	void OnDestroy()
	{
		foreach(Material m in CustomMaterials)
		{
			Destroy (m);
		}
	}
	
	// Set the eye color for each material on this renderer
	public void SetEyeColor(float r, float g, float b)
	{
		foreach(Material m in CustomMaterials)
		{
			m.SetColor("_EyeColor",new Color(r,g,b,1.0f));
		}
		
	}
	
	public void SetSkinColor(float r, float g, float b)
	{
		foreach(Material m in CustomMaterials)
		{
			m.SetColor("_SkinColor",new Color(r,g,b,1.0f));
		}
	}
	
	public void SetHairColor(float r, float g, float b)
	{
		foreach(Material m in CustomMaterials)
		{
			m.SetColor("_HairColor",new Color(r,g,b,1.0f));		
		}
	}
}
