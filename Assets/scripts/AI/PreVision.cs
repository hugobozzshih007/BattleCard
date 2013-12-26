using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;
public class PreVision{

	public PreVision(){
	}
	
	public static Transform GetDirectionMap(Transform chess){
		Transform finalDest = null;
		CharacterProperty chessP = chess.GetComponent<CharacterProperty>();
		RoundCounter rc = Camera.mainCamera.GetComponent<RoundCounter>();
		PlacePrizes pp = GameObject.Find("Maps").transform.GetComponent<PlacePrizes>();
		selection sel = Camera.main.GetComponent<selection>();
		int step = chess.GetComponent<CharacterProperty>().BuffMoveRange;
		IList targetMaps = GetMapTargetList();
		IList roundMaps = new List<Transform>();
		Transform localMap = chess.GetComponent<CharacterSelect>().getMapPosition();
		sel.updateMapSteps();
		chess.GetComponent<CharacterSelect>().findMoveRange(localMap,0,step);
		foreach(Transform m in chess.GetComponent<CharacterSelect>().MoveRangeList){
			roundMaps.Add(m);
		}
		
		Transform closeMap = MapHelper.GetClosestMap(localMap, targetMaps);
		Transform dirMapA = MapHelper.GetClosestMap(closeMap, roundMaps);
		Transform closestMap = MapHelper.GetClosestMap(localMap, rc.PlayerATerritory);
		Transform dirMapB = MapHelper.GetClosestMap(closestMap, roundMaps);
		
		if(!chessP.LeadingCharacter && !chessP.Summoner)
			finalDest = dirMapB;
		else
			finalDest = dirMapA;
		
		foreach(Transform m in roundMaps){
			Identy mID = m.GetComponent<Identy>();
			if(mID.PrizeYel)
				finalDest = m;
		}
		
		if(pp.GetPrizeMap().Count>0){
			foreach(Transform m in roundMaps){
				if(pp.GetPrizeMap().Contains(m)){
					finalDest = m;
					break;
				}
			}
		}
		/*if(roundMaps.Count == 0){
			finalDest = null;	
		}*/
		
		return finalDest;
	}
	
	static IList GetMapTargetList(){
		
		if(GetMapRoundBy(6).Count>0)
			return GetMapRoundBy(6);
		else if(GetMapRoundBy(5).Count>0)
			return GetMapRoundBy(5);
		else if(GetMapRoundBy(4).Count>0)
			return GetMapRoundBy(4);
		else if(GetMapRoundBy(3).Count>0)
			return GetMapRoundBy(3);
		else if(GetMapRoundBy(2).Count>0)
			return GetMapRoundBy(2);
		else if(GetMapRoundBy(1).Count>0)
			return GetMapRoundBy(1);
		else 
			return GetMapRoundBy(0);
		
	}
	
	static IList GetMapRoundBy(int roundNum){
		IList targetMaps = new List<Transform>();
		RoundCounter rc = Camera.mainCamera.GetComponent<RoundCounter>();
		foreach(Transform m in rc.PlayerATerritory){
			Identy mID = m.GetComponent<Identy>();
			int count = 0;  
			foreach(Transform n in mID.neighbor){
				if((n!=null)&& rc.PlayerATerritory.Contains(n))
					count += 1;
			}
			if(count == roundNum){
				targetMaps.Add(m);
			}
		}
		return targetMaps; 
	}
	
}
