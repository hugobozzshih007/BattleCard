using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MapUtility;

public class CharacterSelect : MonoBehaviour {
	public IList MoveRangeList = new List<Transform>();
	public IList AttackRangeList = new List<Transform>();
	private SixGonRays unit;
	public Material originalMat;
	public Material rollOver;
	public Material closeBy;
	private float castLength = 20.0f;
	public bool selectedMode = false;
	public AudioClip[] Voice_select = new AudioClip[4]; 
	public AudioClip Angry_voice_select; 
	CharacterProperty thisProperty;
	// Use this for initialization
	void Start () {
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
			Identity rootID = root.GetComponent<Identity>();
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
			Identity rootID = root.GetComponent<Identity>();
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
	
	public IList FindPathList(Transform root,int maxStep, Transform destination){
		IList pathList = new List<Transform>();
		if(root!=null){
			Identity rootID = root.GetComponent<Identity>();
			
			if(maxStep == 1){
				pathList.Add(root);
				pathList.Add(destination);
			}else if(maxStep == 2){
				pathList.Add(root);
				Dictionary<float, Transform> sortingDict = new Dictionary<float,Transform>();
				foreach(Transform t in rootID.neighbor){
					if(t!=null){
						Identity tID = t.GetComponent<Identity>();
						if(!tID.River && !tID.Trees && !MapUtility.MapHelper.IsMapOccupied(t)){
							float dis = Vector3.Distance(t.transform.position, destination.transform.position);
							if(!sortingDict.ContainsKey(dis))
								sortingDict.Add(dis,t);
						}
					}
				}
				var list = sortingDict.Keys.ToList();
				list.Sort();
				Transform midPath = sortingDict[list[0]];
				pathList.Add(midPath);
				
				pathList.Add(destination);
				
			}else if(maxStep == 3){
				pathList.Add(root);
				Dictionary<float, Transform> sortingDict = new Dictionary<float,Transform>();
				foreach(Transform t in rootID.neighbor){
					if(t!=null){
						Identity tID = t.GetComponent<Identity>();
						if(!tID.River && !tID.Trees && !MapUtility.MapHelper.IsMapOccupied(t)){
							float dis = Vector3.Distance(t.transform.position, destination.transform.position);
							if(!sortingDict.ContainsKey(dis))
								sortingDict.Add(dis,t);
						}
					}
				}
				var list = sortingDict.Keys.ToList();
				list.Sort();
				Transform midPath = sortingDict[list[0]];
				pathList.Add(midPath);
				
				sortingDict.Clear();
				list.Clear();
				Identity midPathID = midPath.GetComponent<Identity>();
				foreach(Transform t in midPathID.neighbor){
					if(t!=null){
						Identity tID = t.GetComponent<Identity>();
						if(!tID.River && !tID.Trees && !MapUtility.MapHelper.IsMapOccupied(t)){
							float dis = Vector3.Distance(t.transform.position, destination.transform.position);
							if(!sortingDict.ContainsKey(dis))
								sortingDict.Add(dis,t);
						}
					}
				}
				list = sortingDict.Keys.ToList();
				list.Sort();
				Transform midPathB = sortingDict[list[0]];
				pathList.Add(midPathB);
				
				pathList.Add(destination);
			}
		}
		return pathList;
	}
	
	public void findAttackRange(Transform root, int step, int maxStep){
		if(root!=null && (step>0)){
			Identity rootID = root.GetComponent<Identity>();
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
			Identity rootID = root.GetComponent<Identity>();
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
