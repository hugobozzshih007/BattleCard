using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class selection : MonoBehaviour {
	private Transform sel;
	private Transform chess = null;
	IList neighbors;
	private Transform selOld;
	private SixGonRays unit;
	private Material originalMat;
	private bool selectedMode = false;
	public Material rollOver;
	public Material closeBy;
	public float castLength = 80.0f;
	public int steps = 2;
	void Start(){
		sel = GameObject.Find("unit0").transform;
		selOld = sel;
		originalMat = sel.renderer.material;
		unit = sel.GetComponent<SixGonRays>();
		unit.selected = true;
		neighbors = unit.getNeighbors(steps);
		
		//print(neighbors.Count);
		foreach(Transform sixGon in neighbors){
			sixGon.renderer.material = originalMat;
		}
	}
	
	//select map or character
	
	
    void Update() {
		if(Input.GetMouseButtonDown(0)){
			Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        	Debug.DrawRay(ray.origin, ray.direction * castLength, Color.yellow);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, castLength)){
				if(!hit.transform.GetComponent<Identy>().mapUnit){
					CharacterSelect character = hit.transform.GetComponent<CharacterSelect>();
					Transform currentPos = character.getMapPosition();
					if(currentPos!=null){
						neighbors = currentPos.GetComponent<SixGonRays>().getNeighbors(hit.transform.GetComponent<CharacterProperty>().moveRange);
						foreach(Transform sixGon in neighbors){
							sixGon.renderer.material = closeBy;
						}
						currentPos.renderer.material = rollOver;
						selectedMode = true;
						chess = hit.transform;
					}
				}
			}
		}
		if(selectedMode){
			Ray ray = camera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, castLength)){
				if(hit.transform.GetComponent<Identy>().mapUnit){
					sel = hit.transform;
					if(neighbors.Contains(sel)){
						sel.renderer.material = rollOver;
						if(selOld!=sel){
							selOld.renderer.material = closeBy;
						}
						selOld = sel;
						if(Input.GetMouseButtonDown(0)){
							chess.GetComponent<CharacterSelect>().getMapPosition().renderer.material = originalMat;
							chess.position = sel.position;
							chess.Translate(new Vector3(0.0f,1.5f,0.0f));
							selectedMode = false;
							foreach(Transform sixGons in neighbors){
								sixGons.renderer.material = originalMat;
							}
						}
					}
				}
			}
		}
		
		
		
		
	
        
		/*
		if(Physics.Raycast(ray, out hit, castLength)){
			sel=hit.transform;
			sel.renderer.material = rollOver;
			unit = sel.GetComponent<SixGonRays>();
			neighbors = unit.getNeighbors(steps);
			//unit.drawRays(steps);
			foreach(Transform sixGon in neighbors){
				sixGon.renderer.material = closeBy;
			}
			
		}else{
			sel.renderer.material = originalMat;
			unit = sel.GetComponent<SixGonRays>();
			neighbors = unit.getNeighbors(steps);
			foreach(Transform sixGon in neighbors){
				sixGon.renderer.material = originalMat;
			}
		}
		if(selOld != sel){
			selOld.renderer.material = originalMat;
			unit = selOld.GetComponent<SixGonRays>();
			neighbors = unit.getNeighbors(steps);
			foreach(Transform sixGon in neighbors){
				sixGon.renderer.material = originalMat;
			}
		}
		selOld = sel;
		*/
    }
}
