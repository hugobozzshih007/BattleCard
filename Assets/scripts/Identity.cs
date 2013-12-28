using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Identity : MonoBehaviour {
	public LayerMask realMapLayer;
	public bool MapUnit;
	public bool summoner;
	public bool River;
	public bool Trees;
	public int FixedSide = 3; 
	public bool PrizeRed = false;
	public bool PrizeYel = false;
	public Transform[] neighbor;
	public Material originalMat, PrizeMatRed, PrizeMatYel; 
	public int step = 0;
	public IDictionary neighbors;
	public Transform Prize = null;
	private bool test = true;
	//public Transform ShowMap; 
	
	void Start(){
		if(PrizeRed){
			transform.renderer.material = PrizeMatRed;
		}else if(MapUnit){
			transform.renderer.material = originalMat;
		}else if(PrizeYel){
			transform.renderer.material = PrizeMatYel;
		}
	}
	
	void getStructure(){
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
		
		//ShowMap = GetRealMap(transform);
	}
	
	
	Transform GetRealMap(Transform map){
		Transform obj = null;
		if(map!=null){
			Vector3 rayDir = -map.up;
			Vector3 startPos = map.position;
			startPos.y = map.position.y+1.0f;
			Ray rayUp = new Ray(startPos, rayDir);
			RaycastHit hit;
			if(Physics.Raycast(rayUp,out hit,2.0f,realMapLayer)){
				obj = hit.transform;
				//print(obj);
			}else{
				print("shit: "+ map);
			}
		}
		return obj;
	}
	
	// Use this for initialization
	void Awake () {
		if(MapUnit){
			getStructure();
		}
	}
	
	// Update is called once per frame
	void Update () {
	}
}
