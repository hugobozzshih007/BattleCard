using UnityEngine;
using System.Collections;

public class CharacterSelect : MonoBehaviour {
	private IList neighbors;
	private SixGonRays unit;
	private Material originalMat;
	public Material rollOver;
	public Material closeBy;
	private float castLength = 20.0f;
	public bool selectedMode = false;
	CharacterProperty thisProperty;
	// Use this for initialization
	void Start () {
		thisProperty = this.GetComponent<CharacterProperty>();
		originalMat = GameObject.Find("unit0").transform.renderer.material;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	/*
	void OnMouseDown(){
		Transform mapUnit = getMapPosition();
		if(mapUnit != null){
			unit = mapUnit.GetComponent<SixGonRays>();
			neighbors = unit.getNeighbors(thisProperty.moveRange);
			foreach(Transform sixGon in neighbors){
				sixGon.renderer.material = closeBy;
			}
			mapUnit.renderer.material = rollOver;
		}
	}*/
	
	// start to select 
	public Transform getMapPosition(){
		Transform mapPosition = null; 
		Vector3 rayDir = -transform.up;
		Ray rayDown = new Ray(transform.position, rayDir);
		RaycastHit hit;
		if(Physics.Raycast(rayDown,out hit,castLength)){
			mapPosition = hit.transform;
		}
		return mapPosition;
	}
	
}
