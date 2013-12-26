using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomColorsController : MonoBehaviour
{
	public List<CustomColors> ColorizableObjects = new List<CustomColors>();
	
	void Start ()
	{
		// If none are set, then we will dynamically find the components
		if (ColorizableObjects.Count == 0)
		{
			ColorizableObjects.AddRange(GetComponentsInChildren<CustomColors>());
		}
	}
	
	public void SetEyeColor(float r, float g, float b)
	{
		foreach(CustomColors c in ColorizableObjects)
		{
			c.SetEyeColor(r,g,b);
		}
	}
	
	public void SetSkinColor(float r, float g, float b)
	{
		foreach(CustomColors c in ColorizableObjects)
		{
			c.SetSkinColor(r,g,b);
		}
	}
	
	public void SetHairColor(float r, float g, float b)
	{
		foreach(CustomColors c in ColorizableObjects)
		{
			c.SetHairColor(r,g,b);
		}
	}	
}

