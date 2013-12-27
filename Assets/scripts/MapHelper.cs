using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MapUtility{
	public class MapHelper
	{
		public MapHelper(){
		}
		
		public static void RemovePassive(PassiveType pt, Transform chess){
			CharacterPassive chessPassive = chess.GetComponent<CharacterPassive>();
			if(CheckAddedPassive(pt, chess))
				chessPassive.PassiveDict[pt] = false;
		}
		
		public static IList GetAroundGFs(Transform map){
			IList GFs = new List<Transform>();
			
			Identy mapID = map.GetComponent<Identy>();
			foreach(Transform unit in mapID.neighbor){
				if(unit != null){
					if(IsMapOccupied(unit)){
						GFs.Add(GetMapOccupiedObj(unit));
					}
				}
			}
			
			return GFs; 
		}
		
		public static Vector3 GetCenterPos(Transform mapA, Transform mapB){
			Vector3 mapC = Vector3.Lerp(mapA.transform.position, mapB.transform.position, 0.5f); 
			return mapC; 
		}
		
		public static Transform FindAnyChildren(Transform parent, string name){
			if(parent.name == name) return parent; 
			for (int i = 0; i < parent.childCount; ++i){
				var result = FindAnyChildren(parent.GetChild(i), name);
				if (result != null) return result;
			}
			return null;
		}
		
		public static bool CheckTowerOnSite(){
			RoundCounter RC = Camera.mainCamera.GetComponent<RoundCounter>();
			bool tower = false;
			foreach(Transform gf in RC.AllChesses){
				CharacterProperty gfp = gf.GetComponent<CharacterProperty>();
				if(!gfp.death && gfp.Tower){
					tower = true;
					break;
				}
			}
			return tower;
		}
		
		public static IList GetTowerMaps(){
			RoundCounter RC = Camera.mainCamera.GetComponent<RoundCounter>();
			IList towerMaps = new List<Transform>();
			foreach(Transform gf in RC.AllChesses){
				CharacterProperty gfp = gf.GetComponent<CharacterProperty>();
				if(!gfp.death && gfp.Tower){
					IList defRange = gf.GetComponent<Tower>().GetDefRange();
					if(defRange.Count>0){
						foreach(Transform map in defRange){
							towerMaps.Add(map);
						}
					}
				}
			}
			return towerMaps; 
		}
		
		public static IList GetFarestMaps(Transform chess, IList maps){
			IList cMapList = new List<Transform>();
			if(maps.Count>0){
				Dictionary<Transform, int> sortDict = new Dictionary<Transform, int>();
				foreach(Transform loc in maps){
					float dis = Vector3.Distance(loc.transform.position, chess.transform.position);
					int disInt = Mathf.RoundToInt(dis);
					sortDict.Add(loc,disInt);
				}
				var sortedDict = (from entry in sortDict orderby entry.Value descending select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
				int farest = sortedDict.Values.ElementAt(0);
				//Debug.Log("nearest = "+nearest);
				foreach(KeyValuePair<Transform,int> entry in sortedDict){
					if(entry.Value == farest){
						cMapList.Add(entry.Key);
					}
				}
			}
			return cMapList;  
		}
		
		public static IList GetClosestMaps(Transform chess, IList maps){
			IList cMapList = new List<Transform>();
			if(maps.Count>0){
				Dictionary<Transform, int> sortDict = new Dictionary<Transform, int>();
				foreach(Transform loc in maps){
					float dis = Vector3.Distance(loc.transform.position, chess.transform.position);
					int disInt = Mathf.RoundToInt(dis);
					sortDict.Add(loc,disInt);
				}
				var sortedDict = (from entry in sortDict orderby entry.Value ascending select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
				int nearest = sortedDict.Values.ElementAt(0);
				//Debug.Log("nearest = "+nearest);
				foreach(KeyValuePair<Transform,int> entry in sortedDict){
					if(entry.Value == nearest){
						cMapList.Add(entry.Key);
					}
				}
			}
			return cMapList;  
		}
		
		public static Transform GetClosestMap(Transform chess, IList maps){
			Transform theMap = null;
			if(maps.Count>0){
				Dictionary<float,Transform> sortDict = new Dictionary<float, Transform>();
				foreach(Transform loc in maps){
					float dis = Vector3.Distance(loc.transform.position, chess.transform.position);
					if(!sortDict.ContainsKey(dis))
						sortDict.Add(dis, loc);
				}
				var list = sortDict.Keys.ToList();
				list.Sort();
				theMap = sortDict[list[0]];
			}else{
				theMap = null; 
				Debug.Log("There is nothing in the list");
			}
			return theMap;
		}
		
		public static bool Attackable(Transform chess){
			MoveCharacter mc = Camera.main.GetComponent<MoveCharacter>();
			bool able = false;
			if(!mc.MoveMode){
				AttackCalculation atc = new AttackCalculation(chess);
				able = (atc.GetAttableTarget(atc.Attacker).Count>0);
			}else{
				able = false;
			}
			return able;
		}
		
		public static void SetFX(Transform chess, Transform fx, float duration){
			Transform fxObj = Object.Instantiate(fx,chess.transform.position, Quaternion.identity) as Transform;
			Object.Destroy(GameObject.Find(fxObj.name).gameObject, duration);
			if(duration >=3.9f){
				SystemSound sSound = GameObject.Find("SystemSoundB").transform.GetComponent<SystemSound>();
				sSound.PlaySound(SysSoundFx.Buff);
			}
		}
		
		public static bool CheckPassive(PassiveType pt, Transform chess){
			bool check = false;
			CharacterPassive chessPassive = chess.GetComponent<CharacterPassive>();
			int pNum = chessPassive.PassiveAbility.Length;
			if(pNum >0){
				for(int i=0; i<pNum; i++){
					PassiveType currentPt = (PassiveType)chessPassive.PassiveAbility[i];
					if(pt == currentPt){
						check = true;
						break;
					}
				}
			}
			return check;
		}
		
		public static bool CheckBuffList(BuffType bt, Transform chess){
			bool check = false;
			BuffList chessBuff = chess.GetComponent<BuffList>();
			int pNum = chessBuff.addBuff.Length;
			if(pNum >0){
				for(int i=0; i<pNum; i++){
					BuffType currentPt = (BuffType)chessBuff.addBuff[i];
					if(bt == currentPt){
						check = true;
						break;
					}
				}
			}
			return check;
		}
		
		public static bool CheckDeBuffList(BuffType bt, Transform chess){
			bool check = false;
			BuffList chessBuff = chess.GetComponent<BuffList>();
			int pNum = chessBuff.deBuff.Length;
			if(pNum >0){
				for(int i=0; i<pNum; i++){
					BuffType currentPt = (BuffType)chessBuff.deBuff[i];
					if(bt == currentPt){
						check = true;
						break;
					}
				}
			}
			return check;
		}
		
		public static bool CheckAddedPassive(PassiveType pt, Transform chess){
			CharacterPassive chessPassive = chess.GetComponent<CharacterPassive>();
			return chessPassive.PassiveDict[pt];
		}
		
		public static void SetObjTransparent(Transform obj, Color col, float alpha){	
			Shader opaShader = Shader.Find("Transparent/Diffuse");
			//obj.renderer.material.shader = opaShader;
			//col.a = alpha;
			//obj.renderer.material.color = col;
			
			Transform model = obj.FindChild("Models");
			List<Transform> models = new List<Transform>();
			if(model.childCount>0){
				for(int i=0; i<model.childCount; i++){
					if(model.GetChild(i).GetComponent<SkinnedMeshRenderer>()!=null){
						models.Add(model.GetChild(i));
					}
				}
			}
			if(models.Count>0){
				foreach(Transform m in models){
					m.renderer.material.shader = opaShader;
					m.renderer.material.SetColor("_Color", col);
					
				}
			}
		}
		
		public static bool SetObjOldShader(Transform obj, Dictionary<string,string> shaderNames, float alpha){
			//Shader diffShader = Shader.Find("shaderName");
			//obj.renderer.material.shader = diffShader;
			//Color currentCol = obj.renderer.material.color; 
			//currentCol.a = alpha;
			//obj.renderer.material.color = currentCol;
			List<Transform> models = new List<Transform>();
			Transform model = obj.FindChild("Models");
			if(model.childCount>0){
				for(int i=0; i<model.childCount; i++){
					if(model.GetChild(i).GetComponent<SkinnedMeshRenderer>()!=null){
						models.Add(model.GetChild(i));
					}
				}
			}
			if(models.Count>0){
				for(int i=0;i<models.Count;i++){
					models[i].renderer.material.shader = Shader.Find(shaderNames[models[i].name].ToString());
					models[i].renderer.material.SetColor("_Color",Color.white);
				}
			}
			return true;
		}
		
		public static bool Success(int rate){
			bool ifSucceed = false;
			int realNum = Random.Range(0,100);
			if(realNum < rate){
				ifSucceed = true;
			}else{
				ifSucceed = false;
			}
			return ifSucceed;
		}
		
		public static bool IsMapOccupied(Transform map){
			bool occupied = false;
			if(map!=null){
				Vector3 rayDir = -map.up;
				Vector3 startPos = map.position;
				startPos.y = map.position.y+15.0f;
				Ray rayUp = new Ray(startPos, rayDir);
				RaycastHit hit;
				if(Physics.Raycast(rayUp,out hit,13.0f)){
					if(hit.transform.gameObject.layer == 10 || hit.transform.gameObject.layer == 11){
						occupied = true;
					}else{
						occupied = false;
					}
				}else{
					occupied = false;
				}
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

