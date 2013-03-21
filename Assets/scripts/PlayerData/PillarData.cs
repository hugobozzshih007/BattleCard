using UnityEngine;
using System.Collections;

public class PillarData : MonoBehaviour {
	GuardianUIData guardian;
	TeamLayout editor; 
	Texture2D removeBut;
	public Transform GuardianForce;
	public Transform Display_Model;
	public bool WithGuardian; 
	public bool LeadingPillar;
	Vector3 screenPos;
	Vector2 butRect = new Vector2(66.5f, 21.0f);
	Rect removeRect;
	// Use this for initialization
	void Start () {
		guardian = null; 
		WithGuardian = false;
		screenPos = Camera.main.WorldToScreenPoint(transform.position);
		screenPos.y = Screen.height - screenPos.y;
		removeRect = new Rect(screenPos.x-butRect.x/1.75f, screenPos.y-butRect.y*5.0f, butRect.x, butRect.y);
		editor = GameObject.Find("GUILayout").transform.GetComponent<TeamLayout>();
		removeBut = editor.RemoveBut;
	}
	
	public void PlaceGuardian(GuardianUIData gf, Transform display){
		guardian = gf; 
		WithGuardian = true;
		GuardianForce = gf.Chess;
		display = Display_Model;
	}
	
	public void RemoveGuardian(){
		Destroy(Display_Model.gameObject);
		guardian = null;
		WithGuardian = false;
		GuardianForce = null;
		
		Display_Model = null;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnGUI(){
		GUI.depth = 1;
		GUI.backgroundColor = Color.clear;
		if(WithGuardian){
			if(GUI.Button(removeRect, removeBut)){
				editor.RemovePillarGF(transform);
				WithGuardian = false;
			}
		}
	}
}
