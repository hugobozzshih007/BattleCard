using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;

public struct DeathUI{
	public Transform Chess;
	public Transform Attacker;
	public Vector3 FxPos;
	
	public DeathUI(Transform chess, Transform attacker){
		Chess = chess;
		Attacker = attacker;
		CommonFX cFX = Camera.mainCamera.GetComponent<CommonFX>();
		CharacterSelect cSelect = Chess.GetComponent<CharacterSelect>();
		Transform map = cSelect.getMapPosition();
		FxPos = new Vector3(map.transform.position.x,map.transform.position.y+0.1f,map.transform.position.z);
		Transform fx = Object.Instantiate(cFX.DeadOut,FxPos,Quaternion.identity) as Transform;
		Object.Destroy(GameObject.Find(fx.name).gameObject, 3.0f);
		Chess.GetComponent<SummonFX>().ActivateDying();
		//Debug.Log("EatShit");
	}
}