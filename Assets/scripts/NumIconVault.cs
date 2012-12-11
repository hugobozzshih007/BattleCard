using UnityEngine;
using System.Collections;

public class NumIconVault : MonoBehaviour {
	
	public Texture2D[] Mana = new Texture2D[5];
	public Texture2D[] CoolDown = new Texture2D[10];
	
	// Use this for initialization
	void Start () {
	
	}
	
	public Texture2D GetManaTexture(int manaCost){
		return Mana[manaCost-1];
	}
	
	public Texture2D GetCDTexture(int round){
		return CoolDown[round];
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
