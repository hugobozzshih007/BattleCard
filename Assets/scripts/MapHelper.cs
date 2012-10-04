using UnityEngine;
using System.Collections;

namespace MapUtility{
	public class MapHelper
	{
		public MapHelper(){
		}
		public static bool IsMapOccupied(Transform map){
			bool occupied = true;
			if(map!=null){
				Vector3 rayDir = -map.up;
				Vector3 startPos = map.position;
				startPos.y = map.position.y+15.0f;
				Ray rayUp = new Ray(startPos, rayDir);
				RaycastHit hit;
				if(Physics.Raycast(rayUp,out hit,13.0f)){
					occupied = true;
				}else{
					occupied = false;
				}
			}else{
				occupied = true;
			}
			return occupied;
		}
		
		public static Transform GetMapOccupiedObj(Transform map){
			Transform obj = null;
			if(map!=null){
				Vector3 rayDir = -map.up;
				Vector3 startPos = map.position;
				startPos.y = map.position.y+15.0f;
				Ray rayUp = new Ray(startPos, rayDir);
				RaycastHit hit;
				if(Physics.Raycast(rayUp,out hit,20.0f)){
					obj = hit.transform;
				}
			}
			return obj;
		}
	}
}

