using UnityEngine;
using System.Collections;

public class FogSetting : MonoBehaviour {
	public float StartDist;
	public float EndDist;
	
	// Use this for initialization
	void Start () {
		RenderSettings.fog = true;
		RenderSettings.fogMode = FogMode.Linear;
		RenderSettings.fogColor = Color.black;
		RenderSettings.fogEndDistance = EndDist;
		RenderSettings.fogStartDistance = StartDist;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
