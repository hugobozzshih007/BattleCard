using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Identy : MonoBehaviour {
	
	public bool MapUnit;
	public bool summoner;
	public bool River;
	public bool Trees;
	public bool Grass;
	public bool Flag;
	public bool StartPoint;
	public Transform[] neighbor;
	public Material originalMat; 
	public int step = 0;
	public IDictionary neighbors;
	private bool test = true;
	
	public void getStructure(){
		neighbor = new Transform[6];
		neighbors = new Dictionary<string,Transform>(6);
		//shoot rays depends on how many steps
		int rayNumber = 6;
		float angle = 360.0f/(float)rayNumber;
		Vector3 rayDir = transform.forward;
		Ray newRay = new Ray(transform.position, rayDir);
		float castLength = 4.0f*Mathf.Tan(60.0f/180.0f*Mathf.PI);
		
		for(int i=0; i<rayNumber; i++){
			RaycastHit hit;
			if(Physics.Raycast(newRay,out hit,castLength)){
				neighbor[i] = hit.transform;
			}else{
				neighbor[i]= null;
			}
			rayDir = Quaternion.AngleAxis(angle,Vector3.up)*rayDir;
			newRay = new Ray(transform.position, rayDir);
		}
		neighbors.Add("Top",neighbor[0]);
		neighbors.Add("TopRight",neighbor[1]);
		neighbors.Add("BotRight",neighbor[2]);
		neighbors.Add("Bot",neighbor[3]);
		neighbors.Add("BotLeft",neighbor[4]);
		neighbors.Add("TopLeft",neighbor[5]);
	}
	
	// Use this for initialization
	void Start () {
		if(MapUnit){
			getStructure();
		}
	}
	
	// Update is called once per frame
	void Update () {
	}
}
