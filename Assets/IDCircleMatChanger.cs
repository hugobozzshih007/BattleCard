using UnityEngine;
using System.Collections;

public class IDCircleMatChanger : MonoBehaviour {
	public Material RedSide; 
	public Material YelSide; 
	public Material Selected;
	Material theSide;
	MainInfoUI chessUI; 
	// Use this for initialization
	void Start () {
		chessUI = Camera.main.GetComponent<MainInfoUI>();
		if(chessUI.PlayerSide == transform.parent.GetComponent<CharacterProperty>().Player){
			transform.renderer.material = RedSide; 
		}else{
			transform.renderer.material = YelSide;
		}
		theSide = transform.renderer.material;
	}
	
	// Update is called once per frame
	void Update () {	
		if(transform.parent.gameObject.layer == 11){
			if(transform.renderer.material != Selected){
				transform.renderer.material = Selected;
			}
		}else{
			if(transform.renderer.material != theSide){
				transform.renderer.material = theSide;
			}
		}
	}
}
