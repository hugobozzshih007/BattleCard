using UnityEngine;
using System.Collections;

public class SetPillars : MonoBehaviour {
	GuardianStorage gfStorage; 
	public Material BrightPillar; 
	public Material DarkPillar; 
	
	// Use this for initialization
	void Start () {
		gfStorage = GameObject.Find("PlayerData").transform.GetComponent<GuardianStorage>();
		int pillarNum = gfStorage.Guardians.Count;
		for(int i=0; i<transform.childCount; i++){
			ChangeMaterials(transform.GetChild(i), DarkPillar);
			transform.GetChild(i).collider.enabled = false;
		}
		for(int i=0; i<pillarNum; i++){
			ChangeMaterials(transform.GetChild(i), BrightPillar);
			transform.GetChild(i).collider.enabled = true;
		}
	}
	
	void ChangeMaterials(Transform chess, Material mat){
		if(chess.childCount>0){
			for(int i=0; i<chess.childCount; i++){
				chess.GetChild(i).renderer.material = mat;
			}
		}else{
			chess.renderer.material = mat; 
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
