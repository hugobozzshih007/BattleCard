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
	public Material originalMat;
	private bool selectedMode = false;
	public Material rollOver;
	public Material closeBy;
	public Material rollOverFlag;
	public Material closeByFlag;
	public Material FlagMat;
	public float castLength = 80.0f;
	public int steps = 2;
	private bool viewScreenPanning = false;
	private float viewScreenOldMouseX;
	private float viewScreenOldMouseY;
	private float viewScreenZoomHeight = 50.0f;
	private float viewScreenPanSpeed = 200.0f;
	
	void Start(){
		sel = GameObject.Find("unit").transform;
		selOld = sel;
	}
	
	//select map or character
	
	
    void Update() {
		if(Input.GetMouseButtonDown(1)) {
  			print ("Panning...");
    		viewScreenPanning = true;
    		viewScreenOldMouseX = Input.mousePosition.x;
    		viewScreenOldMouseY = Input.mousePosition.y;
		}
		if(viewScreenPanning==true) {
		    if(Input.GetMouseButtonUp(1)) {
		        viewScreenPanning = false;
		    }
		    Vector3 viewScreenPanVector = transform.TransformPoint(new Vector3((viewScreenOldMouseX - Input.mousePosition.x) / viewScreenPanSpeed, 0.0f, (viewScreenOldMouseY - Input.mousePosition.y) / viewScreenPanSpeed));
		    // since we use a quick and dirty transform, reset the camera height to what it was
		    viewScreenPanVector.y = viewScreenZoomHeight; 
		    transform.position = viewScreenPanVector;
		}
		
		
		if(Input.GetMouseButtonDown(0)){
			Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        	Debug.DrawRay(ray.origin, ray.direction * castLength, Color.yellow);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, castLength)){
				if(!hit.transform.GetComponent<Identy>().MapUnit){
					CharacterSelect character = hit.transform.GetComponent<CharacterSelect>();
					Transform currentPos = character.getMapPosition();
					if(currentPos!=null){
						character.findMoveRange(currentPos,0,hit.transform.GetComponent<CharacterProperty>().moveRange);
						neighbors = character.MoveRangeList;
						foreach(Transform sixGon in neighbors){
							if(sixGon.GetComponent<Identy>().Flag){
								sixGon.renderer.material = closeByFlag;
							}else{
								sixGon.renderer.material = closeBy;
							}
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
				if(hit.transform.GetComponent<Identy>().MapUnit){
					sel = hit.transform;
					if(neighbors.Contains(sel)){
						if(sel.GetComponent<Identy>().Flag)
							sel.renderer.material = rollOverFlag;
						else
							sel.renderer.material = rollOver;
						if(selOld!=sel){
							if(selOld.GetComponent<Identy>().Flag)
								selOld.renderer.material = closeByFlag;
							else
								selOld.renderer.material = closeBy;
						}
						selOld = sel;
						if(Input.GetMouseButtonDown(0)){
							chess.GetComponent<CharacterSelect>().getMapPosition().renderer.material = originalMat;
							chess.position = sel.position;
							chess.Translate(new Vector3(0.0f,1.5f,0.0f));
							selectedMode = false;
							foreach(Transform sixGons in neighbors){
								if(sixGons.GetComponent<Identy>().Flag)
									sixGons.renderer.material = FlagMat;
								else
									sixGons.renderer.material = originalMat;
							}
							foreach(Transform s in chess.GetComponent<CharacterSelect>().MoveRangeList){
								s.GetComponent<Identy>().step = 0;
							}
							chess.GetComponent<CharacterSelect>().MoveRangeList.Clear();
						}
					}
				}
			}
		}
    }
}
