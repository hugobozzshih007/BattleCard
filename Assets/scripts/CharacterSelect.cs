using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;

public class CharacterSelect : MonoBehaviour {
	public IList MoveRangeList;
	public IList AttackRangeList;
	private SixGonRays unit;
	public Material originalMat;
	public Material rollOver;
	public Material closeBy;
	private float castLength = 20.0f;
	public bool selectedMode = false;
	CharacterProperty thisProperty;
	// Use this for initialization
	void Start () {
		MoveRangeList =  new List<Transform>();
		AttackRangeList = new List<Transform>();
		MoveRangeList.Clear();
		AttackRangeList.Clear();
		thisProperty = this.GetComponent<CharacterProperty>();
		//originalMat = GameObject.Find("unit0").transform.renderer.material;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
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
	
	public void findMoveRange(Transform root, int step, int maxStep){
		if(root!=null && (step>0)){
			Identy rootID = root.GetComponent<Identy>();
			if(!rootID.River && !rootID.Trees && !MapUtility.MapHelper.IsMapOccupied(root)){
				if (maxStep < step){
					return;
				}else if(maxStep == step){
					if(rootID.step == 0 || rootID.step>step){
						rootID.step = step;
						if(!MoveRangeList.Contains(root))
							MoveRangeList.Add(root);
					}
				}else{
					if(rootID.step == 0 || rootID.step>step){
						rootID.step = step;
						if(!MoveRangeList.Contains(root))
							MoveRangeList.Add(root);
						foreach(Transform child in rootID.neighbor){
							findMoveRange(child,step+1,maxStep);
						}
					}
				}
			}
		}else if(root!=null && (step==0)){
			Identy rootID = root.GetComponent<Identy>();
			if(!rootID.River && !rootID.Trees){
				if (maxStep < step){
					return;
				}else if(maxStep == step){
					if(rootID.step == 0 || rootID.step>step){
						rootID.step = step;
						if(!MoveRangeList.Contains(root) && !MapUtility.MapHelper.IsMapOccupied(root))
							MoveRangeList.Add(root);
					}
				}else{
					if(rootID.step == 0 || rootID.step>step){
						rootID.step = step;
						if(!MoveRangeList.Contains(root)&& !MapUtility.MapHelper.IsMapOccupied(root))
							MoveRangeList.Add(root);
						foreach(Transform child in rootID.neighbor){
							findMoveRange(child,step+1,maxStep);
						}
					}
				}
			}
		}
	}
	
	public void findAttackRange(Transform root, int step, int maxStep){
		if(root!=null && (step>0)){
			Identy rootID = root.GetComponent<Identy>();
			if(!rootID.River && !rootID.Trees){
				if (maxStep < step){
					return;
				}else if(maxStep == step){
					if(rootID.step == 0 || rootID.step>step){
						rootID.step = step;
						if(!AttackRangeList.Contains(root))
							AttackRangeList.Add(root);
					}
				}else{
					if(rootID.step == 0 || rootID.step>step){
						rootID.step = step;
						if(!AttackRangeList.Contains(root))
							AttackRangeList.Add(root);
						foreach(Transform child in rootID.neighbor){
							findAttackRange(child,step+1,maxStep);
						}
					}
				}
			}
		}else if(root!=null && (step==0)){
			Identy rootID = root.GetComponent<Identy>();
			if(!rootID.River && !rootID.Trees){
				if (maxStep < step){
					return;
				}else if(maxStep == step){
					if(rootID.step == 0 || rootID.step>step){
						rootID.step = step;
						if(!AttackRangeList.Contains(root))
							AttackRangeList.Add(root);
					}
				}else{
					if(rootID.step == 0 || rootID.step>step){
						rootID.step = step;
						if(!AttackRangeList.Contains(root))
							AttackRangeList.Add(root);
						foreach(Transform child in rootID.neighbor){
							findAttackRange(child,step+1,maxStep);
						}
					}
				}
			}
		}
	}
	
	void OnApplicationQuit(){
		MoveRangeList.Clear();
		AttackRangeList.Clear();
	}
}
