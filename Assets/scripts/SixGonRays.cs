using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class SixGonRays: MonoBehaviour {
	public float stepRange = 1.2f;
	private float castLength = 4.0f*Mathf.Tan(60.0f/180.0f*Mathf.PI);
	public bool selected = false; 
	
    void Start(){
	}
	
	public IList getNeighbors(int steps){
		IList neighbor = new List<Transform>();
		//shoot rays depends on how many steps
		int rayNumber = 6*steps;
		float angle = 360.0f/(float)rayNumber;
		Vector3 rayDir = transform.forward;
		Ray newRay = new Ray(transform.position, rayDir);
		
		for(int i=0; i<rayNumber; i++){
			RaycastHit[] hits;
			hits = Physics.RaycastAll(newRay,castLength*steps);
			foreach(RaycastHit hit in hits){
				if(!neighbor.Contains(hit.transform))
					neighbor.Add(hit.transform);
			}
			rayDir = Quaternion.AngleAxis(angle,Vector3.up)*rayDir;
			newRay = new Ray(transform.position, rayDir);
		}
		return neighbor;
	}
	
	public void drawRays(int steps){
		int rayNumber = 6*steps;
		float angle = 360.0f/(float)rayNumber;
		Vector3 rayDir = transform.forward;
		for(int i=0; i<rayNumber; i++){
			Debug.DrawRay(transform.position,rayDir*castLength*steps,Color.red);
			rayDir = Quaternion.AngleAxis(angle,Vector3.up)*rayDir;
		}
	}
	
	void Update(){
	}
}