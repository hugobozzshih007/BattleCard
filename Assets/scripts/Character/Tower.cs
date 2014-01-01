using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;

public class Tower : MonoBehaviour {
	public Transform RingFX;
	CharacterProperty towerProperty;
	bool deadExcuted = false;
	bool liveExcuted = false;
	IList FixedMaps; 
	IList FXRings;
	CharacterSelect chessSel;  
	// Use this for initialization
	void Start () {
		towerProperty = transform.GetComponent<CharacterProperty>();
		FixedMaps = new List<Transform>();
		FXRings = new List<Transform>();
		chessSel = transform.GetComponent<CharacterSelect>();
	}
	
	public IList GetDefRange(){
		IList moveRange = new List<Transform>();
		chessSel.findMoveRange(chessSel.getMapPosition(),0,2);
		foreach(Transform map in chessSel.MoveRangeList){
			if(map!=null)
				moveRange.Add(map);
		}
		return moveRange;
	}
	
	// Update is called once per frame
	void Update () {
		if(towerProperty.Death && !deadExcuted){
			
			if(FixedMaps.Count>0){
				foreach(Transform maps in FixedMaps){
					maps.GetComponent<Identity>().FixedSide = 3;
				}
			}
			if(FXRings.Count>0){
				for(int i=0; i<FXRings.Count; i++){
					Transform ring = FXRings[i] as Transform;
					Destroy(ring.gameObject);
				}
			}
			FXRings.Clear();
			FixedMaps.Clear();
			liveExcuted = false;
			deadExcuted = true;
		}
		
		if(!towerProperty.Death && !liveExcuted){
			deadExcuted = false;
			Transform localMap = transform.GetComponent<CharacterSelect>().getMapPosition();
			FixedMaps.Add(localMap);
			foreach(Transform map in localMap.GetComponent<Identity>().Neighbor){
				if(map!=null){
					FixedMaps.Add(map);
					Transform redRing = null;
					redRing = Instantiate(RingFX, new Vector3(map.position.x, map.position.y-0.2f, map.position.z), Quaternion.identity)as Transform;
					//print();
					FXRings.Add(redRing);
				}
			}
			
			liveExcuted = true;
		}
	}
	
	void OnGUI(){
	}
}
