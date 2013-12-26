using UnityEngine;
using System.Collections;

public class DisplayCircular : MonoBehaviour {
	public RenderTexture Circular;
	public Camera FxCam; 
	public Texture2D Nothing;
	Camera fxCam; 
	RenderTexture fxRT;
	Vector3 noWhere = new Vector3(0,150,0);
	// Use this for initialization
	void Start () {
		fxCam = new Camera();
		fxRT = new RenderTexture(256, 256, 24);
		fxCam = Instantiate(FxCam,noWhere,Quaternion.identity) as Camera;
		fxCam.camera.targetTexture = fxRT;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI(){
		GUI.backgroundColor = Color.clear;
		GUI.DrawTexture(new Rect(0,0,120,120),fxRT);
		GUI.DrawTexture(new Rect(200,0,120,120),Nothing);
	}
}
